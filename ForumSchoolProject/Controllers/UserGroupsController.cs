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
        public ActionResult<IEnumerable<UserGroupDto>> Get()
        {
            IQueryable<UserGroupDto> query = _context.UserGroups.Select(ug => new UserGroupDto
            {
                UserGroupId = ug.UserGroupId,
                Gdescription = ug.Gdescription,
                AddPosts = ug.AddPosts,
                EditPost = ug.EditPost
            });
            if (!query.Any())
            {
                return NotFound();
            }
            return Ok(query.ToList());

        }
        [HttpGet("{id}")]
        public ActionResult<UserGroupDto> GetById(int id)
        {
            var userGroup = _context.UserGroups.Find(id);
            if (userGroup == null)
            {
                return NotFound();
            }
            UserGroupDto userGroupDto = new UserGroupDto
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
