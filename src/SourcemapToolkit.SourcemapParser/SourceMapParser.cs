using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;

namespace SourcemapToolkit.SourcemapParser
{
	public class SourceMapParser
	{
		private readonly MappingsListParser _mappingsListParser;

		public SourceMapParser()
		{
			_mappingsListParser = new MappingsListParser();
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
			using (JsonTextReader jsonTextReader = new JsonTextReader(sourceMapStream))
			{
				JsonSerializer serializer = new JsonSerializer();

				SourceMap result = serializer.Deserialize<SourceMap>(jsonTextReader);

				// Since SourceMap is immutable we need to allocate a new one and copy over all the information
				List<MappingEntry> parsedMappings = _mappingsListParser.ParseMappings(result.Mappings, result.Names, result.Sources);

				// Resize to free unused memory
				parsedMappings.Capacity = parsedMappings.Count;

				result = new SourceMap(
					version: result.Version,
					file: result.File,
					mappings: result.Mappings,
					sources: result.Sources,
					names: result.Names,
					parsedMappings: parsedMappings,
					sourcesContent: result.SourcesContent);

				sourceMapStream.Close();
				return result;
			}
		}
	}
}
