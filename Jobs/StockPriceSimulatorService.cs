using Microsoft.EntityFrameworkCore;
using StockSimulator.Models;

namespace StockSimulator.Jobs;

public class StockPriceSimulatorService : BackgroundService
{
    private readonly  ILogger<StockPriceSimulatorService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    public StockPriceSimulatorService(IServiceScopeFactory serviceScopeFactory,
        ILogger<StockPriceSimulatorService> logger)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Stock Price Simulator service started");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await using var scope = _serviceScopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<StockSimulatorDbContext>();
                await context.Database.EnsureCreatedAsync(stoppingToken);

                var stocks = await context.Stocks.ToListAsync(stoppingToken);
                foreach (var stock in stocks)
                {
                    decimal diffPercent = Random.Shared.Next(-5, 6);
                    stock.Price+=stock.Price*(diffPercent/100m);
                    stock.Price = Math.Round(stock.Price, 2);
                    if (stock.Price <= 0)
                    {
                        stock.Price = 0.01m;
                    }
                }
                await context.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Stock prices updated");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(5, 20)), stoppingToken);
        }
    }
}