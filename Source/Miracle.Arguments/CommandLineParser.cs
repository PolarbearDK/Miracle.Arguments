using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Miracle.Arguments
{
	/// <summary>
	/// Command line parser
	/// </summary>
	/// <typeparam name="T">argument class type</typeparam>
	public partial class CommandLineParser<T>: ICommandLineParser where T : new()
	{
		private readonly ArgumentSettingsAttribute _argumentsSettings;
		private readonly Dictionary<string, IArgumentDefinition> _namedArguments;
		private readonly SortedList<uint, IArgumentDefinition> _positionalArguments;
		private readonly List<IArgumentDefinition> _helpArguments;
        private IArgumentDefinition _unknownArguments;
		private IArgumentDefinition _commandArgument;
	    private readonly Dictionary<string, ICommandLineParser> _commandArgumentTypes;

	    /// <summary>
		/// Constructor. 
		/// </summary>
		/// <param name="defaultSettings">Settings used if T has no settings attribute</param>
		public CommandLineParser(ArgumentSettingsAttribute defaultSettings)
		{
			_argumentsSettings = (ArgumentSettingsAttribute)typeof(T).GetCustomAttributes(typeof(ArgumentSettingsAttribute), true).FirstOrDefault() ?? defaultSettings;
			_namedArguments = new Dictionary<string, IArgumentDefinition>(_argumentsSettings.GetStringComparer());
			_positionalArguments = new SortedList<uint, IArgumentDefinition>();
			_helpArguments = new List<IArgumentDefinition>();
			_unknownArguments = null;
			_commandArgument = null;
            _commandArgumentTypes = new Dictionary<string, ICommandLineParser>(_argumentsSettings.GetStringComparer());

			Scan();
		}

        /// <summary>
        /// 
        /// </summary>
	    public CommandLineParser()
            : this(new ArgumentSettingsAttribute())
	    {
	        
	    }

        /// <summary>
		/// Scan argument class T for attribute validity
		/// </summary>
		private void Scan()
		{
			foreach (var propertyInfo in typeof (T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				var custProps = propertyInfo.GetCustomAttributes(typeof (ArgumentAttribute), true);
				if (custProps.Any())
				{
					IArgumentDefinition argumentDefinition = ArgumentDefinitionFactory.Create(propertyInfo);

					var unknownArgument = custProps.OfType<ArgumentUnknownAttribute>().FirstOrDefault();
					if (unknownArgument != null)
					{
						if (custProps.Any(x => !(x is ArgumentUnknownAttribute)))
							throw new ArgumentDefinitionException(propertyInfo, "UnknownArguments can not be mixed with any other argument attribute");

						if (_unknownArguments != null)
							throw new ArgumentDefinitionException(propertyInfo, "Only one property on a class can specify UnknownArguments");

						if (!propertyInfo.PropertyType.IsAssignableFrom(typeof (string[])))
							throw new ArgumentDefinitionException(propertyInfo, "UnknownArguments property must be assignable from a string array");

						_unknownArguments = argumentDefinition;
					}
					else
					{
						var positionAttribute = custProps.OfType<ArgumentPositionAttribute>().FirstOrDefault();
						if (positionAttribute != null)
						{
							// Check duplicate posinions
							if (_positionalArguments.ContainsKey(positionAttribute.Position))
								throw new ArgumentDefinitionException(propertyInfo, "Multiple arguments at position " + positionAttribute.Position);

							// Check for mixed position/named arguments
							if (custProps.Any(x=>x is ArgumentNameAttribute))
								throw new ArgumentDefinitionException(propertyInfo, "Mixed positional and named arguments");

							// Check if this multivalue argument hides other positional arguments
							if(argumentDefinition.IsMultiValue)
							{
								if(_positionalArguments.Keys.Any(x=>x > positionAttribute.Position))
									throw new ArgumentDefinitionException(propertyInfo, "Multivalue argument at position " + positionAttribute.Position + " hides additional positional arguments");
							}

							// Check if this argument is hidden by previous multivalue
							if (_positionalArguments.Any(x=> x.Key < positionAttribute.Position && x.Value.IsMultiValue))
								throw new ArgumentDefinitionException(propertyInfo, "Argument at position " + positionAttribute.Position + " is hidden by previous multivalue argument");

							_positionalArguments.Add(positionAttribute.Position, argumentDefinition);
						}
						else
						{
							var nameAttribute = custProps.OfType<ArgumentNameAttribute>().FirstOrDefault();
						    if (nameAttribute != null)
						    {
						        foreach (var name in nameAttribute.Names)
						        {
						            if (_namedArguments.ContainsKey(name))
						                throw new ArgumentDefinitionException(propertyInfo, "Multiple named arguments with name " + name);

						            _namedArguments.Add(name, argumentDefinition);
						        }

						        if (custProps.OfType<ArgumentHelpAttribute>().Any())
						        {
						            if (argumentDefinition.IsBoolean && argumentDefinition.IsMultiValue == false)
						                _helpArguments.Add(argumentDefinition);
						            else
						                throw new ArgumentDefinitionException(propertyInfo, "Help argument attribute can only be applied to boolean properties.");
						        }

                                if (custProps.OfType<ArgumentCommandHelpAttribute>().Any())
						        {
						            if (argumentDefinition.IsString && argumentDefinition.IsMultiValue == false)
						                _helpArguments.Add(argumentDefinition);
						            else
						                throw new ArgumentDefinitionException(propertyInfo, "Command help argument attribute can only be applied to string properties.");
						        }
						    }
						    else
						    {
							    var commandAttributes = custProps.OfType<ArgumentCommand>().ToList();
						        if (commandAttributes.Any())
						        {
                                    if (_commandArgument != null)
                                        throw new ArgumentDefinitionException(propertyInfo, "Only one property on a class can specify ArgumentCommand");

						            _commandArgument = argumentDefinition;

						            foreach (var commandAttribute in commandAttributes)
						            {
						                var genericParserType = typeof (CommandLineParser<>).MakeGenericType(new [] {commandAttribute.Type});
                                        var parserInstance = (ICommandLineParser)Activator.CreateInstance(genericParserType, new object[] { _argumentsSettings });

						                foreach (var name in commandAttribute.Names)
						                {
						                    if (_commandArgumentTypes.ContainsKey(name))
						                        throw new ArgumentDefinitionException(propertyInfo, "Multiple command descriminator " + name);

						                    _commandArgumentTypes.Add(name, parserInstance);
						                }
						            }
                                }
                                else
                                    throw new ArgumentDefinitionException(propertyInfo, "Missing Positional/Named argument attribute");
                            }
						}
					}
				}
			}
		}


		/// <summary>
		/// Parse arguments
		/// </summary>
		/// <param name="args"></param>
		public void Parse(string[] args)
		{
		    var enumerator = args.GetEnumerator();

			Parse(enumerator);
		}

	    /// <summary>
	    /// Parse command
	    /// </summary>
	    /// <param name="enumerator">Args enumerator</param>
	    /// <param name="more">Output parameter that indicates if there are more arguments to process</param>
	    public object ParseCommand(IEnumerator enumerator, out bool more)
	    {
	        try
	        {
	            Parse(enumerator);
                more = false;
	        }
	        catch (RetryableCommandLineArgumentException)
	        {
	            more = true;
	        }

	        CheckRequired();

	        var parameterClass = new T();
	        GetValues(parameterClass);
	        return parameterClass;
	    }

	    /// <summary>
	    /// Parse command line
	    /// </summary>
	    /// <param name="enumerator">Args enumerator</param>
	    public void Parse(IEnumerator enumerator)
	    {
            int positionalArgumentIndex = 0;

            Clear();

            while (enumerator.MoveNext())
	        {
InLoop:
	            var arg = (string) enumerator.Current;

	            // Named argument?
	            if (_argumentsSettings.IsNamedArgument(arg))
	            {
	                var namedArgument = _argumentsSettings.GetNamedArgument(arg);
	                IArgumentDefinition argumentDefinition;

	                // Try to lookup argument
	                if (_namedArguments.TryGetValue(namedArgument.ArgumentName, out argumentDefinition))
	                {
	                    var value = namedArgument.ArgumentValue;
	                    if (value == null && !argumentDefinition.IsBoolean)
	                    {
	                        if (_argumentsSettings.ValueSeparator.Any(x => x == ' '))
	                        {
	                            if (!enumerator.MoveNext())
	                                throw new CommandLineParserException("No value for " + namedArgument.ArgumentName);
	                            value = (string) enumerator.Current;
	                        }
	                    }

	                    if (argumentDefinition.IsMultiValue || (!argumentDefinition.HasValue))
	                        argumentDefinition.AddValue(value);
	                    else
	                    {
	                        switch (_argumentsSettings.DuplicateArgumentBehaviour)
	                        {
	                            case DuplicateArgumentBehaviour.Last:
	                                argumentDefinition.AddValue(value);
	                                break;
	                            case DuplicateArgumentBehaviour.First:
	                                break;
	                            case DuplicateArgumentBehaviour.Fail:
	                                throw new CommandLineParserException("Duplicate argument " + namedArgument.ArgumentName);
	                            case DuplicateArgumentBehaviour.Unknown:
	                                AddUnknownArgument(arg);
	                                break;
	                        }
	                    }
	                }
                    else
	                    AddUnknownArgument(arg);
	            }
	            else
	            {
	                ICommandLineParser commandParser;
	                if (_commandArgumentTypes.TryGetValue(arg, out commandParser))
	                {
	                    bool more;
	                    _commandArgument.AddRawValue(commandParser.ParseCommand(enumerator, out more));
	                    if (more) goto InLoop; // Hey! Look ma! A GOTO!!!
	                }
	                else
	                {
	                    // Positional argument?
	                    if (_positionalArguments.Count > positionalArgumentIndex)
	                    {
	                        IArgumentDefinition argumentDefinition =
	                            _positionalArguments.ElementAt(positionalArgumentIndex).Value;

	                        argumentDefinition.AddValue(arg);

	                        // Multivalue (array) positional argument definitions can recieve multiple values. Don't increment index.
	                        if (!argumentDefinition.IsMultiValue)
	                            positionalArgumentIndex++;
	                    }
	                    else
                            AddUnknownArgument(arg);
	                }
	            }
	        }
	    }


	    /// <summary>
		/// Check that required values has been specified
		/// </summary>
		public void CheckRequired()
		{
			foreach (var keyValue in _namedArguments)
				if (keyValue.Value.IsRequired && !keyValue.Value.HasValue)
					throw new CommandLineParserException("Required argument " + _argumentsSettings.StartOfArgument.First() + keyValue.Key);

			foreach (var keyValue in _positionalArguments)
				if (keyValue.Value.IsRequired && !keyValue.Value.HasValue)
					throw new CommandLineParserException("Required argument at position " + keyValue.Key);
            
            if(_commandArgument != null && _commandArgument.IsRequired && !_commandArgument.HasValue)
                throw new CommandLineParserException("One or more commands expected.");
		}

		/// <summary>
		/// Transfer parsed arguments into parameter 
		/// </summary>
		/// <param name="argumentClass">Instance of T that arguments are to be transfered to</param>
		public void GetValues(T argumentClass)
		{
		    foreach (var argument in GetAllArgumentDefinitions())
		    {
		        argument.GetValues(argumentClass);
		    }
		}

        /// <summary>
        /// Reset all arguments to default state
        /// </summary>
        public void Clear()
        {
            foreach (var argument in GetAllArgumentDefinitions())
            {
                argument.Clear();
            }
        }

	    private  IEnumerable<IArgumentDefinition> GetAllArgumentDefinitions()
	    {
	        foreach (IArgumentDefinition argument in _namedArguments.Values)
	            yield return argument;

	        foreach (IArgumentDefinition argument in _positionalArguments.Values)
                yield return argument;

	        if (_commandArgument != null)
                yield return _commandArgument;

	        if (_unknownArguments != null)
	            yield return _unknownArguments;
	    }

	    /// <summary>
		/// Add argument to list of unknown arguments, or throw exception if no unknown argument property has been defined
		/// </summary>
		/// <param name="arg"></param>
		private void AddUnknownArgument(string arg)
		{
			if (_unknownArguments != null)
				_unknownArguments.AddValue(arg);
			else
				throw new RetryableCommandLineArgumentException("Unknown argument " + arg);
		}
	}
}