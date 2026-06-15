using Cbd.Api.Configuration;
using Cbd.Api.Data;
using Cbd.Api.Models;
using Cbd.Api.Services;
using Microsoft.Extensions.Options;

namespace Cbd.Api.HostedServices;

public sealed class AggregatedOrdersInternalTask(
    AppChannel<AggregatedOrdersCollection> aggregatedOrders,
    IAggregatedOrdersRepository repository,
    IOptions<PeriodicTasksConfig> config,
    ILogger<AggregatedOrdersInternalTask> logger)
    : PeriodicTaskBase(logger)
{
    protected override TimeSpan Period { get; } = config.Value.AggregatedOrdersProcessingPeriod;

    protected override async Task MainAsync(CancellationToken cancellationToken)
    {
        var aggrOrdersCollection = await aggregatedOrders.DequeueAsync(cancellationToken);

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