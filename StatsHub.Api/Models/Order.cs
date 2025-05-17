namespace StatsHub.Api.Models;

public class Order
{
    public string OrderId { get; set; } = null!;
    public string Sku { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public string BrandName { get; set; } = null!;
}