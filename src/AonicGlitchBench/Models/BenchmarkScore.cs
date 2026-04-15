using System;
using System.Collections.Generic;
using System.Linq;

namespace AonicGlitchBench.Models;

public sealed class BenchmarkScore
{
    public required string Tier { get; init; }
    public double TotalScore { get; init; }
    public required IReadOnlyList<BenchmarkMetricBreakdown> Breakdown { get; init; }

    public static string ResolveTier(double score)
    {
        if (score >= 90d)
        {
            return "Excellent";
        }

        if (score >= 70d)
        {
            return "Good";
        }

        if (score >= 50d)
        {
            return "Average";
        }

        return "Poor";
    }

    public static BenchmarkScore FromBreakdown(IEnumerable<BenchmarkMetricBreakdown> breakdown)
    {
        var items = breakdown.ToArray();
        var total = Math.Round(items.Sum(item => item.WeightedScore), 2, MidpointRounding.AwayFromZero);

        return new BenchmarkScore
        {
            TotalScore = total,
            Tier = ResolveTier(total),
            Breakdown = items
        };
    }
}
