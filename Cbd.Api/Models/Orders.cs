namespace Cbd.Api.Models;

public readonly record struct Order(string ProductId, int Quantity);

public record struct AggregatedOrder(string ProductId, int Quantity);