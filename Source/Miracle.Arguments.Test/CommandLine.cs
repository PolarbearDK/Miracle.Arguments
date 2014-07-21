using System;
using NUnit.Framework;

namespace Miracle.Arguments.Test
{

    [TestFixture]
    public class SimpleCommandLineTests
    {
        /// <summary>
        /// Sample argument class that shows examples of most of the functionality of the CommandLineParser.
        /// </summary>
        [ArgumentSettings(
            ArgumentNameComparison = StringComparison.InvariantCultureIgnoreCase,
            DuplicateArgumentBehaviour = DuplicateArgumentBehaviour.Unknown,
            StartOfArgument = new[] { '-' },
            ValueSeparator = new[] { ':' },
            ShowHelpOnArgumentErrors = false
            )]
        [ArgumentDescription("Sample program that shows most of most of the functionality of the CommandLineParser.")]
        public class TestArgumentClass1
        {
            [ArgumentName("MyFlag", "MF")]
            [ArgumentDescription(
                "Named arguments can be specified anywhere on the command line: In any order and before of after positional arguments. Named arguments can be access using any of the specified argument names.")]
            public bool Flag { get; set; }

            [ArgumentName("Number", "N")]
            [ArgumentDescription("Numerical named argument.")]
            public int Number { get; set; }

            [ArgumentName("Help", "H", "?")]
            [ArgumentHelp()]
            [ArgumentDescription(@"Help can be generated automatically from the description attribute of each argument target.
The first name of a named arguments is assumed to be the primary name of the target, the remaining names are assumed to be aliases.
The property name is used as the 'value' name.")]
            public bool Help { get; set; }
        }

        [Test]
        public void Test1()
        {
            var args = new[] {"-help"};

            var parsedArguments = CommandLineParser.ParseCommandLine<TestArgumentClass1>(args);
            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Help, Is.True);
            Assert.That(parsedArguments.Flag, Is.False);
            Assert.That(parsedArguments.Number, Is.EqualTo(0));
        }
    }
}