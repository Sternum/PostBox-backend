using BCryptPasswordEncryptor;
using ForumSchoolProject.Models;
using ForumSchoolProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



public class UserService : IUserService
{
    private readonly ProjektGContext _context;
    private readonly IPasswordEncryptor _passwordEncryptor;

    public UserService(ProjektGContext context, IPasswordEncryptor passwordEncryptor)
    {
        _context = context;
        _passwordEncryptor = passwordEncryptor;
    }

    public async Task<bool> ValidateUserAsync(LoginModel login)
    {
        if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Password))
            return false;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login.Username);
        if (user == null)
            return false;

        return _passwordEncryptor.Verify(login.Password, user.Password);
    }
    public async Task<string> EncryptPassword(string password)
    {
        return _passwordEncryptor.Encrypt(password);
    }
    public async Task<User> GetUserAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Login == username);
    }
    public string GenerateJwtToken(User user)
    {
        var jwtSecretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Uid.ToString()),
                new Claim(ClaimTypes.Role, user.UserGroupId.ToString())

            };

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddHours(3),
            claims: claims,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
