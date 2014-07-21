using System;
using System.Reflection;

namespace Miracle.Arguments.Test.Sample
{
    /// <summary>
    /// Sample argument class that shows examples of most of the functionality of the CommandLineParser.
    /// </summary>
    [ArgumentSettings(
        ArgumentNameComparison = StringComparison.InvariantCultureIgnoreCase,
        DuplicateArgumentBehaviour = DuplicateArgumentBehaviour.Unknown,
        StartOfArgument = new[] {'-'},
        ValueSeparator = new[] {' ','='},
        ShowHelpOnArgumentErrors = false
        )]
    [ArgumentDescription("Sample program that shows most of most of the functionality of the CommandLineParser.")]
    public class SampleArgumentClass
    {
        [ArgumentPosition(0)]
        [ArgumentDescription("Positional arguments are assigned to argument targets in the order specified by argument position. Positions don't need to be sequential, but they must be unique.")]
        public string FirstPositionalArgument { get; set; }

        [ArgumentPosition(22)]
        [ArgumentDescription(
            "Argument targets can be of any type that implements IConvertible. The common language runtime types: Boolean, SByte, Byte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Single, Double, Decimal, DateTime, Char, and String implements this interface. In addition to these, Guid, TimeSpan and Enum is also supported."
            )]
        public ulong SecondPositionalArgument { get; set; }

        [ArgumentPosition(1000)]
        [ArgumentDescription("The last positional argument can be an array. All remaining positional arguments are assigned to this target.")]
        public string[] RemainingPositionalArguments { get; set; }

        [ArgumentName("NamedArgument", "NA")]
        [ArgumentDescription(
            "Named arguments can be specified anywhere on the command line: In any order and before of after positional arguments. Named arguments can be access using any of the specified argument names.")]
        public string MyNamedArgument { get; set; }

        [ArgumentName("Number", "N")]
        [ArgumentDescription("Numerical named argument.")]
        public int Number { get; set; }

        [ArgumentName("Required", "R")]
        [ArgumentRequired]
        [ArgumentDescription("Required argument targets throws an exception if they are missing.")]
        public bool IsRequired { get; set; }

        [ArgumentName("Boolean", "Bool", "B")]
        [ArgumentDescription("Boolean named argument target dont need to specify a value, but it is legal to write: -Bool=false.")]
        public bool Boolean { get; set; }

        [ArgumentName("Flag", "F")]
        [ArgumentDescription("A nullable argument target is only set to a value when the argument is specified on the command line.")]
        public bool? Flag { get; set; }

        [ArgumentName("Multiple", "M")]
        [ArgumentDescription("Arrays or 'MultiValue' targets makes it possible to specify an argument several times on the command line.")]
        public string[] Multiple { get; set; }

        [ArgumentName("MultipleNumeric", "MN")]
        [ArgumentDescription("Any supported type can be 'MultiValue'.")]
        public int[] MultipleNumeric { get; set; }

        [ArgumentName("WithDefault")]
        [ArgumentDescription("Default values can be assigned to attribute target before parse. It is guarnteed that targets will retain its value if not specified on the command line.")]
        public string NamedArgumentWithDefault
        {
            get { return _namedArgumentWithDefault; }
            set { _namedArgumentWithDefault = value; }
        }

        private string _namedArgumentWithDefault = "default value";

        [ArgumentName("BindingFlags", "BF")]
        [ArgumentDescription("It is possible to use Enums as argument targets")]
        public BindingFlags BindingFlags
        {
            get { return _bindingFlags; }
            set { _bindingFlags = value; }
        }

        private BindingFlags _bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;

        /// <summary>
        /// A single argument target can specify ArgumentUnknown attribute (must be string array).
        /// All unknown arguments are added to this target, and wont throw an exception.
        /// If DuplicateArgumentBehaviour is set to DuplicateArgumentBehaviour.Unknown, then all named arguments that has already been specified, is added to this target too.
        /// </summary>
        [ArgumentUnknown]
        public string[] UnknownArguments { get; set; }

        [ArgumentCommand(typeof (SourceArgumentClass), "Copy", "CP")]
        [ArgumentCommand(typeof (MoveArgumentClass), "Move", "MV")]
        [ArgumentCommand(typeof (DeleteArgumentClass), "Delete", "DEL")]
        public object[] Commands { get; set; }

        [ArgumentName("Help", "H", "?")]
        [ArgumentHelp()]
        [ArgumentDescription(@"Help can be generated automatically from the description attribute of each argument target.
The first name of a named arguments is assumed to be the primary name of the target, the remaining names are assumed to be aliases.
The property name is used as the 'value' name.")]
        public bool Help { get; set; }
    }

    public class CopyArgumentClass
    {
        [ArgumentName("Source", "Src")]
        [ArgumentRequired]
        public string Source { get; set; }

        [ArgumentName("Destination", "Dest")]
        [ArgumentRequired]
        public string Destination { get; set; }
    }

    public class MoveArgumentClass
    {
        [ArgumentPosition(0)]
        [ArgumentRequired]
        [ArgumentDescription("From file name.")]
        public string From { get; set; }

        [ArgumentName("To")]
        [ArgumentRequired]
        public string To { get; set; }
    }

    public class DeleteArgumentClass
    {
        [ArgumentPosition(0)]
        [ArgumentRequired]
        [ArgumentDescription("Source file name.")]
        public string Source { get; set; }
    }
}

