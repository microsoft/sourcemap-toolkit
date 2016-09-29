using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IFunctionMapStore
	{
		List<FunctionMapEntry> GetFunctionMapForSourceCode(string sourceCodeUrl);
	}
}