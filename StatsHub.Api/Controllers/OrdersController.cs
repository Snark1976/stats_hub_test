using Microsoft.AspNetCore.Mvc;
using StatsHub.Api.Models;
using StatsHub.Api.Services;

namespace StatsHub.Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(OrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> SyncOrders([FromBody] List<Order> orders)
    {
        if (orders == null || !orders.Any())
        {
            _logger.LogWarning("Получен пустой или null список заказов");
            return BadRequest("Список заказов пуст или некорректен");
        }

        try
        {
            var newOrdersCount = await _orderService.SyncOrdersAsync(orders);
            _logger.LogInformation("Успешно добавлено {Count} новых заказов", newOrdersCount);
            return Ok(new { NewOrdersCount = newOrdersCount });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при синхронизации заказов");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _orderService.GetBrandRevenueStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении статистики");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("daily-stats")]
    public async Task<IActionResult> GetDailyStats()
    {
        try
        {
            var dailyStats = await _orderService.GetDailyRevenueStatsAsync();
            return Ok(dailyStats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении ежедневной статистики");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}