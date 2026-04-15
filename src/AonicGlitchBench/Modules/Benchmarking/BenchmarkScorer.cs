using System;
using System.Collections.Generic;
using AonicGlitchBench.Models;

namespace AonicGlitchBench.Modules.Benchmarking;

public sealed class BenchmarkScorer
{
    public BenchmarkScore Score(HardwareMetrics metrics)
    {
        var breakdown = new List<BenchmarkMetricBreakdown>
        {
            CreateHigherIsBetter("Average FPS", metrics.AverageFps, 60d, 40d),
            CreateHigherIsBetter("1% Low FPS", metrics.OnePercentLowFps, 45d, 15d),
            CreateLowerIsBetter("Frame Time", metrics.AverageFrameTimeMs, 16.7d, 15d),
            CreateLowerIsBetter("CPU Usage", metrics.CpuUsagePercent, 85d, 7.5d),
            CreateLowerIsBetter("GPU Usage", metrics.GpuUsagePercent, 90d, 7.5d),
            CreateLowerIsBetter("CPU Temperature", metrics.CpuTemperatureCelsius, 85d, 5d),
            CreateLowerIsBetter("GPU Temperature", metrics.GpuTemperatureCelsius, 85d, 5d),
            CreateLowerIsBetter("VRAM Usage", metrics.VramUsagePercent, 80d, 2.5d),
            CreateLowerIsBetter("Power Consumption", metrics.PowerConsumptionWatts, 350d, 1.25d),
            CreateLowerIsBetter("Input Latency", metrics.InputLatencyMs, 20d, 1.25d)
        };

        return BenchmarkScore.FromBreakdown(breakdown);
    }

    private static BenchmarkMetricBreakdown CreateHigherIsBetter(string name, double rawValue, double target, double weightPercentage)
    {
        var normalized = Math.Clamp(rawValue / target * 100d, 0d, 100d);
        return CreateBreakdown(name, rawValue, normalized, weightPercentage);
    }

    private static BenchmarkMetricBreakdown CreateLowerIsBetter(string name, double rawValue, double threshold, double weightPercentage)
    {
        if (rawValue <= 0d)
        {
            return CreateBreakdown(name, rawValue, 100d, weightPercentage);
        }

        var ratio = threshold / rawValue;
        var normalized = Math.Clamp(ratio * 100d, 0d, 100d);
        return CreateBreakdown(name, rawValue, normalized, weightPercentage);
    }

    private static BenchmarkMetricBreakdown CreateBreakdown(string name, double rawValue, double normalizedScore, double weightPercentage)
    {
        return new BenchmarkMetricBreakdown
        {
            Name = name,
            RawValue = Math.Round(rawValue, 2, MidpointRounding.AwayFromZero),
            NormalizedScore = Math.Round(normalizedScore, 2, MidpointRounding.AwayFromZero),
            Weight = weightPercentage / 100d
        };
    }
}
