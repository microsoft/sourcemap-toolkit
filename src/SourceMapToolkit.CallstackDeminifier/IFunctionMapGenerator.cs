using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IFunctionMapGenerator
	{
		/// <summary>
		/// Returns a FunctionMap describing the locations of every funciton in the source code.
		/// The functions are to be sorted in decreasing order by start position.
		/// </summary>
		List<FunctionMapEntry> GenerateFunctionMap(string sourceCode);
	}
}