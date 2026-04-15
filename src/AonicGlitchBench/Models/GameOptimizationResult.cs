using System.Collections.Generic;

namespace AonicGlitchBench.Models;

public sealed class GameOptimizationResult
{
    public required GameDefinition Game { get; init; }
    public required OptimizationProfile AppliedProfile { get; init; }
    public required BenchmarkScore BenchmarkScore { get; init; }
    public required IReadOnlyList<PerformanceSnapshot> CapturedSnapshots { get; init; }
    public string Summary { get; init; } = string.Empty;
}
