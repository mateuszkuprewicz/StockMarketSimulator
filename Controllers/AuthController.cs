using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StockSimulator.Models;

namespace StockSimulator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<User> _signInManager;

    public AuthController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpPost("logout")]
    [Authorize] 
    public async Task<IActionResult> Logout() 
    {
        await _signInManager.SignOutAsync();
        
        return Ok(new { message = "Logged out Successfully." });
    }
}