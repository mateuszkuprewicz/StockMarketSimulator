using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockSimulator.Models;
using StockSimulator.Models.DTOs;

namespace StockSimulator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AuthenticationController(SignInManager<User> signInManager,  UserManager<User> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var currentUser = await _userManager.GetUserAsync(User);
        
        if (currentUser == null)
        {
            return Unauthorized(new { message = "User not found." });
        }

        var userInfo = new
        {
            Id = currentUser.Id,
            Email = currentUser.Email,
            Balance = currentUser.Money
        };
        return Ok(userInfo);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO requestDto)
    {
        var temp = await _userManager.GetUserAsync(User);
        if(temp != null)
            return BadRequest(new { message = "User is logged in." });
        var user = new User
        {
            Email = requestDto.login,
            UserName = requestDto.login,
            Money = 1000
        };
        var result = await _userManager.CreateAsync(user, requestDto.password);
        if (result.Succeeded)
        {
            return await Login(requestDto);
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegisterRequestDTO requestDto)
    {
        var user = await _userManager.GetUserAsync(User);
        if(user != null)
            return BadRequest(new { message = "You are already logged in. Log out first"});
        var result = await _signInManager.PasswordSignInAsync(requestDto.login, requestDto.password, requestDto.rememberMe, true);
        if (result.Succeeded)
            return Ok(new { message = "User logged in successfully." });
        return Unauthorized();
    }
    
    [HttpPost("logout")]
    [Authorize] 
    public async Task<IActionResult> Logout() 
    {
        await _signInManager.SignOutAsync();
        
        return Ok(new { message = "Logged out Successfully." });
    }
}