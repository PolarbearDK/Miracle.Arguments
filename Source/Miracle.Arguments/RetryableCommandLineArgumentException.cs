using System;
using System.Runtime.Serialization;

namespace Miracle.Arguments
{
    /// <summary>
    /// Command line argument that can't be processed by current parser, but might be processed by another parser.
    /// </summary>
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