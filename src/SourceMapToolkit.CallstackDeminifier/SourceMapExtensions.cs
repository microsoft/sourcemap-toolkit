using System;
using System.Collections.Generic;
using System.Linq;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal static class SourceMapExtensions
	{
		/// <summary>
		/// Gets the original name corresponding to a function based on the information provided in the source map.
		/// </summary>
		internal static string GetDeminifiedMethodName(this SourceMap sourceMap, IReadOnlyList<BindingInformation> bindings)
		{
			if (sourceMap == null)
			{
				throw new ArgumentNullException(nameof(sourceMap));
			}

			string methodName = null;

			if (bindings != null && bindings.Count > 0)
			{
				MappingEntry objectProtoypeMappingEntry = null;
				if (bindings.Count == 2)
				{
					objectProtoypeMappingEntry =
						sourceMap.GetMappingEntryForGeneratedSourcePosition(bindings[0].SourcePosition);
				}

				MappingEntry mappingEntry =
					sourceMap.GetMappingEntryForGeneratedSourcePosition(bindings.Last().SourcePosition);

				if (mappingEntry?.OriginalName != null)
				{
					if (objectProtoypeMappingEntry?.OriginalName != null)
					{
						string objectName = objectProtoypeMappingEntry.OriginalName;
						if (objectProtoypeMappingEntry.OriginalSourcePosition.ZeroBasedColumnNumber == mappingEntry.OriginalSourcePosition.ZeroBasedColumnNumber
							&& objectProtoypeMappingEntry.OriginalSourcePosition.ZeroBasedLineNumber == mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber
							&& objectName.EndsWith($".{mappingEntry.OriginalName}"))
						{
							// The object name already contains the method name, so do not append it
							methodName = objectName;
						}
						else
						{
							methodName = $"{objectName}.{mappingEntry.OriginalName}";
						}
					}
					else
					{
						methodName = mappingEntry.OriginalName;
					}
				}
			}
			return methodName;
		}
	}
}
