using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RTChatBackend.Api.DTOs;
using RTChatBackend.Application.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Api.Controllers;

[ApiController]
[Route("api/users/")]
public class UserController(
    IUserService userService
    ): ControllerBase
{
    [Authorize]
    [HttpGet("{username}")]
    public async Task<IActionResult> GetByUsername([FromRoute] string username)
    {
        var user = await userService.GetByUsernameAsync(username);
        return user == null ? NotFound() : Ok(user);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserRequest request)
    {
        var user = await userService.CreateAsync(request.Username);
        return user == null
            ? Conflict(new { message = "Username already exists." })
            : CreatedAtAction(nameof(GetByUsername), new { username = user.Username }, user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await userService.LoginAsync(request.LoginCode);
        return user == null ? Unauthorized() : Ok(user);
    }
}