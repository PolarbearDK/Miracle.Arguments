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
        /// <summary>
        /// Parse command line
        /// </summary>
        /// <param name="enumerator">Args enumerator</param>
        void Parse(IEnumerator enumerator);

        /// <summary>
        /// Parse command
        /// </summary>
        /// <param name="enumerator">Args enumerator</param>
        /// <param name="more">Output parameter that indicates if there are more arguments to process</param>
        object ParseCommand(IEnumerator enumerator, out bool more);

        /// <summary>
        /// Generate help for command
        /// </summary>
        /// <param name="writer">output to this writer</param>
        /// <param name="command">command to find</param>
        void GenerateCommandHelp(TextWriter writer, string command);

        /// <summary>
        /// Generate help for command
        /// </summary>
        /// <param name="writer">output to this writer</param>
        /// <param name="names">command names+aliases</param>
        void GenerateCommandHelp(TextWriter writer, string[] names);

        /// <summary>
        /// Find command by name in current parser or in any sub parsers.
        /// </summary>
        /// <param name="command">Name of command to find</param>
        /// <returns>Command match containg command parser and array of names associated with parser, or null if not found</returns>
        CommandMatch FindCommandCommandLineParser(string command);
    }
}