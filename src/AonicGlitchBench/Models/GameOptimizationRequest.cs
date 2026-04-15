using System;

namespace AonicGlitchBench.Models;

public sealed class GameOptimizationRequest
{
    public required GameDefinition Game { get; init; }
    public OptimizationPreference Preference { get; init; } = OptimizationPreference.Balanced;
    public TimeSpan SamplingDuration { get; init; } = TimeSpan.FromSeconds(40);
    public TimeSpan SamplingInterval { get; init; } = TimeSpan.FromSeconds(1);
}
