using StockSimulator.Models.Database;

namespace StockSimulator.Models.Database;

public class UserShare
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required User User { get; set; }
    public int StockId { get; set; }
    public required  Stock Stock { get; set; }
    public int Quantity { get; set; }
}