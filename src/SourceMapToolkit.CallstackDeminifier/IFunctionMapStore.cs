using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IFunctionMapStore
	{
		IReadOnlyList<FunctionMapEntry> GetFunctionMapForSourceCode(string sourceCodeUrl);
	}
}