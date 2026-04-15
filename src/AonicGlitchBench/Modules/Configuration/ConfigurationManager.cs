using System;
using System.Collections.Generic;
using System.IO;
using AonicGlitchBench.Contracts;
using AonicGlitchBench.Models;
using AonicGlitchBench.Utilities;

namespace AonicGlitchBench.Modules.Configuration;

public sealed class ConfigurationManager
{
    private readonly IDebugConsole _debugConsole;

    public ConfigurationManager(IDebugConsole debugConsole)
    {
        _debugConsole = debugConsole;
    }

    public OptimizationProfile GenerateProfile(GameDefinition game, HardwareMetrics metrics, OptimizationPreference preference)
    {
        var settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["resolution_scale"] = ResolveResolutionScale(metrics, preference),
            ["texture_quality"] = ResolveTextureQuality(metrics, preference),
            ["shadow_quality"] = ResolveShadowQuality(metrics, preference),
            ["anti_aliasing"] = ResolveAntiAliasing(metrics, preference),
            ["frame_limit"] = ResolveFrameLimit(metrics, preference),
            ["ray_tracing"] = ResolveRayTracing(metrics, preference)
        };

        var profile = new OptimizationProfile
        {
            GameId = game.Id,
            Preference = preference,
            ProfileName = $"{game.DisplayName} - {preference}",
            Settings = settings,
            Notes = "Auto-generated from current hardware telemetry."
        };

        _debugConsole.Write(DebugLogLevel.Info, nameof(ConfigurationManager), $"Generated profile '{profile.ProfileName}' for {game.DisplayName}.");
        return profile;
    }

    public void SaveProfile(string path, OptimizationProfile profile)
    {
        JsonFileStorage.Write(path, profile);
        _debugConsole.Write(DebugLogLevel.Info, nameof(ConfigurationManager), $"Saved profile to {path}.");
    }

    public OptimizationProfile? LoadProfile(string path)
    {
        var profile = JsonFileStorage.Read<OptimizationProfile>(path);
        _debugConsole.Write(DebugLogLevel.Info, nameof(ConfigurationManager), profile is null
            ? $"No profile found at {path}."
            : $"Loaded profile '{profile.ProfileName}' from {path}.");
        return profile;
    }

    public void ApplyProfile(GameDefinition game, OptimizationProfile profile)
    {
        if (string.IsNullOrWhiteSpace(game.ConfigurationPath))
        {
            throw new InvalidOperationException("Game configuration path is required before applying a profile.");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(game.ConfigurationPath)!);
        File.WriteAllText(game.ConfigurationPath, RenderConfiguration(profile));
        _debugConsole.Write(DebugLogLevel.Info, nameof(ConfigurationManager), $"Applied profile '{profile.ProfileName}' to {game.ConfigurationPath}.");
    }

    private static string RenderConfiguration(OptimizationProfile profile)
    {
        var lines = new List<string>
        {
            "# Aonic Glitch Bench generated profile",
            $"# profile_id={profile.Id}",
            $"# preference={profile.Preference}"
        };

        foreach (var entry in profile.Settings)
        {
            lines.Add($"{entry.Key}={entry.Value}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string ResolveResolutionScale(HardwareMetrics metrics, OptimizationPreference preference)
    {
        return preference switch
        {
            OptimizationPreference.Performance when metrics.GpuUsagePercent > 85d => "85",
            OptimizationPreference.VisualQuality => "100",
            _ => metrics.GpuUsagePercent > 90d ? "90" : "100"
        };
    }

    private static string ResolveTextureQuality(HardwareMetrics metrics, OptimizationPreference preference)
    {
        if (preference == OptimizationPreference.VisualQuality && metrics.VramUsagePercent < 70d)
        {
            return "Ultra";
        }

        return metrics.VramUsagePercent > 85d ? "Medium" : "High";
    }

    private static string ResolveShadowQuality(HardwareMetrics metrics, OptimizationPreference preference)
    {
        if (preference == OptimizationPreference.Performance)
        {
            return "Medium";
        }

        return metrics.GpuTemperatureCelsius > 82d ? "Medium" : "High";
    }

    private static string ResolveAntiAliasing(HardwareMetrics metrics, OptimizationPreference preference)
    {
        if (preference == OptimizationPreference.Performance && metrics.AverageFps < 60d)
        {
            return "FXAA";
        }

        return preference == OptimizationPreference.VisualQuality ? "TAA" : "SMAA";
    }

    private static string ResolveFrameLimit(HardwareMetrics metrics, OptimizationPreference preference)
    {
        if (preference == OptimizationPreference.VisualQuality)
        {
            return "60";
        }

        return metrics.CpuTemperatureCelsius > 85d ? "60" : "120";
    }

    private static string ResolveRayTracing(HardwareMetrics metrics, OptimizationPreference preference)
    {
        return preference == OptimizationPreference.VisualQuality && metrics.GpuUsagePercent < 75d ? "On" : "Off";
    }
}
