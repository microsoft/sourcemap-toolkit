using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IFunctionMapConsumer
	{
		/// <summary>
		/// Finds the JavaScript function that wraps the given source location.
		/// </summary>
		/// <param name="sourcePosition">The location of the code around which we wish to find a wrapping function</param>
		/// <param name="functionMap">The sorted function map that represents the file containing the code of interest</param>
		FunctionMapEntry GetWrappingFunctionForSourceLocation(SourcePosition sourcePosition, List<FunctionMapEntry> functionMap);
	}
}