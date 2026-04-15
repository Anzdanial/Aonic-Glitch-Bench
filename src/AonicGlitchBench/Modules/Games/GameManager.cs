using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AonicGlitchBench.Contracts;
using AonicGlitchBench.Models;
using AonicGlitchBench.Modules.Benchmarking;
using AonicGlitchBench.Modules.Configuration;
using AonicGlitchBench.Modules.Logging;

namespace AonicGlitchBench.Modules.Games;

public sealed class GameManager
{
    private readonly ConfigurationManager _configurationManager;
    private readonly HardwareLogger _hardwareLogger;
    private readonly BenchmarkScorer _benchmarkScorer;
    private readonly IGameLauncher _gameLauncher;
    private readonly IDebugConsole _debugConsole;

    public GameManager(
        ConfigurationManager configurationManager,
        HardwareLogger hardwareLogger,
        BenchmarkScorer benchmarkScorer,
        IGameLauncher gameLauncher,
        IDebugConsole debugConsole)
    {
        _configurationManager = configurationManager;
        _hardwareLogger = hardwareLogger;
        _benchmarkScorer = benchmarkScorer;
        _gameLauncher = gameLauncher;
        _debugConsole = debugConsole;
    }

    public async Task<GameOptimizationResult> OptimizeAsync(GameOptimizationRequest request, CancellationToken cancellationToken)
    {
        _debugConsole.Write(DebugLogLevel.Info, nameof(GameManager), $"Capturing baseline metrics for '{request.Game.DisplayName}'.");
        var baselineMetrics = await _hardwareLogger.CaptureSingleAsync(cancellationToken);
        var profile = _configurationManager.GenerateProfile(request.Game, baselineMetrics, request.Preference);
        _configurationManager.ApplyProfile(request.Game, profile);

        _debugConsole.Write(DebugLogLevel.Info, nameof(GameManager), $"Launching '{request.Game.DisplayName}' with generated profile.");
        var launchResult = await _gameLauncher.LaunchAsync(request.Game, profile, cancellationToken);
        if (!launchResult.Success)
        {
            _debugConsole.Write(DebugLogLevel.Error, nameof(GameManager), launchResult.Message);
            throw new InvalidOperationException(launchResult.Message);
        }

        var snapshots = await _hardwareLogger.CaptureSeriesAsync(
            request.SamplingDuration,
            request.SamplingInterval,
            cancellationToken);

        var aggregateMetrics = AggregateMetrics(snapshots);
        var score = _benchmarkScorer.Score(aggregateMetrics);

        _debugConsole.Write(
            DebugLogLevel.Info,
            nameof(GameManager),
            $"Completed optimization for '{request.Game.DisplayName}' with score {score.TotalScore} ({score.Tier}).");

        return new GameOptimizationResult
        {
            Game = request.Game,
            AppliedProfile = profile,
            BenchmarkScore = score,
            CapturedSnapshots = snapshots,
            Summary = $"Optimization complete. Runtime: {launchResult.Runtime.TotalSeconds:0}s. Score: {score.TotalScore} ({score.Tier})."
        };
    }

    private static HardwareMetrics AggregateMetrics(IReadOnlyList<PerformanceSnapshot> snapshots)
    {
        if (snapshots.Count == 0)
        {
            return new HardwareMetrics();
        }

        return new HardwareMetrics
        {
            AverageFps = snapshots.Average(snapshot => snapshot.Metrics.AverageFps),
            OnePercentLowFps = snapshots.Average(snapshot => snapshot.Metrics.OnePercentLowFps),
            AverageFrameTimeMs = snapshots.Average(snapshot => snapshot.Metrics.AverageFrameTimeMs),
            CpuUsagePercent = snapshots.Average(snapshot => snapshot.Metrics.CpuUsagePercent),
            CpuTemperatureCelsius = snapshots.Max(snapshot => snapshot.Metrics.CpuTemperatureCelsius),
            GpuUsagePercent = snapshots.Average(snapshot => snapshot.Metrics.GpuUsagePercent),
            GpuTemperatureCelsius = snapshots.Max(snapshot => snapshot.Metrics.GpuTemperatureCelsius),
            VramUsagePercent = snapshots.Average(snapshot => snapshot.Metrics.VramUsagePercent),
            RamUsagePercent = snapshots.Average(snapshot => snapshot.Metrics.RamUsagePercent),
            PowerConsumptionWatts = snapshots.Average(snapshot => snapshot.Metrics.PowerConsumptionWatts),
            InputLatencyMs = snapshots.Average(snapshot => snapshot.Metrics.InputLatencyMs)
        };
    }
}
