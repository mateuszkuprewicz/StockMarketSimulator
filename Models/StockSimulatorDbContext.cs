using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace StockSimulator.Models;

public class StockSimulatorDbContext : DbContext
{
    public StockSimulatorDbContext(DbContextOptions<StockSimulatorDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Transaction>  Transactions { get; set; }
    public DbSet<UserShare> UserShares { get; set; }
    
}