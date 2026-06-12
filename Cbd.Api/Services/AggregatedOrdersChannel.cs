using Cbd.Api.Models;
using System.Threading.Channels;

namespace Cbd.Api.Services;

/// <summary>
/// Asynchronní kanál pro předávání informací o agregovaných objednávkách.
/// Tento způsob práce odděluje různé logiky v aplikaci, takže mohou fungovat nezávisle na sobě.
/// Toto je potřeba registrovat jako singleton
/// </summary>
public sealed class AggregatedOrdersChannel
{
    readonly Channel<AggregatedOrdersCollection> aggregatedOrdersQueue;

    public AggregatedOrdersChannel()
    {
        aggregatedOrdersQueue = Channel.CreateUnbounded<AggregatedOrdersCollection>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public ValueTask EnqueueOrderAsync(AggregatedOrdersCollection o, CancellationToken cancellationToken)
        => aggregatedOrdersQueue.Writer.WriteAsync(o, cancellationToken);

    public ValueTask<AggregatedOrdersCollection> DequeueOrderAsync(CancellationToken cancellationToken)
        => aggregatedOrdersQueue.Reader.ReadAsync(cancellationToken);
}