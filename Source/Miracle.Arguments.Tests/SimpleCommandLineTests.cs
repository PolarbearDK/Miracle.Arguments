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
            ValueSeparator = new[] { ' ' },
            ShowHelpOnArgumentErrors = false
            )]
        [ArgumentDescription("Sample program that shows most of most of the functionality of the CommandLineParser.")]
        public class TestArgumentClass1
        {
            [ArgumentName("MyFlag", "MF")]
            public bool Flag { get; set; }

            [ArgumentName("Number", "N")]
            public int Number { get; set; }

            [ArgumentName("Help", "H", "?")]
            [ArgumentHelp()]
            public bool Help { get; set; }
        }

        [Test]
        public void HelpTest()
        {
            var args = "-Help".ToArgs();

            var parsedArguments = CommandLineParserExtensions.ParseCommandLine<TestArgumentClass1>(args);
            Assert.That(parsedArguments, Is.Null);
        }

        [Test]
        public void FlagTest1()
        {
            var args = "-MyFlag".ToArgs();
            var parsedArguments = CommandLineParserExtensions.ParseCommandLine<TestArgumentClass1>(args);
            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Help, Is.False);
            Assert.That(parsedArguments.Flag, Is.True);
            Assert.That(parsedArguments.Number, Is.EqualTo(0));
        }
        [Test]
        public void FlagTest2()
        {
            var args = "-N 2 -MyFlag".ToArgs();

            var parsedArguments = CommandLineParserExtensions.ParseCommandLine<TestArgumentClass1>(args);
            Assert.That(parsedArguments, Is.Not.Null);
            Assert.That(parsedArguments.Help, Is.False);
            Assert.That(parsedArguments.Flag, Is.True);
            Assert.That(parsedArguments.Number, Is.EqualTo(2));
        }
    }
}