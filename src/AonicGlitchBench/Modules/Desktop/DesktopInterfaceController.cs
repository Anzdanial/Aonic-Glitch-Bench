using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AonicGlitchBench.Contracts;
using AonicGlitchBench.Models;
using AonicGlitchBench.Modules.Games;

namespace AonicGlitchBench.Modules.Desktop;

public sealed class DesktopInterfaceController
{
    private readonly GameManager _gameManager;
    private readonly IDebugConsole _debugConsole;
    private readonly List<GameDefinition> _games = [];

    public DesktopInterfaceController(GameManager gameManager, IDebugConsole debugConsole)
    {
        _gameManager = gameManager;
        _debugConsole = debugConsole;
    }

    public IReadOnlyList<GameDefinition> GetSupportedGames() => _games.ToArray();

    public void RegisterGame(GameDefinition game)
    {
        _games.Add(game);
        _debugConsole.Write(DebugLogLevel.Info, nameof(DesktopInterfaceController), $"Registered game '{game.DisplayName}'.");
    }

    public GameDefinition? FindGame(string gameId)
    {
        return _games.FirstOrDefault(game => game.Id == gameId);
    }

    public Task<GameOptimizationResult> StartOptimizationAsync(string gameId, OptimizationPreference preference, CancellationToken cancellationToken)
    {
        var game = FindGame(gameId) ?? throw new KeyNotFoundException($"Game '{gameId}' is not registered.");

        var request = new GameOptimizationRequest
        {
            Game = game,
            Preference = preference
        };

        _debugConsole.Write(DebugLogLevel.Info, nameof(DesktopInterfaceController), $"Starting optimization for '{game.DisplayName}' with preference '{preference}'.");
        return _gameManager.OptimizeAsync(request, cancellationToken);
    }
}
