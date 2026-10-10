namespace StockSimulator.Models.Database;

public class Stock
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int TotalShares { get; set; }
    public int FreeFloat { get; set; }
    public decimal CompanyBudget { get; set; }
}