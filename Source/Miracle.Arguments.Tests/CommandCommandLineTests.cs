using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Miracle.Arguments.Test
{
    [TestFixture]
    public class CommandCommandLineTests
    {
        /// <summary>
        /// Sample argument class that shows examples of most of the functionality of the CommandLineParser.
        /// </summary>
        [ArgumentSettings(
            ArgumentNameComparison = StringComparison.InvariantCultureIgnoreCase,
            DuplicateArgumentBehaviour = DuplicateArgumentBehaviour.Unknown,
            StartOfArgument = new[] { '-' },
            ValueSeparator = new[] { ' ' },
            ShowHelpOnArgumentErrors = false
            )]
        [ArgumentDescription("Unit test command arguments.")]
        public class TestArgumentClass
        {
            public TestArgumentClass()
            {
                StringComparison = StringComparison.CurrentCultureIgnoreCase;
            }

            [ArgumentName("Boolean", "Bool", "B")]
            [ArgumentDescription("Boolean named argument target dont need to specify a value, but it is legal to write: -Bool=false.")]
            public bool Boolean { get; set; }

            [ArgumentName("Number", "N")]
            [ArgumentDescription("Number named argument")]
            public int Number { get; set; }

            [ArgumentPosition(0)]
            [ArgumentDescription("StringComparison used for string compares. Default is CurrentCultureIgnoreCase.")]
            public StringComparison StringComparison { get; set; }

            [ArgumentPosition(1)]
            public StringComparison PositionalEnumWithoutDescription { get; set; }

            [ArgumentPosition(2)]
            public string PositionalStringWithoutDescription { get; set; }

            /// <summary>
            /// Commands are optional arguments that can be specified zero, one or several times. Each command has its on command line parser. 
            /// Type can be an array which allows several commands, or a non array which indicates zero ore one usage.
            /// Type can be any type that all command argument types inherit form (object, interface or base class).
            /// [ArgumentRequired] is supported.
            /// </summary>
            [ArgumentCommand(typeof(CopyArgumentClass), "Copy", "CP")]
            [ArgumentCommand(typeof(MoveArgumentClass), "Move", "MV")]
            [ArgumentCommand(typeof(DeleteArgumentClass), "Delete", "DEL")]
            [ArgumentRequired]
            public object[] MyCommands { get; set; }

            [ArgumentName("Help", "H", "?")]
            [ArgumentHelp()]
            [ArgumentDescription(@"Help can be generated automatically from the description attribute of each argument target.
The first name of a named arguments is assumed to be the primary name of the target, the remaining names are assumed to be aliases.
Help for individual commands are available through [ArgumentCommandHelp].")]
            public bool Help { get; set; }

            [ArgumentName("CmdHelp", "CH")]
            [ArgumentCommandHelp()]
            [ArgumentDescription(@"Help is generated automatically for specified command or sub command.")]
            public string CommandHelp { get; set; }
        }

        public class CommandBaseClass
        {
            [ArgumentName("CommonBaseArgument")]
            [ArgumentDescription("This named argument is common for all commands")]
            public string SomeCommonArgument { get; set; }

            [ArgumentPosition(100)]
            [ArgumentDescription("This positional argument is common for all commands")]
            public string SomeCommonPositionalArgument { get; set; }
        }

        public class CopyArgumentClass : CommandBaseClass
        {
            [ArgumentName("Source", "Src")]
            [ArgumentRequired]
            public string Source { get; set; }

            [ArgumentName("Destination", "Dest")]
            [ArgumentRequired]
            public string Destination { get; set; }

            [ArgumentName("Override", "ow", "o")]
            public bool Override { get; set; }

            [ArgumentPosition(0)]
            [ArgumentDescription("StringComparison used for string compares. Default is CurrentCultureIgnoreCase.")]
            public StringComparison StringComparison { get; set; }

            [ArgumentCommand(typeof(CriteriaArgumentClass), "Where")]
            public object[] MyCriteria { get; set; }
        }

        [ArgumentDescription("Example of a command with a description.")]
        public class MoveArgumentClass : CommandBaseClass
        {
            [ArgumentPosition(0)]
            [ArgumentRequired]
            [ArgumentDescription("From file name.")]
            public string From { get; set; }

            [ArgumentName("To")]
            [ArgumentRequired]
            public string To { get; set; }

            [ArgumentCommand(typeof(CriteriaArgumentClass), "Where")]
            [ArgumentDescription("Example of a sub command with a description.")]
            public object[] MyCriteria { get; set; }
        }

        public class DeleteArgumentClass // Not all inherit from CommandBaseClass
        {
            [ArgumentPosition(0)]
            [ArgumentRequired]
            [ArgumentDescription("Source file name.")]
            public string Source { get; set; }

            [ArgumentCommand(typeof(CriteriaArgumentClass), "Where","And")]
            public object[] MyCriteria { get; set; }
        }

        [ArgumentDescription("Example of a command used by other commands.")]
        public class CriteriaArgumentClass : CommandBaseClass
        {
            [ArgumentPosition(0)]
            [ArgumentDescription("A string criteria extression")]
            public string CriteriaExpression { get; set; }
        }

        [Test]
        public void SimpleTest()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("-bool copy -src abc -dest def");
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.EqualTo(0));

            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Boolean, Is.True);
            Assert.That(parsedArguments.Help, Is.False);
            Assert.That(parsedArguments.MyCommands, Is.Not.Null);
            Assert.That(parsedArguments.MyCommands.Length, Is.EqualTo(1));
            Assert.That(parsedArguments.MyCommands.First(), Is.TypeOf<CopyArgumentClass>());

            var copy = (CopyArgumentClass)parsedArguments.MyCommands.First();
            Assert.That(copy.Source, Is.EqualTo("abc"));
            Assert.That(copy.Destination, Is.EqualTo("def"));
        }

        [Test]
        public void SimpleNumberTest()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("-number 42 copy -src abc -dest def");
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.EqualTo(0));

            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Boolean, Is.False);
            Assert.That(parsedArguments.Number, Is.EqualTo(42));
            Assert.That(parsedArguments.Help, Is.False);
            Assert.That(parsedArguments.MyCommands, Is.Not.Null);
            Assert.That(parsedArguments.MyCommands.Length, Is.EqualTo(1));
            Assert.That(parsedArguments.MyCommands.First(), Is.TypeOf<CopyArgumentClass>());

            var copy = (CopyArgumentClass)parsedArguments.MyCommands.First();
            Assert.That(copy.Source, Is.EqualTo("abc"));
            Assert.That(copy.Destination, Is.EqualTo("def"));
        }

        [Test]
        public void MultipleSubCommandsTest()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("copy -src abc -dest def move first -to second -bool");
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.EqualTo(0));

            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Boolean, Is.True);
            Assert.That(parsedArguments.Help, Is.False);
            Assert.That(parsedArguments.MyCommands, Is.Not.Null);
            Assert.That(parsedArguments.MyCommands.Length, Is.EqualTo(2));
            Assert.That(parsedArguments.MyCommands.First(), Is.TypeOf<CopyArgumentClass>());
            Assert.That(parsedArguments.MyCommands.Last(), Is.TypeOf<MoveArgumentClass>());

            var copy = (CopyArgumentClass)parsedArguments.MyCommands.First();
            Assert.That(copy.Source, Is.EqualTo("abc"));
            Assert.That(copy.Destination, Is.EqualTo("def"));

            var move = (MoveArgumentClass)parsedArguments.MyCommands.Last();
            Assert.That(move.From, Is.EqualTo("first"));
            Assert.That(move.To, Is.EqualTo("second"));
        }

        [Test]
        public void MultipleSubCommandsReentrantTest()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("copy -src abc -dest def -override -bool copy -src ghi -dest jkl");
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.EqualTo(0));

            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Boolean, Is.True);
            Assert.That(parsedArguments.Help, Is.False);
            Assert.That(parsedArguments.MyCommands, Is.Not.Null);
            Assert.That(parsedArguments.MyCommands.Length, Is.EqualTo(2));
            Assert.That(parsedArguments.MyCommands, Has.All.TypeOf<CopyArgumentClass>());

            var copy = (CopyArgumentClass)parsedArguments.MyCommands.First();
            Assert.That(copy.Source, Is.EqualTo("abc"));
            Assert.That(copy.Destination, Is.EqualTo("def"));
            Assert.That(copy.Override, Is.True);

            copy = (CopyArgumentClass)parsedArguments.MyCommands.Last();
            Assert.That(copy.Source, Is.EqualTo("ghi"));
            Assert.That(copy.Destination, Is.EqualTo("jkl"));
            Assert.That(copy.Override, Is.False);
        }

        [Test]
        public void MultipleSubCommandFailureTests()
        {
            Assert.That(CommandLineParserExtensions.ParseCommandLine<TestArgumentClass>("copy abc -dest def move first -to second -bool".ToArgs()), Is.Null);
            Assert.That(CommandLineParserExtensions.ParseCommandLine<TestArgumentClass>("copy -src abc def move first -to second -bool".ToArgs()), Is.Null);
            Assert.That(CommandLineParserExtensions.ParseCommandLine<TestArgumentClass>("copy -src abc -dest def copy first -to second -bool".ToArgs()), Is.Null);
            Assert.That(CommandLineParserExtensions.ParseCommandLine<TestArgumentClass>("copy -src abc -dest def noop first -to second".ToArgs()), Is.Null);
            Assert.That(CommandLineParserExtensions.ParseCommandLine<TestArgumentClass>("copy -src abc -dest def move first".ToArgs()), Is.Null);
            Assert.That(CommandLineParserExtensions.ParseCommandLine<TestArgumentClass>("-bool".ToArgs()), Is.Null);
        }

        [Test]
        public void HelpTest1()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("-help");
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.GreaterThan(0));
            Assert.That(parsedArguments, Is.Null);

            Assert.That(_output, Contains.Substring("Move"));
            Assert.That(_output, Contains.Substring("Copy"));
            Assert.That(_output, Contains.Substring("Delete"));

            Assert.That(_output.Count("-CommonBaseArgument"), Is.EqualTo(0));
        }

        private static string _error;
        private static string _output;

        [Test]
        public void CommandGenericHelpTest1()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("-cmdhelp copy");
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.GreaterThan(0));
            Assert.That(parsedArguments, Is.Null);

            Assert.That(_output.Count("Move"), Is.EqualTo(0));
            Assert.That(_output.Count("Copy"), Is.EqualTo(1));
            Assert.That(_output.Count("Where"), Is.EqualTo(1));
            Assert.That(_output.Count("-CommonBaseArgument"), Is.EqualTo(2));
        }

        [Test]
        public void CommandHelpTest1()
        {
            var parsedArguments = ParseWithOutput<TestArgumentClass>("-help");
            Assert.That(parsedArguments, Is.Null);
            Assert.That(_error.Length, Is.EqualTo(0));
            Assert.That(_output.Length, Is.GreaterThan(0));

            Assert.That(_output.Count("StringComparison"), Is.EqualTo(3));
            Assert.That(_output.Count("PositionalEnumWithoutDescription"), Is.EqualTo(2));
            Assert.That(_output.Count("PositionalStringWithoutDescription"), Is.EqualTo(1));

            Assert.That(_output, Is.Not.Null);
            Assert.That(_output.Count("Move"), Is.EqualTo(1));
            Assert.That(_output.Count("Copy"), Is.EqualTo(1));
            Assert.That(_output.Count("Delete"), Is.EqualTo(1));
            Assert.That(_output.Count("-CommonBaseArgument"), Is.EqualTo(0));
            Assert.That(_output.Count("Where"), Is.EqualTo(0));
        }

        [Test]
        public void CommandHelpForCommandTest()
        {
            var parser = new CommandLineParser<TestArgumentClass>();
            var writer = new StringWriter();

            parser.GenerateCommandHelp(writer, "Move");
            var output = writer.ToString();

            Console.WriteLine("Output (length {0}):", output.Length);
            Console.WriteLine(output);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.Count("Move"), Is.EqualTo(1));
            Assert.That(output.Count("Copy"), Is.EqualTo(0));
            Assert.That(output.Count("-CommonBaseArgument"), Is.EqualTo(2));
            Assert.That(output.Count("SomeCommonPositionalArgument"), Is.EqualTo(2));
        }

        [Test]
        public void CommandHelpForCommand2Test()
        {
            var parser = new CommandLineParser<TestArgumentClass>();
            var writer = new StringWriter();

            parser.GenerateCommandHelp(writer, "copy");
            var output = writer.ToString();

            Console.WriteLine("Output (length {0}):", output.Length);
            Console.WriteLine(output);

            Assert.That(output, Is.Not.Null);
            Assert.That(output.Count("Move"), Is.EqualTo(0));
            Assert.That(output.Count("Copy"), Is.EqualTo(1));
            Assert.That(output.Count("-CommonBaseArgument"), Is.EqualTo(2));
            Assert.That(output.Count("SomeCommonPositionalArgument"), Is.EqualTo(2));
        }

        [Test]
        public void FindCommandCommandLineParserTests()
        {
            var parser = new CommandLineParser<TestArgumentClass>();

            var result = parser.FindCommandCommandLineParser("xyz");
            Assert.That(result, Is.Null);

            result = parser.FindCommandCommandLineParser("copy");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parser.GetType() == typeof (CommandLineParser<CopyArgumentClass>));

            result = parser.FindCommandCommandLineParser("Where");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parser.GetType() == typeof(CommandLineParser<CriteriaArgumentClass>));

            result = parser.FindCommandCommandLineParser("And");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Parser.GetType() == typeof(CommandLineParser<CriteriaArgumentClass>));
        }

        private static T ParseWithOutput<T>(string arguments) where T : class, new()
        {
            var error = new StringWriter();
            var output = new StringWriter();
            var args = arguments.ToArgs();

            var parsedArguments = args.ParseCommandLine<T>(output, error);

            _error = error.ToString();
            _output = output.ToString();

            Console.WriteLine("Error (length {0}):", _error.Length);
            Console.WriteLine(_error);
            Console.WriteLine("Output (length {0}):", _output.Length);
            Console.WriteLine(_output);

            return parsedArguments;
        }
    }

    static class MyClassExtensions
    {
        public static int Count(this string source, string find)
        {
            int count = 0, n = 0;

            while ((n = source.IndexOf(find, n, StringComparison.InvariantCulture)) != -1)
            {
                n += find.Length;
                ++count;
            }
            return count;
        }
    }
}