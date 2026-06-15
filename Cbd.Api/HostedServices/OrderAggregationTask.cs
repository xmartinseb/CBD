using Cbd.Api.Configuration;
using Cbd.Api.Models;
using Cbd.Api.Services;
using Microsoft.Extensions.Options;

namespace Cbd.Api.HostedServices;

public sealed class OrderAggregationTask(
    AppSingletonChannel<OrderCreated> createdOrders,
    AppSingletonChannel<AggregatedOrdersCollection> aggregatedOrders,
    IOptions<PeriodicTasksConfig> config,
    ILogger<OrderAggregationTask> logger)
    : PeriodicTaskBase(logger)
{
    protected override TimeSpan Period { get; } = config.Value.OrderAggregationPeriod;

    protected override async Task MainAsync(CancellationToken cancellationToken)
    {
        var orders = GetOrders();
        var aggrOrders = orders.GroupBy(ord => ord.ProductId).Select(ordGroup => new AggregatedOrder(ordGroup.Key, ordGroup.Sum(ord => ord.Quantity))).ToList();
        await aggregatedOrders.EnqueueAsync(new AggregatedOrdersCollection(aggrOrders, DateTime.UtcNow), cancellationToken);
    }

    List<Order> GetOrders()
    {
        var orders = new List<Order>();
        // Je potřeba tu agregaci někde zastavit (ale není třeba přesný deadline). Jde jen o to, aby tento task kontinuálně produkoval nějaké výsledky, i když
        // by byl zahlcen přísunem mnoha objednávek každou sekundu.
        var readUntilUtc = LastRunUtc + Period;
        
        while (createdOrders.TryDequeue(out var nextOrder))
        {
            orders.Add(nextOrder.Order);
            if (nextOrder.CreatedUtc >= readUntilUtc)
                break;
        }

        return orders;
    }
}