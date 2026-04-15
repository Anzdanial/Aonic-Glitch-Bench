namespace AonicGlitchBench.Models;

public sealed class HardwareMetrics
{
    public double AverageFps { get; init; }
    public double OnePercentLowFps { get; init; }
    public double AverageFrameTimeMs { get; init; }
    public double CpuUsagePercent { get; init; }
    public double CpuTemperatureCelsius { get; init; }
    public double GpuUsagePercent { get; init; }
    public double GpuTemperatureCelsius { get; init; }
    public double VramUsagePercent { get; init; }
    public double RamUsagePercent { get; init; }
    public double PowerConsumptionWatts { get; init; }
    public double InputLatencyMs { get; init; }
}
