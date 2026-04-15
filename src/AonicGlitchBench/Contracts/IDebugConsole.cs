using System.Collections.Generic;
using AonicGlitchBench.Models;

namespace AonicGlitchBench.Contracts;

public interface IDebugConsole
{
    void Write(DebugLogLevel level, string source, string message);
    IReadOnlyList<DebugLogEntry> GetEntries();
    void Clear();
}
