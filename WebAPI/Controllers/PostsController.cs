using APIContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepo;
    private readonly IUserRepository userRepo;

    public PostsController(IPostRepository postRepo, IUserRepository userRepo)
    {
        this.postRepo = postRepo;
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<PostDTO>> AddPost([FromBody] CreatePostDTO request)
    {
        try
        {
            VerifyUserExists(request.UserId);

            Post post = new Post { Title = request.Title, Body = request.Body, UserId = request.UserId };
            Post created = await postRepo.AddAsync(post);
            PostDTO DTO = new()
            {
                Id = created.Id,
                Title = created.Title,
                Body = created.Body,
                UserId = created.UserId
            };
            return Created($"/posts/{DTO.Id}", DTO);
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost([FromRoute] int id, [FromBody] UpdatePostDTO request)
    {
        try
        {
            Post existing = await postRepo.GetSingleAsync(id);

            Post post = new Post { Id = id, Title = request.Title, Body = request.Body, UserId = existing.UserId };
            await postRepo.UpdateAsync(post);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDTO>> GetPost([FromRoute] int id)
    {
        try
        {
            Post post = await postRepo.GetSingleAsync(id);
            return Ok(new PostDTO { Id = post.Id, Title = post.Title, Body = post.Body, UserId = post.UserId });
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDTO>> GetPosts(
        [FromQuery] string? titleContains,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        try
        {
            IQueryable<Post> posts = postRepo.GetManyAsync();

            if (!string.IsNullOrWhiteSpace(titleContains))
            {
                posts = posts.Where(p => p.Title.ToLower().Contains(titleContains.ToLower()));
            }

            if (userId.HasValue)
            {
                posts = posts.Where(p => p.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                List<int> matchingUserIds = userRepo.GetManyAsync()
                    .Where(u => u.Username.ToLower() == username.ToLower())
                    .Select(u => u.Id)
                    .ToList();

                posts = posts.Where(p => matchingUserIds.Contains(p.UserId));
            }

            List<PostDTO> DTOs = posts
                .Select(p => new PostDTO { Id = p.Id, Title = p.Title, Body = p.Body, UserId = p.UserId })
                .ToList();

            return Ok(DTOs);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost([FromRoute] int id)
    {
        try
        {
            await postRepo.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    private void VerifyUserExists(int userId)
    {
        bool exists = userRepo.GetManyAsync().Any(u => u.Id == userId);

        if (!exists)
        {
            throw new ArgumentException($"User with ID {userId} does not exist.");
        }
    }
}