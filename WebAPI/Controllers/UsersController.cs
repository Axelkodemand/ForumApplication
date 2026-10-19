using APIContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepo;

    public UsersController(IUserRepository userRepo)
    {
        this.userRepo = userRepo;
    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> AddUser([FromBody] CreateUserDto request)
    {
        try
        {
            VerifyUserNameIsAvailable(request.Username);

            User user = new User { Username = request.Username, Password = request.Password };
            User created = await userRepo.AddAsync(user);
            UserDTO dto = new()
            {
                Id = created.Id,
                Username = created.Username
            };
            return Created($"/users/{dto.Id}", dto);
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
    public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] UpdateUserDto request)
    {
        try
        {
            VerifyUserNameIsAvailable(request.Username, id);

            User user = new User { Id = id, Username = request.Username, Password = request.Password };
            await userRepo.UpdateAsync(user);
            return NoContent();
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
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
    public async Task<ActionResult<UserDTO>> GetUser([FromRoute] int id)
    {
        try
        {
            User user = await userRepo.GetSingleAsync(id);
            return Ok(new UserDTO { Id = user.Id, Username = user.Username });
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
    public ActionResult<IEnumerable<UserDTO>> GetUsers([FromQuery] string? usernameContains)
    {
        try
        {
            IQueryable<User> users = userRepo.GetManyAsync();

            if (!string.IsNullOrWhiteSpace(usernameContains))
            {
                users = users.Where(u => u.Username.ToLower().Contains(usernameContains.ToLower()));
            }

            List<UserDTO> dtos = users
                .Select(u => new UserDTO { Id = u.Id, Username = u.Username })
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
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        try
        {
            await userRepo.DeleteAsync(id);
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

    private void VerifyUserNameIsAvailable(string username, int? ignoreId = null)
    {
        bool taken = userRepo.GetManyAsync()
            .Any(u => u.Id != ignoreId && u.Username.ToLower() == username.ToLower());

        if (taken)
        {
            throw new ArgumentException($"Username '{username}' is already taken.");
        }
    }
}