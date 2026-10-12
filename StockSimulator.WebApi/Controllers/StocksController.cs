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
        if (request.Count <= 0)
            return BadRequest("You must buy a positive number of shares");
        
        if(request.MaxPrice < 0.00m)
            return BadRequest("You must buy shares for a  positive amount of money");

        int maxTries = 10;
        for (int ii = 0; ii < maxTries; ii++)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized(new { message = "User not logged." });
            }
            
            var requestedStock = await _context.Stocks.SingleOrDefaultAsync(s => s.Id == request.StockId);
            if(requestedStock == null)
                return NotFound(new { message = "Stock with provided id not found." });
            
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead);
        
            if(user.Money <  request.MaxPrice * request.MaxPrice)
                return BadRequest(new { message = "You need at least {request.Count * request.MaxPrice:C} to place this order with a max price of {request.MaxPrice:C} per share." });
            decimal finalCost = 0.0m;
            
            List<MarketOffer> offers = await _context.MarketOffers.OrderBy(o => o.Price).Where(o => o.StockId == request.StockId && o.Price <= request.MaxPrice).Include(o => o.Stock).Include(o => (o as UserMarketOffer).User).ToListAsync();
            int fittingOffers = offers.Sum(fo => fo.Quantity);
            if(fittingOffers < request.Count)
                return NotFound(new { message = "There are only " + fittingOffers + "shares not more expensive than your requested price." });

            int i = 0;
            int requestCount = request.Count;
            while (requestCount > 0)
            {
                int reserved = 0;
                if (offers[i].Quantity > requestCount)
                {
                    reserved = requestCount;
                    offers[i].OnPurchase(requestCount);
                    requestCount = 0;
                }
                else
                {
                    reserved = offers[i].Quantity;
                    requestCount -= offers[i].Quantity;
                    offers[i].OnPurchase(offers[i].Quantity);
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
        
            try
            {
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return Ok(new {message = "Successfully bought " + request.Count + " " + requestedStock.Name + " shares." });
                
            }
            catch (DbUpdateException e)
            {
                //
                await transaction.RollbackAsync();
                _context.ChangeTracker.Clear();
                await Task.Delay(new Random().Next(50, 150));
            }
        }
        
        return Conflict("Excessive server traffic. Try again.");
    }

    [HttpPost("sell")]
    [Authorize]
    public async Task<IActionResult> SellStocks(SellStockDto request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }
        
        if (request.Count <= 0)
            return BadRequest("You must sell a positive number of shares.");

        if (request.Price < 0.0m)
            return BadRequest("You must sell for a non-negative amount of money");
        
        var requestedStock = await _context.Stocks.SingleOrDefaultAsync(s => s.Id == request.StockId);
        if(requestedStock == null)
            return NotFound(new { message = "Stock with provided id not found." });
        
        await using var transaction = await _context.Database.BeginTransactionAsync();
        
        var userShare =  await _context.UserShares.SingleOrDefaultAsync(s => s.UserId == user.Id && s.StockId == request.StockId);
        if (userShare == null)
            return BadRequest(new { message = "You don't own this share." });
        if(request.Count > userShare.Quantity)
            return BadRequest(new { message = "You do not own " + request.Count + " shares." });
        
        userShare.Quantity -= request.Count;
        if(userShare.Quantity == 0)
            _context.UserShares.Remove(userShare);

        _context.MarketOffers.Add(new UserMarketOffer()
        {
            Quantity = request.Count,
            Price = request.Price,
            UserId = user.Id,
            User = user,
            StockId = request.StockId,
            Stock = requestedStock
        });

        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            await transaction.RollbackAsync();
            return Conflict("Your another request is processed. Refresh and try again.");
        }

        return Ok(new { message = "Successfully put up " + request.Count + " " + requestedStock.Name + " shares for sales." });
    }

    [HttpPost("cancel-sell-offers")]
    [Authorize]
    public async Task<IActionResult> CancelSellOffer(CancelSellOfferDTO request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }

        var offer = await _context.MarketOffers.SingleOrDefaultAsync(o => o.Id == request.SellOfferId);
        if(offer == null)
            return  BadRequest(new { message = "This offer doesn't exist."});
        if(!(offer is UserMarketOffer marketOffer && marketOffer.UserId == user.Id ))
            return BadRequest(new { message = "You don't own this offer." });
        if(request.Quantity >  marketOffer.Quantity)
            return BadRequest(new { message = "You didn't sell so many shares in this offer." });
        
        marketOffer.Quantity -= request.Quantity;
        if(marketOffer.Quantity == 0)
            _context.MarketOffers.Remove(marketOffer);
        
        var userShare = await _context.UserShares
            .SingleOrDefaultAsync(s => s.UserId == user.Id && s.StockId == marketOffer.StockId);
        if (userShare == null)
        {
            _context.UserShares.Add(new UserShare()
            {
                UserId = user.Id,
                StockId = marketOffer.StockId, 
                Quantity = request.Quantity
            });
        }
        else
        {
            userShare.Quantity += request.Quantity;
        }

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return Conflict(("Your another request is processed. Refresh and try again"));
        }
        return Ok(new { message = "Successfully cancelled offer" });
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
    
    [HttpGet("my-sell-offers")]
    [Authorize]
    public async Task<ActionResult<List<UserMarketOffer>>> GetMySellOffers()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Unauthorized(new { message = "User not logged." });
        }

        return await _context.MarketOffers.OfType<UserMarketOffer>().Where(s => s.UserId == user.Id).ToListAsync();
    }

    [HttpGet("OrderBook")]
    public async Task<ActionResult<List<MarketOffer>>> GetOrderBook()
    {
        return await _context.MarketOffers.ToListAsync();
    }
    
}