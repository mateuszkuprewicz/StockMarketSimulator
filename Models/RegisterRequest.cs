using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockSimulator.Models;

public class RegisterRequest
{
    [Required]
    public string login { get; set; }
    [Required]
    public string password { get; set; }
    [DefaultValue(false)]
    public bool rememberMe { get; set; }
}
