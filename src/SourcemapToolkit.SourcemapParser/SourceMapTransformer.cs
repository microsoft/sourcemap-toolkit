using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser;

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
		var visitedLines = new HashSet<int>();
		var parsedMappings = new List<MappingEntry>(sourceMap.ParsedMappings.Count); // assume each line will not have been visited before

		foreach (var mapping in sourceMap.ParsedMappings)
		{
			var generatedLine = mapping.SourceMapPosition.Line;

			if (visitedLines.Add(generatedLine))
			{
				var newMapping = mapping.CloneWithResetColumnNumber();
				parsedMappings.Add(newMapping);
			}
		}

		// Free-up any unneeded space.  This no-ops if we're already the right size.
		parsedMappings.Capacity = parsedMappings.Count;

		var newMap = new SourceMap(
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