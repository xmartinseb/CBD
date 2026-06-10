using Cbd.Api.Models;
using System.Threading.Channels;

namespace Cbd.Api.Services;


public class CreatedOrdersChannel
{
    private Channel<OrderCreated> createdOrdersQueue;

    public CreatedOrdersChannel()
    {
        createdOrdersQueue = Channel.CreateUnbounded<OrderCreated>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public ValueTask EnqueueOrderAsync(OrderCreated o, CancellationToken cancellationToken)
        => createdOrdersQueue.Writer.WriteAsync(o, cancellationToken);

    public bool TryPeek(out OrderCreated o)
        => createdOrdersQueue.Reader.TryPeek(out o);

    public bool TryPop(out OrderCreated o)
        => createdOrdersQueue.Reader.TryRead(out o);
}