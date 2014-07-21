using System;
using System.Collections;
using System.IO;

namespace Miracle.Arguments
{
    /// <summary>
    /// Base interface used for common access to generic implementations.
    /// </summary>
    public interface ICommandLineParser
    {
        void Parse(IEnumerator enumerator);
        object ParseCommand(IEnumerator enumerator, out bool more);
        void GenerateCommandHelp(TextWriter writer, string command);
        void GenerateCommandHelp(TextWriter writer);
        Tuple<string[], ICommandLineParser> FindCommandCommandLineParser(string command);
    }
}