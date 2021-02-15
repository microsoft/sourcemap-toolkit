using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
	public static class SourceMapTransformer
	{
		/// <summary>
		/// Removes column information from a source map
		/// This can significantly reduce the size of source maps
		/// If there is a tie between mapping entries, the first generated line takes priority
		/// <returns>A new source map</returns>
		/// </summary>
		public static SourceMap Flatten(SourceMap sourceMap)
		{
			HashSet<int> visitedLines = new HashSet<int>();
			List<MappingEntry> parsedMappings = new List<MappingEntry>(sourceMap.ParsedMappings.Count); // assume each line will not have been visited before

			foreach (MappingEntry mapping in sourceMap.ParsedMappings)
			{
				int generatedLine = mapping.GeneratedSourcePosition.ZeroBasedLineNumber;

				if (visitedLines.Add(generatedLine))
				{
					MappingEntry newMapping = mapping.CloneWithResetColumnNumber();
					parsedMappings.Add(newMapping);
				}
			}

			// Free-up any unneeded space.  This no-ops if we're already the right size.
			parsedMappings.Capacity = parsedMappings.Count;

			SourceMap newMap = new SourceMap(
				version: sourceMap.Version,
				file: sourceMap.File,
				mappings: sourceMap.Mappings,
				sources: sourceMap.Sources,
				names: sourceMap.Names,
				parsedMappings: parsedMappings,
				sourcesContent: sourceMap.SourcesContent);

			return newMap;
		}
	}
}
