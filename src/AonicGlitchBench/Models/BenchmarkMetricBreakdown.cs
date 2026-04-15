namespace AonicGlitchBench.Models;

public sealed class BenchmarkMetricBreakdown
{
    public required string Name { get; init; }
    public double RawValue { get; init; }
    public double NormalizedScore { get; init; }
    public double Weight { get; init; }
    public double WeightedScore => NormalizedScore * Weight;
}
