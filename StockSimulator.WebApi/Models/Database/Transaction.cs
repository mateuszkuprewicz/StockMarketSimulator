using StockSimulator.Models.Database;

namespace StockSimulator.Models.Database;
using System;


public class Transaction
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public User User { get; set; }
    public int StockId { get; set; }
    public Stock Stock { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
}

public enum TransactionType
{
    Buy,
    Sell
}