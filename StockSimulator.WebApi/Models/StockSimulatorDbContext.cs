using System.Collections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StockSimulator.Models;

public class StockSimulatorDbContext : IdentityDbContext<User>
{
    public StockSimulatorDbContext(DbContextOptions<StockSimulatorDbContext> options)
        : base(options)
    {
    }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Transaction>  Transactions { get; set; }
    public DbSet<UserShare> UserShares { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Stock>().HasData(
        new Stock()
        {
            Id = 1,
            Name = "World Wide Water",
            Price = 2.23m
        },
        new Stock
        {
            Id = 2,
            Name = "Ultra Evil Company Inc.",
            Price = 1024m
        },
        new Stock()
        {
            Id = 3,
            Name = "Artistic Artists' Art",
            Price = 0.5m
        },
        new Stock()
        {
            Id = 4,
            Name = "Sport TM",
            Price = 25.13m
        },
        new Stock()
        {
            Id = 5,
            Name = "Country Government S.A.",
            Price = 314m
        }
        );
    }
    
}