using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ForumSchoolProject.Models;
using ForumSchoolProject.Authorization;
using BCryptPasswordEncryptor;
using Microsoft.EntityFrameworkCore;
using ForumSchoolProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ProjektGContext _context;
    private readonly TokenGenerator _tokenGenerator;
    private readonly IUserService _userService;

    public AuthController(ProjektGContext context , TokenGenerator tokenGenerator, IUserService userService)
    {
        _context = context;
        _tokenGenerator = tokenGenerator;
        _userService = userService;
    }
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        if (!ModelState.IsValid)
        {
            // Returns a 400 Bad Request response with validation errors
            return BadRequest(ModelState);
        }

        if (await _userService.ValidateUserAsync(login))
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == login.Username);

            var token = _tokenGenerator.GenerateJwtToken(user);
            return Ok(token);
        }

        return Unauthorized("Invalid credentials");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel register)
    {
        if (!ModelState.IsValid)
        {
            // Returns a 400 Bad Request response with validation errors
            return BadRequest(ModelState);
        }

        if (await _context.Users.AnyAsync(u => u.Login == register.Username))
        {
            return BadRequest("Username already exists");
        }

        var hashedPassword = await _userService.EncryptPassword(register.Password);
        var user = new User { Login = register.Username, Password = hashedPassword, LastName = register.LastName, Name = register.Name, UserGroupId = 2 };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var token = _tokenGenerator.GenerateJwtToken(user);
        return Ok(new { Token = token, Message = "Registration successful" });
    }
}

