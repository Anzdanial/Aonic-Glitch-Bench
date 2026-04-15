using System;

namespace AonicGlitchBench.Models;

public sealed class GameLaunchResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public TimeSpan Runtime { get; init; }
}
