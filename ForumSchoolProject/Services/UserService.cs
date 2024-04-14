using BCryptPasswordEncryptor;
using ForumSchoolProject.Models;
using ForumSchoolProject.Services.Interfaces;
using Microsoft.EntityFrameworkCore;



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
}
