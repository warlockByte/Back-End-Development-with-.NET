using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;
using UserManagementAPI.Services;

namespace UserManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<User>> GetAll() => Ok(repository.GetAll());

    [HttpGet("{id:int}")]
    public ActionResult<User> GetById(int id)
    {
        var user = repository.GetById(id);
        return user is null
            ? NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"No user with ID {id} exists.",
                Status = StatusCodes.Status404NotFound
            })
            : Ok(user);
    }

    [HttpPost]
    public ActionResult<User> Create(UserRequest request)
    {
        var createdUser = repository.Add(request.ToUser());
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UserRequest request) =>
        repository.Update(id, request.ToUser())
            ? NoContent()
            : NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"No user with ID {id} exists.",
                Status = StatusCodes.Status404NotFound
            });

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id) =>
        repository.Delete(id)
            ? NoContent()
            : NotFound(new ProblemDetails
            {
                Title = "User not found",
                Detail = $"No user with ID {id} exists.",
                Status = StatusCodes.Status404NotFound
            });
}
