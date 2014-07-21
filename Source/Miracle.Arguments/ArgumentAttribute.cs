using System;

namespace Miracle.Arguments
{
	/// <summary>
	/// Abstract base class for all Argument attributes
	/// </summary>
	public abstract class ArgumentAttribute : Attribute
	{
	}

	/// <summary>
	/// Description statement Indicates that a public property represents a named command line argument. 
	/// Use multiple attributes for aliases.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
	public class ArgumentDescriptionAttribute : ArgumentAttribute
	{
		/// <summary>
		/// Description test
		/// </summary>
		public string Description { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="description">Description of an argument target or argument class</param>
		public ArgumentDescriptionAttribute(string description)
		{
			Description = description;
		}
	}

	/// <summary>
	/// Indicates that a public property represents a named command line argument. 
	/// Use multiple attributes for aliases.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ArgumentNameAttribute : ArgumentAttribute
	{
		/// <summary>
		/// Name/Alias of a named argument
		/// </summary>
		public string[] Names { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="names">Name/Aliases of a named argument</param>
		public ArgumentNameAttribute(params string[] names)
		{
			Names = names;
		}
	}

	/// <summary>
	/// Indicates that a public property represents a unnamed (positional) command line argument. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ArgumentPositionAttribute : ArgumentAttribute
	{
		/// <summary>
		/// Unique position of positional argument
		/// </summary>
		public uint Position { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="position">Unique position of positional argument</param>
		public ArgumentPositionAttribute(uint position)
		{
			Position = position;
		}
	}

	/// <summary>
	/// Indicates that a public property are to recieve all unknown arguments. 
	/// Only one property on an argument class can have this attribute, and the property must be assignable from a string array
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ArgumentUnknownAttribute : ArgumentAttribute
	{
	}

	/// <summary>
	/// Indicates that an argument is required. Used with ArgumentName/ArgumentPosition attributes
	/// Only one property on an argument class can have this attribute, and the property must be assignable from a string array
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class ArgumentRequiredAttribute : ArgumentAttribute
	{
	}

    /// <summary>
    /// Indicates that when this argument is true, help is automatically shown by the command line parser
    /// The property must be a boolean
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentHelpAttribute : ArgumentAttribute
    {
    }

    /// <summary>
    /// Indicates that when this argument contains a value, then help is automatically shown by the command line parser for that command
    /// The property must be a string
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ArgumentCommandHelpAttribute : ArgumentAttribute
    {
    }

    /// <summary>
    /// Indicates that a public property represents a named collection of sub commands. 
    /// Use multiple attributes for aliases.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ArgumentCommand : ArgumentAttribute
    {
        /// <summary>
        /// The type of the sub command
        /// </summary>
        public Type Type { get; private set; }
        
		/// <summary>
		/// Name/Alias of a named argument
		/// </summary>
		public string[] Names { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">Type of sub command</param>
        /// <param name="names">Name/Aliases of a named sub command</param>
        public ArgumentCommand(Type type, params string[] names)
        {
			Names = names;
            Type = type;
        }
    }
}
