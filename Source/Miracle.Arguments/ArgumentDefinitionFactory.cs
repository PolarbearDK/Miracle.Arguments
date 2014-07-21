using System;
using System.Reflection;

namespace Miracle.Arguments
{
	/// <summary>
	/// Factory class for Argument Definition
	/// </summary>
	static class ArgumentDefinitionFactory
	{
		/// <summary>
		/// Create argument definition from PropertyInfo
		/// </summary>
		/// <param name="propertyInfo">property </param>
		/// <returns></returns>
		public static IArgumentDefinition Create(PropertyInfo propertyInfo)
		{
			var argType = propertyInfo.PropertyType;
			if (argType.IsArray) argType = argType.GetElementType();
			Type generic = typeof(ArgumentDefinition<>).MakeGenericType(argType);
			return (IArgumentDefinition)Activator.CreateInstance(generic, new object[] { propertyInfo });
		}
	}
}