using Cbd.Api.Models;
using System.Collections.Concurrent;

namespace Cbd.Api.Data;

public interface IOrdersRepository
{
    Task AddAsync(Order o, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken);
}

/// <summary>
/// Thread safe in-memory repository, pro ilustraci a testování
/// </summary>
public sealed class InMemoryOrdersRepository : IOrdersRepository
{
    readonly ConcurrentQueue<Order> _orders = [];

    public Task AddAsync(Order o, CancellationToken cancellationToken)
    {
        _orders.Enqueue(o);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken)
    {
        IReadOnlyList<Order> snapshot = [.. _orders];
        return Task.FromResult(snapshot);
    }
}
  
/// <summary>
/// Není implementováno, pouze pro ilustraci
/// </summary>
public sealed class SqlOrdersRepository : IOrdersRepository
{
    public Task AddAsync(Order o, CancellationToken cancellationToken) => throw new NotImplementedException();
    public Task<IReadOnlyList<Order>> GetAll(CancellationToken cancellationToken) => throw new NotImplementedException();
}