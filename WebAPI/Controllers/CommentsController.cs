using APIContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepo;
    private readonly IUserRepository userRepo;
    private readonly IPostRepository postRepo;

    public CommentsController(ICommentRepository commentRepo, IUserRepository userRepo, IPostRepository postRepo)
    {
        this.commentRepo = commentRepo;
        this.userRepo = userRepo;
        this.postRepo = postRepo;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDTO>> AddComment([FromBody] CreateCommentDTO request)
    {
        try
        {
            VerifyUserExists(request.UserId);
            VerifyPostExists(request.PostId);

            Comment comment = new Comment { Body = request.Body, UserId = request.UserId, PostId = request.PostId };
            Comment created = await commentRepo.AddAsync(comment);
            CommentDTO dto = new()
            {
                Id = created.Id,
                Body = created.Body,
                UserId = created.UserId,
                PostId = created.PostId
            };
            return Created($"/comments/{dto.Id}", dto);
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
    public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] UpdateCommentDTO request)
    {
        try
        {
            Comment existing = await commentRepo.GetSingleAsync(id);

            Comment comment = new Comment { Id = id, Body = request.Body, UserId = existing.UserId, PostId = existing.PostId };
            await commentRepo.UpdateAsync(comment);
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
    public async Task<ActionResult<CommentDTO>> GetComment([FromRoute] int id)
    {
        try
        {
            Comment comment = await commentRepo.GetSingleAsync(id);
            return Ok(new CommentDTO { Id = comment.Id, Body = comment.Body, UserId = comment.UserId, PostId = comment.PostId });
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
    public ActionResult<IEnumerable<CommentDTO>> GetComments(
        [FromQuery] int? userId,
        [FromQuery] string? username,
        [FromQuery] int? postId)
    {
        try
        {
            IQueryable<Comment> comments = commentRepo.GetManyAsync();

            if (userId.HasValue)
            {
                comments = comments.Where(c => c.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(username))
            {
                List<int> matchingUserIds = userRepo.GetManyAsync()
                    .Where(u => u.Username.ToLower() == username.ToLower())
                    .Select(u => u.Id)
                    .ToList();

                comments = comments.Where(c => matchingUserIds.Contains(c.UserId));
            }

            if (postId.HasValue)
            {
                comments = comments.Where(c => c.PostId == postId.Value);
            }

            List<CommentDTO> dtos = comments
                .Select(c => new CommentDTO { Id = c.Id, Body = c.Body, UserId = c.UserId, PostId = c.PostId })
                .ToList();

            return Ok(dtos);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        try
        {
            await commentRepo.DeleteAsync(id);
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

    private void VerifyPostExists(int postId)
    {
        bool exists = postRepo.GetManyAsync().Any(p => p.Id == postId);

        if (!exists)
        {
            throw new ArgumentException($"Post with ID {postId} does not exist.");
        }
    }
}