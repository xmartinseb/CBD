using Cbd.Api.Models;
using System.Threading.Channels;

namespace Cbd.Api.Services;


public class AggregatedOrdersChannel
{
    private Channel<AggregatedOrdersCollection> aggregatedOrdersQueue;

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