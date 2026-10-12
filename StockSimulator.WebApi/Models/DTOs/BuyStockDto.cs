namespace StockSimulator.Models.DTOs;

public class BuyStockDto
{
    public int StockId { get; set; }
    public int MaxPrice { get; set; }
    public int Count { get; set; }
}