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
		/// Parses a string representing a source map into a SourceMap object.
		/// </summary>
		public SourceMap ParseSourceMap(string sourceMapString)
		{
			SourceMap result = JsonConvert.DeserializeObject<SourceMap>(sourceMapString);
			result.ParsedMappings = _mappingsListParser.ParseMappings(result.Mappings, result.Names, result.Sources);
			return result;
		}
	}
}
