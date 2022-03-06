using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miracle.Arguments
{
	/// <summary>
	/// Controls how command line arguments are parsed
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ArgumentSettingsAttribute : Attribute
	{
		private char[] _startOfArgument = new[] { '/', '-' };
		private char[] _valueSeparator = new[] { ':', '=', ' ' };
		private StringComparison _argumentNameComparison = StringComparison.InvariantCultureIgnoreCase;
		private DuplicateArgumentBehaviour _duplicateArgumentBehaviour = DuplicateArgumentBehaviour.Fail;
		private string _helpExecutableName = null;
		private int _helpWidth = 80;
		private int _helpIndent = 4;
		private bool _showHelpOnArgumentErrors = true;

		/// <summary>
		/// Characters signifying the beginning of a named argument
		/// </summary>
		public char[] StartOfArgument
		{
			get { return _startOfArgument; }
			set
			{
				if(value == null)
					throw new ArgumentNullException();

				if(value.Length == 0)
					throw new RankException();

				_startOfArgument = value;
			}
		}

		/// <summary>
		/// Characters signifying the end of a named argument, and the beginning of the value
		/// </summary>
		public char[] ValueSeparator
		{
			get { return _valueSeparator; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();

				if (value.Length == 0)
					throw new RankException();

				_valueSeparator = value;
			}
		}

		/// <summary>
		/// StringComparison used when comparing argument names.
		/// </summary>
		public StringComparison ArgumentNameComparison
		{
			get { return _argumentNameComparison; }
			set { _argumentNameComparison = value; }
		}

		/// <summary>
		/// Specifies how duplicate named arguments ar handled.
		/// </summary>
		public DuplicateArgumentBehaviour DuplicateArgumentBehaviour
		{
			get { return _duplicateArgumentBehaviour ; }
			set { _duplicateArgumentBehaviour = value; }
		}

		internal IEqualityComparer<string> GetStringComparer()
		{
			switch (ArgumentNameComparison)
			{
				case StringComparison.CurrentCulture:
					return StringComparer.CurrentCulture;
				case StringComparison.CurrentCultureIgnoreCase:
					return StringComparer.CurrentCultureIgnoreCase;
				case StringComparison.InvariantCulture:
					return StringComparer.InvariantCulture;
				case StringComparison.InvariantCultureIgnoreCase:
					return StringComparer.InvariantCultureIgnoreCase;
				case StringComparison.Ordinal:
					return StringComparer.Ordinal;
				case StringComparison.OrdinalIgnoreCase:
					return StringComparer.OrdinalIgnoreCase;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Check if argument is considered named argument using current argument settings
		/// </summary>
		/// <param name="arg">Argument to check</param>
		/// <returns>True if argument is a named argument</returns>
		public bool IsNamedArgument(string arg)
		{
			return StartOfArgument.Any(x=>x == arg[0]);
		}

		/// <summary>
		/// Convert arg into named argument using current argument settings
		/// </summary>
		/// <param name="arg">single command line argument</param>
		/// <returns>Argument split into name/value</returns>
		public NamedArgument GetNamedArgument(string arg)
		{
			if(IsNamedArgument(arg))
			{
				arg = arg.Substring(1);
				var idx = arg.IndexOfAny(ValueSeparator);
				if (idx > 0)
				{
					return new NamedArgument
					       	{
					       		ArgumentName = arg.Substring(0, idx),
					       		ArgumentValue = arg.Substring(idx + 1)
					       	};
				}
				return new NamedArgument
				       	{
				       		ArgumentName = arg,
				       	};
			}
			return null;
		}

		/// <summary>
		/// Name of executable used in help
		/// </summary>
		public string HelpExecutableName
		{
			get
            {
                var entryAssembly = Assembly.GetEntryAssembly();
                return _helpExecutableName
                    ?? (entryAssembly != null
                        ? GetExeName(entryAssembly)
                        : "Unit test");
            }
            set { _helpExecutableName = value; }
		}

        private static string GetExeName(Assembly entryAssembly)
        {
            var name = entryAssembly.ManifestModule.Name;
            return name.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) 
				? name.Substring(0, name.Length - 4) + ".exe" 
				: name;
        }

        /// <summary>
        /// Width of help 
        /// </summary>
        public int HelpWidth
		{
			get { return _helpWidth; }
			set { _helpWidth = value; }
		}

		/// <summary>
		/// Indent of each help description lines
		/// </summary>
		public int HelpIndent
		{
			get { return _helpIndent; }
			set { _helpIndent = value; }
		}

		/// <summary>
		/// Automatically show help when an argument error occur
		/// </summary>
		public bool ShowHelpOnArgumentErrors
		{
			get { return _showHelpOnArgumentErrors; }
			set { _showHelpOnArgumentErrors = value; }
		}
	}
}