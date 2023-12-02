using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal class FunctionMapConsumer : IFunctionMapConsumer
	{
		/// <summary>
		/// Finds the JavaScript function that wraps the given source location.
		/// </summary>
		/// <param name="sourcePosition">The location of the code around which we wish to find a wrapping function</param>
		/// <param name="functionMap">The function map, sorted in decreasing order by start source position, that represents the file containing the code of interest</param>
		public FunctionMapEntry GetWrappingFunctionForSourceLocation(SourcePosition sourcePosition, IReadOnlyList<FunctionMapEntry> functionMap)
		{
			foreach (var mapEntry in functionMap)
			{
				if (mapEntry.StartSourcePosition < sourcePosition && mapEntry.EndSourcePosition > sourcePosition)
				{
					return mapEntry;
				}
			}

			return null;
		}
	}
}
