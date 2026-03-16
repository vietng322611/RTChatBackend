using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RTChatBackend.Api.DTOs;
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
        var userDto = await userService.GetByUsernameAsync(username);
        return userDto == null ? NotFound() : Ok(userDto);
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
        var userDto = await userService.LoginAsync(request.LoginCode);
        return userDto == null ? Unauthorized() : Ok(userDto);
    }
    
    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string username)
    {
        var userDtos = await userService.SearchAsync(username);
        return Ok(userDtos);
    }
}