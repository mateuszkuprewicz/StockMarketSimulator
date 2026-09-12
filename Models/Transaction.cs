namespace StockSimulator.Models;
using System;


public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required User User { get; set; }
    public int StockId { get; set; }
    public required Stock Stock { get; set; }
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