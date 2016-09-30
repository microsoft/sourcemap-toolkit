using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Contains information regarding the location of a particular function in a JavaScript file
	/// </summary>
	internal class FunctionMapEntry
	{
		/// <summary>
		/// The name of the method
		/// </summary>
		public string FunctionName { get; set; }

		/// <summary>
		/// The location of the function name declaration
		/// </summary>
		public SourcePosition FunctionNameSourcePosition { get; set; }

		/// <summary>
		/// Denotes the location of the beginning of this function
		/// </summary>
		public SourcePosition StartSourcePosition { get; set; }

		/// <summary>
		/// Denotes the end location of this function
		/// </summary>
		public SourcePosition EndSourcePosition { get; set; }
	}
}
