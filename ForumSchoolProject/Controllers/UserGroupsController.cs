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
    public class UserGroupsController : ControllerBase 
    {
        private readonly ILogger<UserGroupsController> _logger;
        private readonly ProjektGContext _context;

        public UserGroupsController(ILogger<UserGroupsController> logger, ProjektGContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserGroupDto>>> Get()
        {
            var userGroups = await _context.UserGroups
                .Select(ug => new UserGroupDto
                {
                    UserGroupId = ug.UserGroupId,
                    Gdescription = ug.Gdescription,
                    AddPosts = ug.AddPosts,
                    EditPost = ug.EditPost
                })
                .ToListAsync();
            if (!userGroups.Any())
            {
                return NotFound();
            }
            return Ok(userGroups);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserGroupDto>> GetById(int id)
        {
            var userGroup = await _context.UserGroups.FindAsync(id);
            if (userGroup == null)
            {
                return NotFound();
            }
            var userGroupDto = new UserGroupDto
            {
                UserGroupId = userGroup.UserGroupId,
                Gdescription = userGroup.Gdescription,
                AddPosts = userGroup.AddPosts,
                EditPost = userGroup.EditPost
            };
            return Ok(userGroupDto);
        }
    }
}
