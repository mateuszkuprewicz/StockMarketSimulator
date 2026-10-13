using System.ComponentModel.DataAnnotations;

namespace StockSimulator.Models.Database;

public abstract class MarketOffer
{
    public int Id{ get; set; }
    public decimal Price{ get; set; }
    
    [ConcurrencyCheck]
    public int Quantity{ get; set; }
    public int CompanyId { get; set; }
    public Company Company{ get; set; }
    public abstract void OnPurchase(int requestedAmount);
}

public class UserMarketOffer : MarketOffer
{
    public required string UserId { get; set; }
    public User User { get; set; }

    public override void OnPurchase(int requestedAmount)
    {
        Quantity -= requestedAmount;
        User.Money += requestedAmount * Price;
    }
}

public class CompanyMarketOffer : MarketOffer
{
    public override void OnPurchase(int requestedAmount)
    {
        Quantity -= requestedAmount;
        Company.CompanyBudget += requestedAmount * Price;
        Company.FreeFloat -= requestedAmount;
    }
}