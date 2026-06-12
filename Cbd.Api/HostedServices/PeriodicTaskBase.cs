namespace Cbd.Api.HostedServices;

/// <summary>
/// Zaobaluje background service do elegantní periodické úlohy, 
/// která se spouští v pravidelných intervalech a zpracovává nezachycené výjimky, aby nedošlo k pádu celé úlohy
/// </summary>
public abstract class PeriodicTaskBase(ILogger logger) : BackgroundService
{
    protected abstract TimeSpan Period { get; }
    protected abstract Task MainAsync(CancellationToken cancellationToken);
    protected DateTime LastRunUtc { get; private set; } = default;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                await DelayAsync(LastRunUtc, cancellationToken);
                LastRunUtc = DateTime.UtcNow;
                logger.LogDebug("Starting periodic task {Task}", GetType().Name);
                await MainAsync(cancellationToken);
            }
            catch (TaskCanceledException) { throw; }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in periodic task {Task}", GetType().Name);
            }
        }
    }

    async Task DelayAsync(DateTime lastRunUtc, CancellationToken cancellationToken)
    {
        var sinceLastRun = DateTime.UtcNow - lastRunUtc;
        if (sinceLastRun < Period)
            await Task.Delay(Period - sinceLastRun, cancellationToken);
    }
}