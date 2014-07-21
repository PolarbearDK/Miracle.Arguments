using System;
using System.Runtime.Serialization;

namespace Miracle.Arguments
{
    public class RetryableCommandLineArgumentException : CommandLineParserException
    {
        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        public RetryableCommandLineArgumentException()
        {
        }

        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        public RetryableCommandLineArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        public RetryableCommandLineArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Constructor (overloaded)
        /// </summary>
        protected RetryableCommandLineArgumentException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}