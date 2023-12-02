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
		if (sourceMapStream == null)
		{
			return null;
		}
		using (var jsonTextReader = new JsonTextReader(sourceMapStream))
		{
			var serializer = new JsonSerializer();

			var deserializedSourceMap = serializer.Deserialize<SourceMapDeserializable>(jsonTextReader);

			// Since SourceMap is immutable we need to allocate a new one and copy over all the information
			var parsedMappings = _mappingsListParser.ParseMappings(deserializedSourceMap.Mappings, deserializedSourceMap.Names, deserializedSourceMap.Sources);

			// Resize to free unused memory
			RemoveExtraSpaceFromList(parsedMappings);
			RemoveExtraSpaceFromList(deserializedSourceMap.Sources);
			RemoveExtraSpaceFromList(deserializedSourceMap.Names);

			if (_removeSourcesContent && deserializedSourceMap.SourcesContent != null)
			{
				deserializedSourceMap.SourcesContent.Clear();
			}
			else
			{
				RemoveExtraSpaceFromList(deserializedSourceMap.SourcesContent);
			}

			var result = new SourceMap(
				version: deserializedSourceMap.Version,
				file: deserializedSourceMap.File,
				mappings: deserializedSourceMap.Mappings,
				sources: deserializedSourceMap.Sources,
				names: deserializedSourceMap.Names,
				parsedMappings: parsedMappings,
				sourcesContent: deserializedSourceMap.SourcesContent);

			sourceMapStream.Close();
			return result;
		}
	}

	private void RemoveExtraSpaceFromList<T>(List<T> list)
	{
		if (list != null)
		{
			list.Capacity = list.Count;
		}
	}
}