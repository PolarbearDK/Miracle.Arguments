namespace Miracle.Arguments
{
    /// <summary>
    /// Class representing a matching command including aliases
    /// </summary>
    public class CommandMatch
    {
        /// <summary>
        /// Matching command line parser
        /// </summary>
        public ICommandLineParser Parser { get; private set; }
        /// <summary>
        /// Aliases associated with command line parser
        /// </summary>
        public string[] Aliases { get; private set; }

        /// <summary>
        /// Construct CommandMatch object
        /// </summary>
        /// <param name="parser">Parser instance</param>
        /// <param name="aliases">Aliases associated with parser</param>
        public CommandMatch(ICommandLineParser parser, string[] aliases)
        {
            Parser = parser;
            Aliases = aliases;
        }
    }
}