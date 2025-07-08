namespace SmallService.API.Options;

public sealed class CircuitBreakerResponseOptions
{
    public double FailureThreshold { get; set; }
    public double SamplingDuration { get; set; }
    public int MinimumThroughput { get; set; }
    public int DurationOfBreak { get; set; }
}