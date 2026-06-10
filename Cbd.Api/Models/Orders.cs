namespace Cbd.Api.Models;

public readonly record struct Order(string ProductId, int Quantity);
public readonly record struct OrderCreated(Order Order, DateTime CreatedUtc);
public record struct AggregatedOrder(string ProductId, int Quantity);
public readonly record struct AggregatedOrdersCollection(IReadOnlyList<AggregatedOrder> AggregatedOrders, DateTime AggregateTime);