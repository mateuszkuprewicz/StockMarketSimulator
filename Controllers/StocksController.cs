using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSimulator.Models;

namespace StockSimulator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController : ControllerBase
{
    private readonly StockSimulatorDbContext _context;
    private readonly UserManager<User> _userManager;
    public StocksController(StockSimulatorDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<Stock>>> GetStocks()
    {
        return await _context.Stocks.ToListAsync();
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
}