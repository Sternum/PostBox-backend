using ForumSchoolProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ForumSchoolProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ProjektGContext _context;

        public UsersController(ILogger<UsersController> logger, ProjektGContext context)
        {
            _logger = logger;
            _context = context;
        }

        // Combine Get and Search into a single method
        [HttpGet]
        public ActionResult<IEnumerable<UserDto>> Get(string? name)
        {
            IQueryable<UserDto> query = _context.Users.Select(u => new UserDto
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
        [HttpGet("{id}")]
        public ActionResult<UserDto> GetById(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            UserDto userDto = new UserDto
            {
                Name = user.Name,
                LastName = user.LastName,
                Login = user.Login,
                UserGroupId = user.UserGroupId
            };
            return Ok(userDto); 
        }

        // Create a new user
        [HttpPost]
        public ActionResult<User> Create(CreateUserDto createUserDto)
        {
            var user = new User
            {
                Name = createUserDto.Name,
                LastName = createUserDto.LastName,
                Login = createUserDto.Login,
                Password = createUserDto.Password,
                UserGroupId = createUserDto.UserGroupId

            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = user.Uid }, user);

        }


        // Update a user by ID
        [HttpPut("{id}")]
        public ActionResult Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
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
        [HttpDelete("{id}")]
        public ActionResult<User> Delete(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok(user);
        }
    }
}
