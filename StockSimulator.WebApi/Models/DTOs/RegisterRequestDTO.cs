using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockSimulator.Models.DTOs;

public class RegisterRequestDTO
{
    [Required]
    public string login { get; set; }
    [Required]
    public string password { get; set; }
    [DefaultValue(false)]
    public bool rememberMe { get; set; }
}
