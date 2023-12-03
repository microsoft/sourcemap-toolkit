using System.Collections.Generic;
using System.IO;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.Mapping;

internal interface IFunctionMapGenerator
{
	/// <summary>
	/// Returns a FunctionMap describing the locations of every funciton in the source code.
	/// The functions are to be sorted in decreasing order by start position.
	/// </summary>
	IReadOnlyList<FunctionMapEntry> GenerateFunctionMap(StreamReader sourceCodeStreamReader, SourceMap sourceMap);
}