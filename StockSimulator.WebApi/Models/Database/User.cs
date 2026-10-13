using Microsoft.AspNetCore.Identity;

namespace StockSimulator.Models;

public class User : IdentityUser
{
    public decimal Money { get; set; }
}