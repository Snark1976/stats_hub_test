using Microsoft.AspNetCore.Mvc;
using StatsHub_Api.Models;
using StatsHub_Api.Services;

namespace StatsHub_Api.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController(OrderService orderService, ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost("sync")]
    public async Task<IActionResult> SyncOrders([FromBody] List<Order>? orders)
    {
        if (orders == null || !orders.Any())
        {
            logger.LogWarning("Получен пустой или null список заказов");
            return BadRequest("Список заказов пуст или некорректен");
        }

        try
        {
            var newOrdersCount = await orderService.SyncOrdersAsync(orders);
            logger.LogInformation("Успешно добавлено {Count} новых заказов", newOrdersCount);
            return Ok(new { NewOrdersCount = newOrdersCount });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при синхронизации заказов");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await orderService.GetBrandRevenueStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении статистики");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }

    [HttpGet("daily-stats")]
    public async Task<IActionResult> GetDailyStats()
    {
        try
        {
            var dailyStats = await orderService.GetDailyRevenueStatsAsync();
            return Ok(dailyStats);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при получении ежедневной статистики");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}