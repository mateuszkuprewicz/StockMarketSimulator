using System.ComponentModel.DataAnnotations;
using StockSimulator.Models.Database;

namespace StockSimulator.Models.Database;

public class UserShare
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public User? User { get; set; }
    public int CompanyId { get; set; }
    public Company? Company { get; set; }
    
    [ConcurrencyCheck]
    public int Quantity { get; set; }
}