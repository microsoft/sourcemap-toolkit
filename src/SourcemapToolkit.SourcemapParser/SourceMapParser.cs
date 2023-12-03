using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace SourcemapToolkit.SourcemapParser;

public class SourceMapParser
{
	private readonly MappingsListParser _mappingsListParser;
	private readonly bool _removeSourcesContent;

	public SourceMapParser(bool removeSourcesContent = false)
	{
		_mappingsListParser = new();
		_removeSourcesContent = removeSourcesContent;
	}

	/// <summary>
	/// Parses a stream representing a source map into a SourceMap object.
	/// </summary>
	public SourceMap ParseSourceMap(StreamReader sourceMapStream)
	{
		if (sourceMapStream == null) return  null;

		using var jsonTextReader = new JsonTextReader(sourceMapStream);
		var sourceMap = new JsonSerializer().Deserialize<SourceMapDeserializable>(jsonTextReader);

		// since SourceMap is immutable we need to allocate a new one and copy over all the information
		var mappings = _mappingsListParser.ParseMappings(sourceMap.Mappings, sourceMap.Names, sourceMap.Sources);

		// Resize to free unused memory
		RemoveExtraSpaceFromList(mappings);
		RemoveExtraSpaceFromList(sourceMap.Sources);
		RemoveExtraSpaceFromList(sourceMap.Names);

		if (_removeSourcesContent && sourceMap.SourcesContent != null)
		{
			sourceMap.SourcesContent.Clear();
		}
		else
		{
			RemoveExtraSpaceFromList(sourceMap.SourcesContent);
		}

		var result = new SourceMap(
			version: sourceMap.Version,
			file: sourceMap.File,
			mappings: sourceMap.Mappings,
			sources: sourceMap.Sources,
			names: sourceMap.Names,
			parsedMappings: mappings,
			sourcesContent: sourceMap.SourcesContent);

		sourceMapStream.Close();
		return result;
	}

	private void RemoveExtraSpaceFromList<T>(List<T> list)
	{
		if (list != null)
		{
			list.Capacity = list.Count;
		}
	}
}