using Cbd.Api.Models;
using Cbd.Api.Services;

namespace Cbd.Api.HostedServices;

public sealed class OrderAggregationTask(CreatedOrdersChannel createdOrders, AggregatedOrdersChannel aggregatedOrders)
    : BackgroundService
{
    static readonly TimeSpan Interval = TimeSpan.FromSeconds(20);

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        DateTime lastRunUtc = default;

        while (true)
        {
            await DelayAsync(lastRunUtc, cancellationToken);
            lastRunUtc = DateTime.UtcNow;
            var orders = await GetOrders(lastRunUtc, cancellationToken);
            var aggrOrders = orders.GroupBy(ord => ord.ProductId).Select(ordGroup => new AggregatedOrder(ordGroup.Key, ordGroup.Sum(ord => ord.Quantity))).ToList();
            await aggregatedOrders.EnqueueOrderAsync(new AggregatedOrdersCollection(aggrOrders, DateTime.UtcNow), cancellationToken);
        }
    }

    async Task<IReadOnlyList<Order>> GetOrders(DateTime lastRunUtc, CancellationToken cancellationToken)
    {
        var orders = new List<Order>();
        var readUntil = lastRunUtc + Interval;

        while (createdOrders.TryPeek(out var nextOrder) && nextOrder.CreatedUtc < readUntil)
        {
            orders.Add(nextOrder.Order);
            createdOrders.TryPop(out _);
        }
        
        return orders;
    }

    static async Task DelayAsync(DateTime lastRunUtc, CancellationToken cancellationToken)
    {
        var delta = DateTime.UtcNow - lastRunUtc;
        if (delta < Interval)
            await Task.Delay(Interval - delta, cancellationToken);
    }
}