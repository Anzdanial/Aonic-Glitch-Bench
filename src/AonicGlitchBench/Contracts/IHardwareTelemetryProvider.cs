using System.Threading;
using System.Threading.Tasks;
using AonicGlitchBench.Models;

namespace AonicGlitchBench.Contracts;

public interface IHardwareTelemetryProvider
{
    Task<HardwareMetrics> CaptureAsync(CancellationToken cancellationToken);
}
