using System;
using System.Collections.Generic;

namespace AonicGlitchBench.Models;

public sealed class OptimizationProfile
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string GameId { get; init; } = string.Empty;
    public OptimizationPreference Preference { get; init; } = OptimizationPreference.Balanced;
    public string ProfileName { get; init; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public IReadOnlyDictionary<string, string> Settings { get; init; } = new Dictionary<string, string>();
    public string Notes { get; init; } = string.Empty;
}
