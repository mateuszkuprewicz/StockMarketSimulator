namespace StockSimulator.Models;

public class UserShare
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int StockId { get; set; }
    public required  Stock Stock { get; set; }
    public int Quantity { get; set; }
}