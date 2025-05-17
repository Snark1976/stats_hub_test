using StatsHub_Api.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using StatsHub_Api.Data;

namespace StatsHub.Tests;

public class OrdersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public OrdersControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<StatsHubContext>();
        db.Orders.RemoveRange(db.Orders);
        db.SaveChanges();
    }

    [Fact]
    public async Task PostOrdersSync_ValidOrders_ReturnsNewOrdersCount()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                OrderId = "ORD-1001",
                Sku = "SKU-001",
                Price = 1299.99m,
                Quantity = 1,
                CreatedAt = DateTime.Parse("2024-05-01T10:15:00Z"),
                BrandName = "Spring Sale"
            }
        };
    
        // Act
        var response = await _client.PostAsJsonAsync("/orders/sync", orders);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        var newOrdersCount = responseContent.GetProperty("newOrdersCount").GetInt32();
        Assert.Equal(1, newOrdersCount);
    }
    
    [Fact]
    public async Task PostOrdersSync_DuplicateOrders_ReturnsZeroNewOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order
            {
                OrderId = "ORD-1001",
                Sku = "SKU-001",
                Price = 1299.99m,
                Quantity = 1,
                CreatedAt = DateTime.Parse("2024-05-01T10:15:00Z"),
                BrandName = "Spring Sale"
            }
        };
    
        // Сначала отправляем заказ
        await _client.PostAsJsonAsync("/orders/sync", orders);
    
        // Act: Повторная отправка
        var response = await _client.PostAsJsonAsync("/orders/sync", orders);
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseContent = await response.Content.ReadFromJsonAsync<JsonElement>();
        var newOrdersCount = responseContent.GetProperty("newOrdersCount").GetInt32();
        Assert.Equal(0, newOrdersCount);
    }

    [Fact]
    public async Task PostOrdersSync_EmptyList_ReturnsBadRequest()
    {
        // Arrange
        var orders = new List<Order>();

        // Act
        var response = await _client.PostAsJsonAsync("/orders/sync", orders);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var responseContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("Список заказов пуст или некорректен", responseContent);
    }
}