using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AonicGlitchBench.Contracts;
using AonicGlitchBench.Models;

namespace AonicGlitchBench.Modules.Logging;

public sealed class HardwareLogger
{
    private readonly IHardwareTelemetryProvider _telemetryProvider;
    private readonly IDebugConsole _debugConsole;

    public HardwareLogger(IHardwareTelemetryProvider telemetryProvider, IDebugConsole debugConsole)
    {
        _telemetryProvider = telemetryProvider;
        _debugConsole = debugConsole;
    }

    public async Task<HardwareMetrics> CaptureSingleAsync(CancellationToken cancellationToken)
    {
        var metrics = await _telemetryProvider.CaptureAsync(cancellationToken);
        _debugConsole.Write(
            DebugLogLevel.Trace,
            nameof(HardwareLogger),
            $"Captured snapshot: FPS={metrics.AverageFps:0.##}, CPU={metrics.CpuUsagePercent:0.##}%, GPU={metrics.GpuUsagePercent:0.##}%.");

        return metrics;
    }

    public async Task<IReadOnlyList<PerformanceSnapshot>> CaptureSeriesAsync(
        TimeSpan duration,
        TimeSpan interval,
        CancellationToken cancellationToken)
    {
        var snapshots = new List<PerformanceSnapshot>();
        var startedAt = DateTimeOffset.UtcNow;
        _debugConsole.Write(DebugLogLevel.Info, nameof(HardwareLogger), $"Started logging for {duration.TotalSeconds:0}s.");

        while (DateTimeOffset.UtcNow - startedAt < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            snapshots.Add(new PerformanceSnapshot
            {
                Metrics = await CaptureSingleAsync(cancellationToken)
            });

            await Task.Delay(interval, cancellationToken);
        }

        _debugConsole.Write(DebugLogLevel.Info, nameof(HardwareLogger), $"Completed logging with {snapshots.Count} snapshots.");
        return snapshots;
    }
}
