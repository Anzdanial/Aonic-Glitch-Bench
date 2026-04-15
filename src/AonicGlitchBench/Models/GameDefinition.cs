using System.Collections.Generic;

namespace AonicGlitchBench.Models;

public sealed class GameDefinition
{
    public string Id { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public SupportedEngine Engine { get; init; }
    public string InstallationPath { get; init; } = string.Empty;
    public string ExecutablePath { get; init; } = string.Empty;
    public string ConfigurationPath { get; init; } = string.Empty;
    public IReadOnlyDictionary<string, string> Metadata { get; init; } = new Dictionary<string, string>();
}
