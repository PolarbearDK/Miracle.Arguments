using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miracle.Arguments
{
	/// <summary>
	/// Implementation of an argument definition
	/// </summary>
	/// <typeparam name="T">Any type that can be converted from string</typeparam>
	class ArgumentDefinition<T> : IArgumentDefinition
	{
		private readonly PropertyInfo _propertyInfo;
		private readonly List<T> _values = new List<T>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="propertyInfo"></param>
		public ArgumentDefinition(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		#region IArgumentDefinition Members

		private object ChangeType(string value, Type conversionType)
		{
			if(conversionType.IsEnum)
				return Enum.Parse(conversionType, value, true);

            if (conversionType == typeof(Guid))
                return Guid.Parse(value);

            if (conversionType == typeof(TimeSpan))
                return TimeSpan.Parse(value);

			return Convert.ChangeType(value, conversionType);
		}

		/// <summary>
		/// Add value to list of values
		/// </summary>
		/// <param name="value">String value that are to be converted to T</param>
		public void AddValue(string value)
		{
			Type type = typeof(T);

			try
			{
				if (value == null && IsBoolean)
					value = true.ToString();

				object convertedValue = null;

				if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
				{
                    Type underlyingType = Nullable.GetUnderlyingType(type);
                    if (value != null)
					    convertedValue = ChangeType(value, underlyingType);
                    else
                    {
                        if(underlyingType.IsEnum)
                            throw new CommandLineParserException("Missing enum value for nullable argument");
                    }
				}
				else
					convertedValue = ChangeType(value, type);

                AddRawValue(convertedValue);
			}
			catch (Exception ex)
			{
                throw new RetryableCommandLineArgumentException(string.Format("Unable to convert [{0}] into {1}", value, type.Name), ex);
			}
		}

	    public void AddRawValue(object value)
	    {
            if (IsMultiValue || (!HasValue))
                _values.Add((T)value);
            else
                _values[0] = (T)value;
	    }

	    /// <summary>
		/// Get collected value(s) for this argument
		/// </summary>
		/// <param name="argumentClass">class that are to recieve value</param>
		public void GetValues(object argumentClass)
		{
			// has value been set
			if (_values.Count > 0)
			{
				_propertyInfo.SetValue(argumentClass,
				                       IsMultiValue
				                       	? (object)_values.ToArray()
				                       	: _values[0],
				                       null);
			}

			// Otherwise leave it alone!
		}

	    public void Clear()
	    {
	        _values.Clear();
	    }

	    /// <summary>
		/// Is this a required argument?
		/// </summary>
		public bool IsRequired
		{
			get { return _propertyInfo.GetCustomAttributes(typeof(ArgumentRequiredAttribute), true).Any(); }
		}

		/// <summary>
		/// Is this a multi value argument?
		/// </summary>
		public bool IsMultiValue
		{
			get { return _propertyInfo.PropertyType.IsArray; }
		}

        /// <summary>
        /// Is this a boolean argument?
        /// </summary>
        public bool IsBoolean
        {
            get { return (typeof(T) == typeof(bool) || typeof(T) == typeof(bool?)); }
        }

        /// <summary>
        /// Is this a boolean argument?
        /// </summary>
        public bool IsString
        {
            get { return (typeof(T) == typeof(string)); }
        }

	    private Type ArgumentType
	    {
	        get
	        {
	            Type type = typeof (T);

	            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof (Nullable<>)))
	            {
	                type = Nullable.GetUnderlyingType(type);
	            }
	            return type;
	        }
	    }

	    public bool IsEnum
	    {
	        get { return ArgumentType.IsEnum; }
	    }

	    public string[] EnumValueList
	    {
	        get
	        {
	            Type type = ArgumentType;
	            return type.IsEnum 
                    ? Enum.GetNames(type) 
                    : new string[] {};
	        }
	    }

	    /// <summary>
		/// Does this argument contain at least one value?
		/// </summary>
		public bool HasValue
		{
			get { return _values.Count > 0; }
		}

		public object Value
		{
			get
			{
				return HasValue
				       	? (object) _values[0]
				       	: null;
			}
		}

		public PropertyInfo Property
		{
			get { return _propertyInfo; }
		}

		#endregion
	}
}