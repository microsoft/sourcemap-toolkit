using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Contains information regarding the location of a particular function in a JavaScript file
	/// </summary>
	internal class FunctionMapEntry
	{
		public FunctionMapEntry(
			List<BindingInformation> bindings,
			SourcePosition startSourcePosition,
			SourcePosition endSourcePosition)
		{
			Bindings = bindings;
			StartSourcePosition = startSourcePosition;
			EndSourcePosition = endSourcePosition;
		}

		/// <summary>
		/// A list of bindings that are associated with this function map entry.
		/// To get the complete name of the function associated with this mapping entry
		/// append the names of each bindings with a "."
		/// </summary>
		public IReadOnlyList<BindingInformation> Bindings { get; }

		/// <summary>
		/// If this entry represents a function whose name was minified, this value 
		/// may contain an associated deminfied name corresponding to the function.
		/// </summary>
		public string? DeminfifiedMethodName { get; internal set; }

		/// <summary>
		/// Denotes the location of the beginning of this function
		/// </summary>
		public SourcePosition StartSourcePosition { get; }

		/// <summary>
		/// Denotes the end location of this function
		/// </summary>
		public SourcePosition EndSourcePosition { get; }
	}
}
