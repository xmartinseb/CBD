using System.Threading.Channels;

namespace Cbd.Api.Services;

/// <summary>
/// Asynchronní kanál pro předávání zpráv mezi částmi aplikace.
/// Odděluje logiku produkce a konzumace dat — producent a konzument fungují nezávisle.
/// Musí být registrován jako singleton.
/// </summary>
public sealed class AppChannel<T>
{
    readonly Channel<T> _queue;

    public AppChannel()
    {
        _queue = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
        });
    }

    public ValueTask EnqueueAsync(T item, CancellationToken cancellationToken)
        => _queue.Writer.WriteAsync(item, cancellationToken);

    public bool TryDequeue(out T item)
        => _queue.Reader.TryRead(out item!);

    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken)
        => _queue.Reader.ReadAsync(cancellationToken);
}
