using Microsoft.AspNetCore.Mvc;
using User.API.BusinessLogic.Abstractions;

namespace User.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(Data.Models.User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Data.Models.User>> GetUserById(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
