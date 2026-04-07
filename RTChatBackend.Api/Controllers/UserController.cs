using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RTChatBackend.Api.DTOs;
using RTChatBackend.Application.Interfaces;

namespace RTChatBackend.Api.Controllers;

[ApiController]
[Route("api/users/")]
public class UserController(
    IUserService userService
) : ControllerBase
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
        if (userDto == null) return Unauthorized();

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userDto.UserId.ToString()),
            new(ClaimTypes.Name, userDto.Username)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(15)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return Ok(userDto);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok();
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query = "")
    {
        var usernames = await userService.SearchAsync(query);
        return Ok(usernames);
    }
}