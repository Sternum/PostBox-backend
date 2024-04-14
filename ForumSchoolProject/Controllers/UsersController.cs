using ForumSchoolProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ForumSchoolProject.Authorization;

namespace ForumSchoolProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ProjektGContext _context;
        IAuthorizationHelperService _authorizationHelperService;

        public UsersController(ILogger<UsersController> logger, ProjektGContext context, IAuthorizationHelperService authorizationHelperService)
        {
            _logger = logger;
            _context = context;
            _authorizationHelperService = authorizationHelperService;
        }

        // Combined Get and Search into a single method
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetUserDto>>> Get(string? name)
        {
            IQueryable<GetUserDto> query = _context.Users.Select(u => new GetUserDto
            {
                Name = u.Name,
                LastName = u.LastName,
                Login = u.Login,
                UserGroupId = u.UserGroupId
            });

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.Name.Contains(name) || u.LastName.Contains(name));
            }
            var users = query.ToList();
            if (!users.Any())
            {
                return NotFound();
            }
            return Ok(users);
        }

        // Get a single user by ID
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetUserDto>> GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            GetUserDto userDto = new GetUserDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Login = user.Login,
                UserGroupId = user.UserGroupId
            };
            return Ok(userDto); 
        }

        // Create a new user
        [AllowAnonymous]
        [HttpPost]
        public async Task <ActionResult<User>> Create(CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Returns detailed validation errors
            }
            var passwordEncryptor = BCryptPasswordEncryptor.Factory.CreateEncryptor();
            var hashedPassword = passwordEncryptor.Encrypt(createUserDto.Password);
            var user = new User
            {
                Name = createUserDto.Name,
                LastName = createUserDto.LastName,
                Login = createUserDto.Login,
                Password = hashedPassword,
                UserGroupId = 2
            };
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (_context.Users.Any(e => e.Login == user.Login))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction(nameof(GetById), new { id = user.Uid }, user);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  // Returns detailed validation errors
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null || !_authorizationHelperService.IsAdminOrOwner(user.Uid))
                {
                    return Forbid();
                }

                // Update only the fields that are provided in the updateUserDto
                if (!String.IsNullOrEmpty(updateUserDto.Name))
                {
                    user.Name = updateUserDto.Name;
                }
                if (!String.IsNullOrEmpty(updateUserDto.LastName))
                {
                    user.LastName = updateUserDto.LastName;
                }
                if (!String.IsNullOrEmpty(updateUserDto.Login))
                {
                    user.Login = updateUserDto.Login;
                }
                if (!String.IsNullOrEmpty(updateUserDto.Password))
                {
                    // Only update the password if it's provided
                    user.Password = BCryptPasswordEncryptor.Factory.CreateEncryptor().Encrypt(updateUserDto.Password);
                }

                _context.Entry(user).State = EntityState.Modified;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Users.Any(e => e.Uid == user.Uid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message + " " + e.InnerException);
            }

            return NoContent();
        }


        [Authorize] // Only Admins should be able to make users admins
        [HttpPost("{id}/makeAdmin")]
        public async Task<ActionResult> MakeAdmin(int id)
        {
            if (!ModelState.IsValid)
            {
                   return BadRequest(ModelState);  // Returns detailed validation errors
            }
            if (!_authorizationHelperService.IsAdmin())
            {
                return Forbid();
            }
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            user.UserGroupId = 1;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Uid == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok($"User {user.Name} {user.LastName} has been promoted to admin.");
        }
        // Delete a user by ID
        [Authorize]
        [HttpDelete("{id}")]
        public async Task <ActionResult<User>> Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            if (!_authorizationHelperService.IsAdminOrOwner(user.Uid))
            {
                return Forbid();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }
    }
}
