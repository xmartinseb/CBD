using Cbd.Api.Data;
using Cbd.Api.Services;

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
        {
            logger.LogDebug("No new aggregated orders at {Time}", aggrOrdersCollection.AggregateTimeUtc);
        }
        else
        {
            await repository.AddAsync(aggrOrdersCollection, cancellationToken);
            logger.LogInformation("Stored aggregation: {Count} products at {Time}", aggrOrdersCollection.AggregatedOrders.Count, aggrOrdersCollection.AggregateTimeUtc);
        }
    }
}