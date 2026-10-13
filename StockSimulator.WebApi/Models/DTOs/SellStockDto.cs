namespace StockSimulator.Models.DTOs;

public class SellStockDto
{
    public int CompanyId { get; set; }
    public int Price { get; set; }
    public int Count { get; set; }
}