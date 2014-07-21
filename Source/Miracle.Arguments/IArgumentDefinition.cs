using System.Reflection;

namespace Miracle.Arguments
{
	/// <summary>
	/// Interface for an argument definition
	/// </summary>
	interface IArgumentDefinition
	{
		/// <summary>
		/// Add string value to argument definition
		/// </summary>
		/// <param name="value"></param>
		void AddValue(string value);
        /// <summary>
        /// Add raw (With a type of T) value to argument definition
        /// </summary>
        /// <param name="value"></param>
        void AddRawValue(object value);
		/// <summary>
		/// Get collected value(s) for this argument
		/// </summary>
		/// <param name="argumentClass">class that are to recieve value</param>
		void GetValues(object argumentClass);
        /// <summary>
        /// Clear all values
        /// </summary>
        void Clear();
        /// <summary>
		/// Is this a required argument?
		/// </summary>
		bool IsRequired { get; }
		/// <summary>
		/// Is this a multi value argument?
		/// </summary>
		bool IsMultiValue { get; }
        /// <summary>
        /// Is this a boolean (flag) argument?
        /// </summary>
        bool IsBoolean { get; }
        /// <summary>
        /// Is this a string argument?
        /// </summary>
        bool IsString { get; }
        /// <summary>
        /// Is this a Enum argument?
        /// </summary>
        bool IsEnum { get; }
        /// <summary>
        /// Is this a Enum argument?
        /// </summary>
        string[] EnumValueList { get; }
        /// <summary>
		/// Does this argument contain at least one value?
		/// </summary>
		bool HasValue { get; }
		/// <summary>
		/// Does this argument contain at least one value?
		/// </summary>
		object Value { get; }
		/// <summary>
		/// Reference to property info that defined this argument
		/// </summary>
		PropertyInfo Property { get; }

	}
}