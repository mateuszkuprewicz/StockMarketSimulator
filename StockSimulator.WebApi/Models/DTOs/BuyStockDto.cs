namespace StockSimulator.Models.DTOs;

public class BuyStockDto
{
    public int CompanyId { get; set; }
    public int MaxPrice { get; set; }
    public int Count { get; set; }
}