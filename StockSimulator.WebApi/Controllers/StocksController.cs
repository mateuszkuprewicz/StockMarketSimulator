using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSimulator.Models;
using StockSimulator.Models.Database;
using StockSimulator.Models.DTOs;

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

    [HttpPost("buy")]
    [Authorize]
    public async Task<IActionResult> BuyStocks(BuyStockDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }

        if (request.Count <= 0)
            return BadRequest("You must buy a positive number of shares");
        
        var requestedStock = await _context.Stocks.SingleOrDefaultAsync(s => s.Id == request.StockId);
        if(requestedStock == null)
            return NotFound(new { message = "Stock with provided id not found." });
        
        using var transaction = _context.Database.BeginTransaction(IsolationLevel.RepeatableRead);
        
        if(user.Money <  request.Price * request.Price)
            return BadRequest(new { message = "You don't have enough money to buy that number of shares of such price" });
        decimal finalCost = 0.0m;
        
        List<MarketOffer> offers = await _context.MarketOffers.OrderBy(o => o.Price).Where(o => o.StockId == request.StockId && o.Price <= request.Price).Include(o => o.Stock).Include(o => (o as UserMarketOffer).User).ToListAsync();
        int fittingOffers = offers.Sum(fo => fo.Amount);
        if(fittingOffers < request.Count)
            return NotFound(new { message = "There are only " + fittingOffers + "shares not more expensive than your requested price." });

        int i = 0;
        int requestCount = request.Count;
        while (requestCount > 0)
        {
            int reserved = 0;
            if (offers[i].Amount > requestCount)
            {
                reserved = requestCount;
                offers[i].OnPurchase(requestCount);
                requestCount = 0;
            }
            else
            {
                reserved = offers[i].Amount;
                requestCount -= offers[i].Amount;
                offers[i].OnPurchase(offers[i].Amount);
                _context.MarketOffers.Remove(offers[i]);
            }
            finalCost += reserved * offers[i].Price;
            i++;
        }
        
        if(user.Money < finalCost)
            return BadRequest("You don't have enough money to buy that number of shares.");
        user.Money -= finalCost;
        
        _context.Transactions.Add(new Transaction()
        {
            UserId = user.Id,
            User = user,
            StockId = request.StockId,
            Stock = requestedStock,
            Quantity = request.Count,
            Price = finalCost/request.Count,
            Date = DateTime.Now,
            Type = TransactionType.Buy
        });

        UserShare? userShare = await _context.UserShares.SingleOrDefaultAsync(s => s.UserId == user.Id && s.StockId == request.StockId);
        if (userShare == null)
            _context.UserShares.Add(new UserShare()
            {
                UserId = user.Id,
                User = user,
                StockId = request.StockId,
                Stock = requestedStock,
                Quantity = request.Count
            });
        else
        {
            userShare.Quantity += request.Count;
        }
        
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return Ok(new {message = "Successfully bought " + request.Count + " " + requestedStock.Name + " shares." });
    }

    [HttpPost("sell")]
    [Authorize]
    public async Task<IActionResult> SellStocks(SellStockDto request)
    {
        using var transaction = _context.Database.BeginTransaction(IsolationLevel.RepeatableRead);
        
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }

        if (request.Count <= 0)
            return BadRequest("You must sell a positive number of shares.");
    
        Stock? requestedStock = await _context.Stocks.SingleOrDefaultAsync(s => s.Id == request.Id);
        if(requestedStock == null)
            return NotFound(new { message = "Stock with provided id not found." });
        
        var userShare =  await _context.UserShares.SingleOrDefaultAsync(s => s.UserId == user.Id && s.StockId == request.Id);
        if (userShare == null)
            return BadRequest(new { message = "You don't own this share." });
        if(request.Count > userShare.Quantity)
            return BadRequest(new { message = "You do not own " + request.Count + " shares." });
        
        user.Money += request.Count * requestedStock.Price;

        _context.Transactions.Add(new Transaction()
        {
            UserId = user.Id,
            User = user,
            StockId = request.Id,
            Stock = requestedStock,
            Quantity = request.Count,
            Price = requestedStock.Price,
            Date = DateTime.Now,
            Type = TransactionType.Sell
        });
        
        userShare.Quantity -= request.Count;
        if(userShare.Quantity == 0)
            _context.UserShares.Remove(userShare);
        await _context.SaveChangesAsync();
        transaction.Commit();

        return Ok(new { message = "Successfully sold " + request.Count + " " + requestedStock.Name + " shares." });
    }
    
    [HttpGet("transactions")]
    [Authorize]
    public async Task<IActionResult> GetTransactions()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }

        var transactions = await _context.Transactions
            .Include(t => t.Stock)
            .Where(t => t.UserId == user.Id)
            .OrderByDescending(t => t.Date)
            .Select(t => new
            {
                Id = t.Id,
                StockName = t.Stock.Name,
                Quantity = t.Quantity,
                Price = t.Price,
                Date = t.Date,
                Type = t.Type.ToString()
            })
            .ToListAsync();

        return Ok(transactions);
    }

    [HttpGet("shares")]
    [Authorize]
    public async Task<IActionResult> GetShares()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }

        var shares = await _context.UserShares
            .Include(s => s.Stock)
            .Where(s => s.UserId == user.Id)
            .Select(s => new
            {
                Id = s.Id,
                StockId = s.StockId,
                StockName = s.Stock.Name,
                Quantity = s.Quantity,
                CurrentPrice = s.Stock.Price,
                TotalValue = s.Quantity * s.Stock.Price
            })
            .ToListAsync();

        return Ok(shares);
    }

    [HttpGet("OrderBook")]
    public async Task<ActionResult<List<MarketOffer>>> GetOrderBook()
    {
        return await _context.MarketOffers.ToListAsync();
    }
}