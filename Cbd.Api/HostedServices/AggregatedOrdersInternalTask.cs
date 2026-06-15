using Cbd.Api.Data;
using Cbd.Api.Models;
using Cbd.Api.Services;

namespace Cbd.Api.HostedServices;

/// <summary>
/// Asynchronně zpracovává zagregované objednávky — čeká na nové položky v kanálu a ukládá je.
/// Neni to periodicka úloha, dequeue probíhá hned jak data dorazí.
/// </summary>
public sealed class AggregatedOrdersInternalTask(
    AppSingletonChannel<AggregatedOrdersCollection> aggregatedOrders,
    IAggregatedOrdersRepository repository,
    ILogger<AggregatedOrdersInternalTask> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
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
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in task {Task}", GetType().Name);
            }
        }
    }
}