using Cbd.Api.Data;
using Cbd.Api.Services;
using Cbd.Api.Tools;

namespace Cbd.Api.HostedServices;

public sealed class AggregatedOrdersInternalTask(
    AggregatedOrdersChannel aggregatedOrders,
    IAggregatedOrdersRepository repository,
    ILogger<AggregatedOrdersInternalTask> logger)
    : PeriodicTaskBase(logger)
{
    protected override TimeSpan Period { get; } = TimeSpan.FromSeconds(20);

    protected override async Task MainAsync(CancellationToken cancellationToken)
    {
        var aggrOrdersCollection = await aggregatedOrders.DequeueOrderAsync(cancellationToken);

        if (aggrOrdersCollection.AggregatedOrders.Count == 0)
            Console.WriteLine($"No new aggregated orders at {aggrOrdersCollection.AggregateTimeUtc}");
        else
        {
            Console.WriteLine(aggrOrdersCollection.ToJson());
            await repository.AddAsync(aggrOrdersCollection, cancellationToken);
        }
    }
}