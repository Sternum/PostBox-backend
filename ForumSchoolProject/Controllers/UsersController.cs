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
        public ActionResult<IEnumerable<GetUserDto>> Get(string? name)
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
        public ActionResult<GetUserDto> GetById(int id)
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
        public ActionResult<User> Create(CreateUserDto createUserDto)
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
                UserGroupId = createUserDto.UserGroupId

            };

            _context.Users.Add(user);
            try
            {
                _context.SaveChanges();
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
        public ActionResult Update(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // Returns detailed validation errors
            }

            if (!_authorizationHelperService.IsAdminOrOwner(user.Uid) )
            {
                return Forbid();
            }

            user.Password = BCryptPasswordEncryptor.Factory.CreateEncryptor().Encrypt(user.Password);
            _context.Entry(user).State = EntityState.Modified;  // TODO YYYY if password is changed it has to be hashed again
            try
            {
                _context.SaveChanges();
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
            return NoContent();
        }

        // Delete a user by ID
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult<User> Delete(int id)
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
            _context.SaveChanges();

            return Ok(user);
        }
    }
}
