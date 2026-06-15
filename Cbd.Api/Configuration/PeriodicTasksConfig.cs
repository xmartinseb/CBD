namespace Cbd.Api.Configuration;

public sealed class PeriodicTasksConfig
{
    public TimeSpan OrderAggregationPeriod { get; set; } = TimeSpan.FromSeconds(20);
}
