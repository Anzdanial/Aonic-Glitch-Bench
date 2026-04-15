using System;

namespace AonicGlitchBench.Models;

public sealed class PerformanceSnapshot
{
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;
    public required HardwareMetrics Metrics { get; init; }
}
