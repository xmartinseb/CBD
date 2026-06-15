using System.Threading.Channels;

namespace Cbd.Api.Services;

/// <summary>
/// Asynchronní kanál pro předávání zpráv mezi částmi aplikace.
/// Odděluje logiku produkce a konzumace dat — producent a konzument fungují nezávisle.
/// Musí být registrován jako singleton.
/// </summary>
public sealed class AppSingletonChannel<T>
{
    readonly Channel<T> _queue;

    public AppSingletonChannel()
    {
        _queue = Channel.CreateUnbounded<T>(new UnboundedChannelOptions
        {
            SingleReader = true, // Zpracování dat probíhá v jedné hosted service
            SingleWriter = false, // Může být více http requestů najednou, které generují nová data
        });
    }

    public ValueTask EnqueueAsync(T item, CancellationToken cancellationToken)
        => _queue.Writer.WriteAsync(item, cancellationToken);

    public bool TryDequeue(out T item)
        => _queue.Reader.TryRead(out item!);

    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken)
        => _queue.Reader.ReadAsync(cancellationToken);
}
