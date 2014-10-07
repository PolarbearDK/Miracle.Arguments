using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Miracle.Arguments
{
    public partial class CommandLineParser<T>
    {
        /// <summary>
        /// Generate description part of help from ArgumentDescriprion attribute on the settings class.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateHelpDescription(TextWriter writer)
        {
            var description = (ArgumentDescriptionAttribute)typeof(T).GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
            if (description != null)
            {
                WordWrap.WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth, 0);
                writer.WriteLine();
            }
        }

        /// <summary>
        /// Generate command line usage help for positional arguments.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GeneratePositionalCommandLineHelp(TextWriter writer)
        {
            foreach (var argument in _positionalArguments.Values)
            {
                writer.Write(' ');
                if (!argument.IsRequired) writer.Write('[');
                writer.Write('<');
                writer.Write(argument.Property.Name);
                writer.Write('>');
                if (argument.IsMultiValue) writer.Write("...");
                if (!argument.IsRequired) writer.Write(']');
            }
        }

        /// <summary>
        /// Generate command line usage help for named arguments.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateNamedCommandLineHelp(TextWriter writer)
        {
            foreach (var argument in _namedArguments.Values.GroupBy(x => x).Select(x => x.Key))
            {
                var argumentName = (ArgumentNameAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).First();

                writer.Write(' ');
                if (!argument.IsRequired) writer.Write('[');
                writer.Write(_argumentsSettings.StartOfArgument[0]);
                writer.Write(argumentName.Names[0]);

                if (!argument.IsBoolean)
                {
                    writer.Write(_argumentsSettings.ValueSeparator[0]);
                    writer.Write('<');
                    writer.Write(argument.Property.Name);
                    writer.Write('>');
                    if (argument.IsMultiValue) writer.Write("...");
                }
                if (!argument.IsRequired) writer.Write(']');
            }
        }

        private void GenerateCommandCommandLineHelp(StringWriter writer)
        {
            if (_commandArgument != null)
            {
                var argumentName = (ArgumentNameAttribute)_commandArgument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).FirstOrDefault();
                var name = argumentName != null ? argumentName.Names[0] : _commandArgument.Property.Name;

                writer.Write(' ');
                if (!_commandArgument.IsRequired) writer.Write('{');
                writer.Write(name);
                if (_commandArgument.IsMultiValue) writer.Write("...");
                if (!_commandArgument.IsRequired) writer.Write('}');
            }
        }

        /// <summary>
        /// Generate command line usage help.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        /// <param name="isCommand"></param>
        public void GenerateCommandLineHelp(TextWriter writer, bool isCommand)
        {
            var sw = new StringWriter();

            if (!isCommand)
            {
                sw.Write("Usage: ");
                sw.Write(_argumentsSettings.HelpExecutableName);
            }

            GeneratePositionalCommandLineHelp(sw);
            GenerateNamedCommandLineHelp(sw);
            GenerateCommandCommandLineHelp(sw);

            WordWrap.WordWrapText(writer, sw.ToString(), _argumentsSettings.HelpWidth, 0);
        }

        /// <summary>
        /// Generate detailed command line usage help for each argument
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateDetailedCommandLineHelp(TextWriter writer)
        {
            foreach (var argument in _positionalArguments.Values)
            {
                var description = (ArgumentDescriptionAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
                // Only list argument if there is some info related to it.
                if (description != null || argument.IsEnum)
                {
                    writer.WriteLine();
                    writer.Write('<');
                    writer.Write(argument.Property.Name);
                    writer.WriteLine('>');
                    WriteEnumValueSet(writer, argument);

                    if (description != null)
                        WordWrap.WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth - _argumentsSettings.HelpIndent, _argumentsSettings.HelpIndent);
                }
            }

            foreach (var argument in _namedArguments.Values.GroupBy(x => x).Select(x => x.Key))
            {
                var argumentName = (ArgumentNameAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).First();

                writer.WriteLine();
                writer.Write(_argumentsSettings.StartOfArgument[0]);
                writer.Write(argumentName.Names[0]);

                if (!argument.IsBoolean)
                {
                    writer.Write(_argumentsSettings.ValueSeparator[0]);
                    writer.Write('<');
                    writer.Write(argument.Property.Name);
                    writer.Write('>');
                }

                if (argumentName.Names.Length > 1)
                {
                    writer.Write(" (Alias: ");
                    writer.Write(
                        String.Join("/",
                                    argumentName
                                        .Names
                                        .Skip(1)
                                        .Select(x => _argumentsSettings.StartOfArgument[0] + x)
                                        .ToArray()));
                    writer.Write(')');
                }
                writer.WriteLine();
                WriteEnumValueSet(writer, argument);

                var description = (ArgumentDescriptionAttribute)argument.Property.GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
                if (description != null)
                {
                    WordWrap.WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth - _argumentsSettings.HelpIndent, _argumentsSettings.HelpIndent);
                }
            }

            if (_commandArgument != null)
            {
                writer.WriteLine();
                var description = (ArgumentDescriptionAttribute)_commandArgument.Property.GetCustomAttributes(typeof(ArgumentDescriptionAttribute), true).FirstOrDefault();
                if (description != null)
                {
                    WordWrap.WordWrapText(writer, description.Description, _argumentsSettings.HelpWidth, 0);
                }

                var argumentName = (ArgumentNameAttribute)_commandArgument.Property.GetCustomAttributes(typeof(ArgumentNameAttribute), true).FirstOrDefault();
                var name = argumentName != null ? argumentName.Names[0] : _commandArgument.Property.Name;
                writer.Write(name);
                writer.WriteLine(" (Command):");

                foreach (var group in _commandArgumentTypes.GroupBy(x => x.Value, x => x.Key))
                {
                    var names = group.ToArray();

                    writer.Write(new string(' ', _argumentsSettings.HelpIndent));
                    writer.Write(names[0]);

                    if (names.Length > 1)
                    {
                        writer.Write(" (Alias: ");
                        writer.Write(String.Join("/", names.Skip(1).ToArray()));
                        writer.Write(')');
                    }

                    writer.WriteLine();
                }
            }
        }

        private void WriteEnumValueSet(TextWriter writer, IArgumentDefinition argument)
        {
            if (argument.IsEnum)
            {
                var valueText = "Value set: " + String.Join(", ", argument.EnumValueList);
                WordWrap.WordWrapText(writer, valueText, _argumentsSettings.HelpWidth - _argumentsSettings.HelpIndent, _argumentsSettings.HelpIndent);
            }
        }

        /// <summary>
        /// Generate command line help
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        public void GenerateHelp(TextWriter writer)
        {
            var cmdArgument = _helpArguments.FirstOrDefault(x => x.HasValue && x.IsString);
            if (cmdArgument != null)
            {
                GenerateCommandHelp(writer, (string)cmdArgument.Value);
            }
            else
            {
                GenerateHelpDescription(writer);
                GenerateCommandLineHelp(writer, false);
                GenerateDetailedCommandLineHelp(writer);
            }
        }
        /// <summary>
        /// Generate help for command
        /// </summary>
        /// <param name="writer">output to this writer</param>
        /// <param name="names">command names+aliases</param>
        public void GenerateCommandHelp(TextWriter writer, string[] names)
        {
            GenerateHelpDescription(writer);
            writer.Write("Command: ");
            writer.Write(names[0]);

            if (names.Length > 1)
            {
                writer.Write(" (Alias: ");
                writer.Write(String.Join("/", names.Skip(1).ToArray()));
                writer.Write(')');
            }

            GenerateCommandLineHelp(writer, true);
            GenerateDetailedCommandLineHelp(writer);
        }

        /// <summary>
        /// Generate help for command
        /// </summary>
        /// <param name="writer">output to this writer</param>
        /// <param name="command">command to find</param>
        public void GenerateCommandHelp(TextWriter writer, string command)
        {
            var commandMatch = FindCommandCommandLineParser(command);
            if (commandMatch != null)
            {
                commandMatch.Parser.GenerateCommandHelp(writer, commandMatch.Aliases);
            }
            else
            {
                writer.WriteLine("Unknown command: {0}", command);
            }
        }

        /// <summary>
        /// Find command by name in current parser or in any sub parsers.
        /// </summary>
        /// <param name="command">Name of command to find</param>
        /// <returns>Command match containg command parser and array of names associated with parser, or null if not found</returns>
        public CommandMatch FindCommandCommandLineParser(string command)
        {
            IEqualityComparer<string> equalityComparer = _argumentsSettings.GetStringComparer();
            IEnumerable<IGrouping<ICommandLineParser, string>> groups = _commandArgumentTypes.GroupBy(x => x.Value, x => x.Key);

            foreach (var group in groups)
            {
                var names = group.ToArray();
                if (names.Any(x => equalityComparer.Equals(x, command)))
                {
                    return new CommandMatch(group.Key, names);
                }
            }

            foreach (var commandType in groups.Select(x => x.Key))
            {
                var parser = commandType.FindCommandCommandLineParser(command);
                if (parser != null) return parser;
            }

            return null;
        }

        /// <summary>
        /// Automatically generate command line help when an argument error occur.
        /// </summary>
        /// <param name="writer">TextWriter to write output to</param>
        internal void AutoGenerateHelp(TextWriter writer)
        {
            if (_argumentsSettings.ShowHelpOnArgumentErrors)
                GenerateHelp(writer);
        }

        /// <summary>
        /// Return true if one of the argument targets has ArgumentHelp and is set to true
        /// </summary>
        public bool IsHelp
        {
            get
            {
                return _helpArguments.Any(x => x.HasValue
                                               && ((x.IsBoolean && ((bool)x.Value))
                                                   || x.IsString));
            }
        }
    }
}
