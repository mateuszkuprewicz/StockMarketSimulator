using System.Collections;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StockSimulator.Models.Database;

namespace StockSimulator.Models.Database;

public class StockSimulatorDbContext : IdentityDbContext<User>
{
    public StockSimulatorDbContext(DbContextOptions<StockSimulatorDbContext> options)
        : base(options)
    {
    }
    public DbSet<Stock> Stocks { get; set; }
    public DbSet<Transaction>  Transactions { get; set; }
    public DbSet<UserShare> UserShares { get; set; }
    public DbSet<MarketOffer> MarketOffers { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var www = new Stock()
        {
            Id = 1,
            Name = "World Wide Water",
            Price = 2.23m,
            TotalShares = 1000,
            FreeFloat = 10
        };

        var uec = new Stock
        {
            Id = 2,
            Name = "Ultra Evil Company Inc.",
            Price = 1024m,
            TotalShares = 500,
            FreeFloat = 8
        };
        var aaa = new Stock()
        {
            Id = 3,
            Name = "Artistic Artists' Art",
            Price = 0.5m,
            TotalShares = 100,
            FreeFloat = 12
        };
        var sport = new Stock()
        {
            Id = 4,
            Name = "Sport TM",
            Price = 25.13m,
            TotalShares = 200,
            FreeFloat = 15
        };
        var cg = new Stock()
        {
            Id = 5,
            Name = "Country Government S.A.",
            Price = 314m,
            TotalShares = 50,
            FreeFloat = 3
        };
        
        
        modelBuilder.Entity<Stock>().HasData(
            www,
            uec,
            aaa,
            sport,
            cg
        );

        modelBuilder.Entity<CompanyMarketOffer>().HasData(
            new CompanyMarketOffer()
            {
                Id = 1,
                Price = www.Price,
                Quantity = www.FreeFloat,
                StockId = www.Id,
            },
            new CompanyMarketOffer()
            {
                Id = 2,
                Price = uec.Price,
                Quantity = uec.FreeFloat,
                StockId = uec.Id,
            },
            new CompanyMarketOffer()
            {
                Id = 3,
                Price = aaa.Price,
                Quantity = aaa.FreeFloat,
                StockId = aaa.Id,
            },
            new CompanyMarketOffer()
            {
                Id = 4,
                Price = sport.Price,
                Quantity = sport.FreeFloat,
                StockId = sport.Id,
            },
            new CompanyMarketOffer()
            {
                Id = 5,
                Price = cg.Price,
                Quantity = cg.FreeFloat,
                StockId = cg.Id,
            }
        );
    }
    
}