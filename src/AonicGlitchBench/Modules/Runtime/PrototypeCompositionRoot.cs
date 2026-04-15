using AonicGlitchBench.Contracts;
using AonicGlitchBench.Modules.Benchmarking;
using AonicGlitchBench.Modules.Configuration;
using AonicGlitchBench.Modules.Debugging;
using AonicGlitchBench.Modules.Desktop;
using AonicGlitchBench.Modules.Games;
using AonicGlitchBench.Modules.Logging;

namespace AonicGlitchBench.Modules.Runtime;

public sealed class PrototypeCompositionRoot
{
    public PrototypeCompositionRoot(IHardwareTelemetryProvider telemetryProvider, IGameLauncher gameLauncher)
    {
        DebugConsole = new DebugConsole();
        ConfigurationManager = new ConfigurationManager(DebugConsole);
        HardwareLogger = new HardwareLogger(telemetryProvider, DebugConsole);
        BenchmarkScorer = new BenchmarkScorer();
        GameManager = new GameManager(ConfigurationManager, HardwareLogger, BenchmarkScorer, gameLauncher, DebugConsole);
        DesktopInterface = new DesktopInterfaceController(GameManager, DebugConsole);
    }

    public IDebugConsole DebugConsole { get; }
    public ConfigurationManager ConfigurationManager { get; }
    public HardwareLogger HardwareLogger { get; }
    public BenchmarkScorer BenchmarkScorer { get; }
    public GameManager GameManager { get; }
    public DesktopInterfaceController DesktopInterface { get; }
}
