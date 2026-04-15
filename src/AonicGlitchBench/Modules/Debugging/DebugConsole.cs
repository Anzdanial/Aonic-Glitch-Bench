using System.Collections.Generic;
using System.Linq;
using AonicGlitchBench.Contracts;
using AonicGlitchBench.Models;

namespace AonicGlitchBench.Modules.Debugging;

public sealed class DebugConsole : IDebugConsole
{
    private readonly List<DebugLogEntry> _entries = [];
    private readonly object _gate = new();

    public void Write(DebugLogLevel level, string source, string message)
    {
        lock (_gate)
        {
            _entries.Add(new DebugLogEntry
            {
                Level = level,
                Source = source,
                Message = message
            });
        }
    }

    public IReadOnlyList<DebugLogEntry> GetEntries()
    {
        lock (_gate)
        {
            return _entries.ToArray();
        }
    }

    public void Clear()
    {
        lock (_gate)
        {
            _entries.Clear();
        }
    }
}
