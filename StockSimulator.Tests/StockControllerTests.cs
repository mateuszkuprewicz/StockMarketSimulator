using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using StockSimulator.Controllers;
using StockSimulator.Models; 
using Microsoft.AspNetCore.Identity;

namespace StockSimulator.Tests;

public class StocksControllerTests
{
    //BUY
    [Fact]
    public async Task BuyShares_WithSufficientFunds_UpdatesBalanceAndReturnsOk()
    {
        //Arrange
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();

        int stockId = 999;
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = 1000m };
        var stock = new Stock { Id = stockId, Name = "Apple", Price = 100m };
        
        context.Users.Add(user);
        context.Stocks.Add(stock);
        context.SaveChanges();

        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new BuyStockDto 
        { 
            Id = stockId, 
            Count = 1, 
        };
        
        //Act
        var result = await controller.BuyStocks(dto);
        
        //Assert
        Assert.IsType<OkObjectResult>(result); 

        var dbUser = await context.Users.FindAsync("user-1");
        Assert.NotNull(dbUser);
        Assert.Equal(900m, dbUser.Money);

        var userShare = await context.UserShares.SingleOrDefaultAsync();
        Assert.NotNull(userShare);
        Assert.Equal(stockId, userShare.StockId);
        Assert.Equal(1, userShare.Quantity);
        Assert.Equal("user-1", userShare.UserId);

        var transaction = await context.Transactions.SingleOrDefaultAsync();
        Assert.NotNull(transaction);
        Assert.Equal(stockId, transaction.StockId);
        Assert.Equal(1, transaction.Quantity);
        Assert.Equal(100m, transaction.Price);
        Assert.Equal("user-1", transaction.UserId);
                
    }

    [Fact]
    public async Task BuyShares_WithUnsufficientFunds_ReturnsBadRequest()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();

        int stockId = 999;
        decimal userMoney = 100m;
        decimal stockPrice = 200m;
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = userMoney };
        var stock = new Stock { Id = stockId, Name = "Apple", Price = stockPrice };
        
        context.Users.Add(user);
        context.Stocks.Add(stock);
        context.SaveChanges();

        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new BuyStockDto 
        { 
            Id = stockId, 
            Count = 1, 
        };
        
        var result = await controller.BuyStocks(dto);
        
        Assert.IsType<BadRequestObjectResult>(result);
        
        var dbUser = await context.Users.FindAsync("user-1");
        Assert.Equal(userMoney,  dbUser.Money);
        
        var userShare = await context.UserShares.SingleOrDefaultAsync();
        Assert.Null(userShare);
        
        var transaction = await context.Transactions.SingleOrDefaultAsync();
        Assert.Null(transaction);
        
    }

    [Fact]
    public async Task BuyNotExistingStock_ReturnsNotFound()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;
        
        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();
        
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = 100m };
        context.Users.Add(user);
        context.SaveChanges();
        
        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new BuyStockDto 
        { 
            Id = -1, 
            Count = 1, 
        };
        
        var result = await controller.BuyStocks(dto);
        
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task BuyShareNotLoggedIn_ReturnsUnauthorized()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;
        
        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();
        
        context.SaveChanges();
        
        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((User?)null);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new BuyStockDto 
        { 
            Id = -1, 
            Count = 1, 
        };
        
        var result = await controller.BuyStocks(dto);
        
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task BuyNegativeNumberOfShares_ReturnsBadRequest()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();

        int stockId = 999;
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = 1000m };
        var stock = new Stock { Id = stockId, Name = "Apple", Price = 100m };
        
        context.Users.Add(user);
        context.Stocks.Add(stock);
        context.SaveChanges();

        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new BuyStockDto 
        { 
            Id = stockId, 
            Count = -1, 
        };
        
        //Act
        var result = await controller.BuyStocks(dto);
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    
    //SELL
    [Fact]
    public async Task SellShareNotLoggedIn_ReturnsUnauthorized()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;
        
        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();
        
        context.SaveChanges();
        
        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync((User?)null);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new SellStockDto 
        { 
            Id = -1, 
            Count = 1, 
        };
        
        var result = await controller.SellStocks(dto);
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public async Task SellNotExistingStock_ReturnsNotFound()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;
        
        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();
        
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = 100m };
        context.Users.Add(user);
        context.SaveChanges();
        
        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new SellStockDto 
        { 
            Id = -1, 
            Count = 1, 
        };
        
        var result = await controller.SellStocks(dto);
        
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task SellShareNotOwned_ReturnsBadRequest()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();

        int stockId = 999;
        int userShareId = 1;
        decimal userMoney = 100m;
        decimal stockPrice = 200m;
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = userMoney };
        var stock = new Stock { Id = stockId, Name = "Apple", Price = stockPrice };
        var userShare = new UserShare(){Id = userShareId,  UserId = user.Id, User = user, StockId =  stockId, Stock = stock, Quantity = 2};
        
        context.Users.Add(user);
        context.Stocks.Add(stock);
        context.UserShares.Add(userShare);
        context.SaveChanges();

        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var request = new SellStockDto
        {
            Id = stockId,
            Count = 3
        };
        
        var result = await controller.SellStocks(request);
        
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task SellAllShareOwned_ReturnsOk()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();

        int stockId = 999; string userId = "user-1";
        int userShareId = 1;
        int userShareQuantity = 2;
        decimal userMoney = 1000m; decimal stockPrice = 100m;
        var user = new User { Id = userId, UserName = "Gamer123", Money = userMoney };
        var stock = new Stock { Id = stockId, Name = "Apple", Price = stockPrice };
        var userShare = new UserShare(){Id = userShareId,  UserId = user.Id, User = user, StockId =  stockId, Stock = stock, Quantity = userShareQuantity};
        
        context.Users.Add(user);
        context.Stocks.Add(stock);
        context.UserShares.Add(userShare);
        context.SaveChanges();

        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new SellStockDto 
        { 
            Id = stockId, 
            Count = userShareQuantity 
        };
        
        var result = await controller.SellStocks(dto);
        
        Assert.IsType<OkObjectResult>(result);
        
        var dbUser = await context.Users.SingleOrDefaultAsync();
        Assert.NotNull(dbUser);
        Assert.Equal(userMoney + userShareQuantity * stockPrice, dbUser.Money);
        
        var share = await context.UserShares.FindAsync(userShareId);
        Assert.Null(share);

        var transaction = await context.Transactions.SingleOrDefaultAsync();
        Assert.NotNull(transaction);
        Assert.Equal(userShareQuantity, transaction.Quantity);
        Assert.Equal(stockId, transaction.StockId);
        Assert.Equal(userId, transaction.UserId);
        Assert.Equal(stockPrice, transaction.Price);
        Assert.Equal(TransactionType.Sell, transaction.Type);
    }

    [Fact]
    public async Task SellNegativeNumberOfShares_ReturnsBadRequest()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<StockSimulatorDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new StockSimulatorDbContext(options);
        context.Database.EnsureCreated();

        int stockId = 999;
        var user = new User { Id = "user-1", UserName = "Gamer123", Money = 1000m };
        var stock = new Stock { Id = stockId, Name = "Apple", Price = 100m };
        
        context.Users.Add(user);
        context.Stocks.Add(stock);
        context.SaveChanges();

        var storeStub = new Mock<IUserStore<User>>();
        var userManagerStub = new Mock<UserManager<User>>(storeStub.Object, null, null, null, null, null, null, null, null);
        
        userManagerStub
            .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var controller = new StocksController(context, userManagerStub.Object);

        var dto = new SellStockDto 
        { 
            Id = stockId, 
            Count = -1, 
        };
        
        //Act
        var result = await controller.SellStocks(dto);
        Assert.IsType<BadRequestObjectResult>(result);
    }
}