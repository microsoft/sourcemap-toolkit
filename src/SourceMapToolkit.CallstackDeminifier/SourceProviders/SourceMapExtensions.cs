using System;
using System.Collections.Generic;
using System.Linq;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.SourceProviders;

internal static class SourceMapExtensions
{
	/// <summary>
	/// Gets the original name corresponding to a function based on the information provided in the source map.
	/// </summary>
	internal static string GetDeminifiedMethodName(this SourceMap sourceMap, IReadOnlyList<BindingInformation> bindings)
	{
        ArgumentNullException.ThrowIfNull(sourceMap);

        string methodName = null;

		if (bindings != null && bindings.Count > 0)
		{
			var prototypeMappingEntry = bindings.Count == 2 ? sourceMap.GetMappingEntryForGeneratedPosition(bindings[0].SourcePosition) : null;
			var mappingEntry = sourceMap.GetMappingEntryForGeneratedPosition(bindings.Last().SourcePosition);

			if (mappingEntry?.OriginalName != null)
			{
				if (prototypeMappingEntry?.OriginalName != null)
				{
					var objectName = prototypeMappingEntry.Value.OriginalName;
					if (prototypeMappingEntry.Value.SourcePosition.Column == mappingEntry.Value.SourcePosition.Column
					    && prototypeMappingEntry.Value.SourcePosition.Line == mappingEntry.Value.SourcePosition.Line
					    && objectName.Length > mappingEntry.Value.OriginalName.Length
					    && objectName.EndsWith(mappingEntry.Value.OriginalName)
					    && (objectName[objectName.Length - 1 - mappingEntry.Value.OriginalName.Length] == '.'))
					{
						// The object name already contains the method name, so do not append it
						methodName = objectName;
					}
					else
					{
						methodName = $"{objectName}.{mappingEntry.Value.OriginalName}";
					}
				}
				else
				{
					methodName = mappingEntry.Value.OriginalName;
				}
			}
		}
		return methodName;
	}
}