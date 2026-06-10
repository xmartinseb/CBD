using Cbd.Api.Services;
using Cbd.Api.Tools;

namespace Cbd.Api.HostedServices;

public sealed class AggregatedOrdersInternalTask(AggregatedOrdersChannel aggregatedOrders)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var aggrOrdersCollection = await aggregatedOrders.DequeueOrderAsync(cancellationToken);
            if (aggrOrdersCollection.AggregatedOrders.Count == 0)
                Console.WriteLine($"No aggregated orders at {aggrOrdersCollection.AggregateTime}");
            else
                Console.WriteLine(aggrOrdersCollection.ToJson());

            await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }
    }
}