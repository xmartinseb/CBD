using Cbd.Api.Models;
using System.Collections.Concurrent;

namespace Cbd.Api.Data;

public interface IAggregatedOrdersRepository
{
    Task AddAsync(AggregatedOrdersCollection collection, CancellationToken cancellationToken);
    Task<IReadOnlyList<AggregatedOrdersCollection>> GetAll(CancellationToken cancellationToken);
}

/// <summary>
/// Thread safe in-memory repository agregovaných objednávek, pro ilustraci a testování
/// </summary>
public sealed class InMemoryAggregatedOrdersRepository : IAggregatedOrdersRepository
{
    readonly ConcurrentBag<AggregatedOrdersCollection> _collections = [];

    public Task AddAsync(AggregatedOrdersCollection collection, CancellationToken cancellationToken)
    {
        _collections.Add(collection);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AggregatedOrdersCollection>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<AggregatedOrdersCollection> snapshot = [.. _collections];
        return Task.FromResult(snapshot);
    }
}
