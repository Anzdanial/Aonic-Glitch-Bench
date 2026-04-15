using System.Threading;
using System.Threading.Tasks;
using AonicGlitchBench.Models;

namespace AonicGlitchBench.Contracts;

public interface IGameLauncher
{
    Task<GameLaunchResult> LaunchAsync(GameDefinition game, OptimizationProfile profile, CancellationToken cancellationToken);
}
