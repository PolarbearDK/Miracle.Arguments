using System;
using System.IO;

namespace Miracle.Arguments
{
    /// <summary>
    /// Command line Factory/Extension class
    /// </summary>
    public static class CommandLineParserExtensions
    {
        /// <summary>
        /// Really simple way to parse command line arguments into argument class.
        /// Help is automatically shown if a help argument target is set to true.
        /// </summary>
        /// <typeparam name="T">argument class type</typeparam>
        /// <param name="args">arguments to parse</param>
        /// <param name="output">TextWriter where normal output is written to</param>
        /// <param name="error">TextWriter where error output is written to</param>
        /// <returns>Parsed argument class or null in case of error</returns>
        public static T ParseCommandLine<T>(this string[] args, TextWriter output, TextWriter error)
            where T : class, new()
        {
            CommandLineParser<T> commandLineParser;

            try
            {
                commandLineParser = new CommandLineParser<T>();
            }
            catch (ArgumentDefinitionException ex)
            {
                error.WriteLine(ex.Message);
                return null;
            }

            try
            {
                commandLineParser.Parse(args);
                if (commandLineParser.IsHelp)
                {
                    commandLineParser.GenerateHelp(output);
                    return null;
                }

                commandLineParser.CheckRequired();

                var parameterClass = new T();
                commandLineParser.GetValues(parameterClass);
                return parameterClass;
            }
            catch (CommandLineParserException ex)
            {
                error.WriteLine(ex.Message);
                commandLineParser.AutoGenerateHelp(error);
                return null;
            }
        }

        /// <summary>
        /// Really simple way to parse command line arguments into argument class.
        /// Help is automatically shown if a help argument target is set to true.
        /// </summary>
        /// <typeparam name="T">argument class type</typeparam>
        /// <param name="args">arguments to parse</param>
        /// <returns>Parsed argument class or null in case of error</returns>
        public static T ParseCommandLine<T>(this string[] args)
            where T : class, new()
        {
            return ParseCommandLine<T>(args, Console.Out, Console.Error);
        }

        /// <summary>
        /// Parse argument class and show help
        /// </summary>
        /// <typeparam name="T">argument class type</typeparam>
        /// <param name="writer">TextWriter to write output to</param>
        public static void Help<T>(this TextWriter writer) where T : new()
        {
            new CommandLineParser<T>().GenerateHelp(writer);
        }
    }
}