using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using StatsHub.Api.Data;
using StatsHub.Api.Models;

namespace StatsHub.Api.Services;

public class OrderService
{
    private readonly StatsHubContext _context;
    private readonly IMemoryCache _cache;

    public OrderService(StatsHubContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<int> SyncOrdersAsync(List<Order> orders)
    {
        var existingOrderIds = await _context.Orders
            .Select(o => o.OrderId)
            .ToListAsync();

        var newOrders = orders
            .Where(o => !existingOrderIds.Contains(o.OrderId))
            .ToList();

        await _context.Orders.AddRangeAsync(newOrders);
        await _context.SaveChangesAsync();

        return newOrders.Count;
    }

    public async Task<Dictionary<string, decimal>> GetBrandRevenueStatsAsync()
    {
        var cacheKey = "BrandRevenueStats";
        if (_cache.TryGetValue(cacheKey, out Dictionary<string, decimal>? cachedStats) && cachedStats != null)
        {
            return cachedStats;
        }

        var stats = await _context.Orders
            .GroupBy(o => o.BrandName)
            .Select(g => new
            {
                BrandName = g.Key,
                Revenue = g.Sum(o => o.Price * o.Quantity)
            })
            .ToDictionaryAsync(x => x.BrandName, x => x.Revenue);

        _cache.Set(cacheKey, stats, TimeSpan.FromMinutes(5));
        return stats;
    }

    public async Task<List<object>> GetDailyRevenueStatsAsync()
    {
        return await _context.Orders
            .GroupBy(o => o.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Revenue = g.Sum(o => o.Price * o.Quantity)
            })
            .OrderBy(x => x.Date)
            .ToListAsync<object>();
    }
}