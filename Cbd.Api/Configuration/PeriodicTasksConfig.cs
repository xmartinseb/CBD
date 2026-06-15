namespace Cbd.Api.Configuration;

public sealed class PeriodicTasksConfig
{
    public TimeSpan OrderAggregationPeriod { get; set; } = TimeSpan.FromSeconds(20);
    public TimeSpan AggregatedOrdersProcessingPeriod { get; set; } = TimeSpan.FromSeconds(20);
}
