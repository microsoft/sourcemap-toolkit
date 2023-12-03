using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier.Mapping;

internal interface IFunctionMapStore
{
	IReadOnlyList<FunctionMapEntry> GetFunctionMapForSourceCode(string sourceCodeUrl);
}