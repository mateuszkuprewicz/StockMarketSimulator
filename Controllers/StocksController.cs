using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSimulator.Models;

namespace StockSimulator.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StocksController : ControllerBase
{
    private readonly StockSimulatorDbContext _context;
    public StocksController(StockSimulatorDbContext context)
    {
        _context = context;
    }

    [HttpGet()]
    public async Task<ActionResult<IEnumerable<Stock>>> GetStocks()
    {
        return await _context.Stocks.ToListAsync();
    }
}