using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ForumSchoolProject.Models;
using ForumSchoolProject.Authorization;
using BCryptPasswordEncryptor;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ProjektGContext _context;
    private readonly TokenGenerator _tokenGenerator;
    private readonly IPasswordEncryptor _passwordEncryptor;

    public AuthController(ProjektGContext context , TokenGenerator tokenGenerator, IPasswordEncryptor passwordEncryptor)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _passwordEncryptor = passwordEncryptor;
    }

    [HttpPost("login")]
    public ActionResult<string> Login([FromBody] LoginModel login)
    {
        if (!ModelState.IsValid)
        {
            // Returns a 400 Bad Request response with validation errors
            return BadRequest(ModelState);
        }
        // Example of user validation. Replace with your actual user validation logic
        if (ValidateUser(login))
        {
            var user = _context.Users.Where(u => u.Login == login.Username).FirstOrDefault();

            var token = _tokenGenerator.GenerateJwtToken(user);
            return Ok(token);
        }

        return Unauthorized("Invalid credentials");
    }
    [HttpPost("register")]
    public ActionResult<string> Register([FromBody] RegisterModel register)
    {
        if (!ModelState.IsValid)
        {
            // Returns a 400 Bad Request response with validation errors
            return BadRequest(ModelState);
        }

        if (_context.Users.Any(u => u.Login == register.Username))
        {
            return BadRequest("Username already exists");
        }

        var hashedPassword = _passwordEncryptor.Encrypt(register.Password);
        var user = new User { Login = register.Username, Password = hashedPassword, LastName = register.LastName, Name = register.Name, UserGroupId =  2};

        _context.Users.Add(user);
        _context.SaveChanges();

        var token = _tokenGenerator.GenerateJwtToken(user);
        return Ok(new { Token = token, Message = "Registration successful" });
    }

    private bool ValidateUser(LoginModel login)
    {
        // Here you should check the provided credentials against your user store
        // For demo purposes, let's assume any user is valid if they provide a password
        if (!string.IsNullOrEmpty(login.Username) && !string.IsNullOrEmpty(login.Password))
        { 
            var user = _context.Users.Where(u => u.Login == login.Username).FirstOrDefault();
            return _passwordEncryptor.Verify(login.Password, user.Password);
        }
        else
        {
            return false;
        }
    }


}

