using System;
using System.Runtime.Serialization;

namespace Miracle.Arguments
{
	/// <summary>
	/// Exception thrown by Command line parser
	/// </summary>
	[Serializable]
	public class CommandLineParserException : Exception
	{
		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public CommandLineParserException()
		{
		}

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public CommandLineParserException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public CommandLineParserException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		protected CommandLineParserException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}