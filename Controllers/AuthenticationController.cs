using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockSimulator.Models;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new User
        {
            Email = request.login,
            UserName = request.login,
            Money = 1000
        };
        var result = await _userManager.CreateAsync(user, request.password);
        if (result.Succeeded)
        {
            return await Login(request);
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] RegisterRequest request)
    {
        var result = await _signInManager.PasswordSignInAsync(request.login, request.password, request.rememberMe, true);
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


