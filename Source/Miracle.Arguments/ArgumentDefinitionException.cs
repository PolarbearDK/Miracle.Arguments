using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Miracle.Arguments
{
	/// <summary>
	/// The exception that is thrown when the command line parser finds a problem while scanning the argument class
	/// </summary>
	public class ArgumentDefinitionException : CommandLineParserException
	{
		/// <summary>
		/// PropertyInfo of failed argument definition
		/// </summary>
		public PropertyInfo PropertyInfo { get; private set; }

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public ArgumentDefinitionException(PropertyInfo propertyInfo)
		{
			PropertyInfo = propertyInfo;
		}

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public ArgumentDefinitionException(PropertyInfo propertyInfo, string message)
			: base(string.Format("{0} ({1})", message, propertyInfo.Name))
		{
			PropertyInfo = propertyInfo;
		}

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		public ArgumentDefinitionException(PropertyInfo propertyInfo, string message, Exception innerException)
			: base(string.Format("{0} ({1})", message, propertyInfo.Name), innerException)
		{
			PropertyInfo = propertyInfo;
		}

		/// <summary>
		/// Constructor (overloaded)
		/// </summary>
		protected ArgumentDefinitionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}