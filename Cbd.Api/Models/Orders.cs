namespace Cbd.Api.Models;

public sealed record Order(string ProductId, int Quantity);
public readonly record struct OrderCreated(Order Order, DateTime CreatedUtc);
public sealed record AggregatedOrder(string ProductId, int Quantity);
public readonly record struct AggregatedOrdersCollection(IReadOnlyList<AggregatedOrder> AggregatedOrders, DateTime AggregateTime);