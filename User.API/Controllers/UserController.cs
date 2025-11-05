using Microsoft.AspNetCore.Mvc;
using User.API.BusinessLogic.Abstractions;
using User.API.Dtos;

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
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetUserById(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
