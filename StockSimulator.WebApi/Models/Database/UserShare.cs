using System.ComponentModel.DataAnnotations;
using StockSimulator.Models.Database;

namespace StockSimulator.Models.Database;

public class UserShare
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    
    [ConcurrencyCheck]
    public int Quantity { get; set; }
}