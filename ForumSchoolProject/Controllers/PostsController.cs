using ForumSchoolProject.Authorization;
using ForumSchoolProject.Models;
using ForumSchoolProject.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ForumSchoolProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : Controller
    {
        ProjektGContext _context;
        ILogger<PostsController> _logger;
        IAuthorizationHelperService _authorizationHelperService;
        IUserService _userService;

        public PostsController(ProjektGContext context, ILogger<PostsController> logger, IAuthorizationHelperService authorizationHelperService, IUserService userService)
        {
            _context = context;
            _logger = logger;
            _authorizationHelperService = authorizationHelperService;
            _userService = userService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<GetPostDto>>> Get()
        {
            var posts = await _context.Posts
                .Select(p => new GetPostDto
                {
                    Pid = p.Pid,
                    PostDate = p.PostDate,
                    EditDate = p.EditDate,
                    PostDescription = p.PostDescription,
                    Uid = p.Uid
                })
                .ToListAsync();

            return Ok(posts);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<GetPostDto>> GetById(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            var postDto = new GetPostDto
            {
                Pid = post.Pid,
                PostDate = post.PostDate,
                EditDate = post.EditDate,
                PostDescription = post.PostDescription,
                Uid = post.Uid
            };

            return Ok(postDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Post>> Create(CreatePostDto postDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userIdfromClaims = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var post = new Post
            {
                PostDate = DateTime.Now,
                PostDescription = postDto.PostDescription,
                Uid = userIdfromClaims
            };
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            int generatedPid = post.Pid;

            return CreatedAtAction(nameof(GetById), new { id = generatedPid }, post);
        }


        [Authorize]
        [HttpPut]
        public async Task<ActionResult> Update(int postId, string description)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (!_authorizationHelperService.IsAdminOrOwner(post.Uid))
            {
                return Unauthorized();
            }
            post.EditDate = DateTime.Now;
            post.PostDescription = description;
            _context.Entry(post).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Posts.Any(e => e.Pid == post.Pid))
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

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<GetPostDto>> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            if (!_authorizationHelperService.IsAdminOrOwner(post.Uid))
            {
                return Unauthorized();
            }
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
