using ForumSchoolProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ForumSchoolProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : Controller
    {
        ProjektGContext _context;
        ILogger<PostsController> _logger;

        public PostsController(ProjektGContext context, ILogger<PostsController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet]
        public ActionResult<GetPostDto> Get()
        {
            IQueryable<GetPostDto> query = _context.Posts.Select(p => new GetPostDto
            {
                Pid = p.Pid,
                PostDate = p.PostDate,
                EditDate = p.EditDate,
                PostDescription = p.PostDescription,
                Uid = p.Uid
            });
            var posts = query.ToList();
            return Ok(posts);
        }

        [HttpGet("{id}")]
        public ActionResult<GetPostDto> GetById(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }
            GetPostDto postDto = new GetPostDto
            {
                Pid = post.Pid,
                PostDate = post.PostDate,
                EditDate = post.EditDate,
                PostDescription = post.PostDescription,
                Uid = post.Uid
            };

            return Ok(postDto);
        }
        [HttpPost]
        public ActionResult<Post> Create(CreatePostDto postDto)
        {
            var post = new Post
            {
                PostDate = postDto.PostDate,
                EditDate = postDto.EditDate,
                PostDescription = postDto.PostDescription,
                Uid = postDto.Uid
            };
            _context.Posts.Add(post);
            _context.SaveChanges();

            int generatedPid = post.Pid;

            return CreatedAtAction(nameof(GetById), new { id = generatedPid }, post);
        }
        [HttpPut]
        public ActionResult Update(Post post)
        {
            _context.Entry(post).State = EntityState.Modified;
            try
            {
                _context.SaveChanges();
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


        [HttpDelete]
        public ActionResult<GetPostDto> Delete(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null)
            {
                return NotFound();
            }
            _context.Posts.Remove(post);
            _context.SaveChanges();

            return Ok();
        }

    }
}
