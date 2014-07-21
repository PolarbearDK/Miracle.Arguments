namespace Miracle.Arguments
{
	/// <summary>
	/// Describe how parser should react to duplicate named arguments that are not multivalue.
	/// </summary>
	public enum DuplicateArgumentBehaviour
	{
		/// <summary>
		/// Use last value specified
		/// </summary>
		Last,
		/// <summary>
		/// Use first value specified
		/// </summary>
		First,
		/// <summary>
		/// Fail on duplicate arguments
		/// </summary>
		Fail,
		/// <summary>
		/// Treat as unknown argument
		/// </summary>
		Unknown,
	}
}