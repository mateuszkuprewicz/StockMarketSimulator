using System.ComponentModel.DataAnnotations;

namespace StockSimulator.Models.Database;

public abstract class MarketOffer
{
    public int Id{ get; set; }
    public decimal Price{ get; set; }
    
    [ConcurrencyCheck]
    public int Amount{ get; set; }
    public int StockId { get; set; }
    public Stock Stock{ get; set; }
    public abstract void OnPurchase(int requestedAmount);
}

public class UserMarketOffer : MarketOffer
{
    public int UserId { get; set; }
    public required User User { get; set; }

    public override void OnPurchase(int requestedAmount)
    {
        Amount -= requestedAmount;
        User.Money += requestedAmount * Price;
    }
}

public class CompanyMarketOffer : MarketOffer
{
    public override void OnPurchase(int requestedAmount)
    {
        Amount -= requestedAmount;
        Stock.CompanyBudget += requestedAmount * Price;
        Stock.FreeFloat -= requestedAmount;
    }
}