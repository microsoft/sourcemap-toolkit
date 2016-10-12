using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
	public class SourceMap
	{
		/// <summary>
		/// The version of the source map specification being used
		/// </summary>
		public int Version;

		/// <summary>
		/// The name of the generated file to which this source map corresponds
		/// </summary>
		public string File;

		/// <summary>
		/// The raw, unparsed mappings entry of the soure map
		/// </summary>
		public string Mappings;

		/// <summary>
		/// The list of source files that were the inputs used to generate this output file
		/// </summary>
		public List<string> Sources;

		/// <summary>
		/// A list of known original names for entries in this file
		/// </summary>
		public List<string> Names;

		/// <summary>
		/// Parsed version of the mappings string that is used for getting original names and source positions
		/// </summary>
		public List<MappingEntry> ParsedMappings;

		/// <summary>
		/// Finds the mapping entry for the generated source position. If no exact match is found, it will attempt 
		/// to return a nearby mapping that should map to the same piece of code.
		/// </summary>
		/// <param name="generatedSourcePosition">The location in generated code for which we want to discover a mapping entry</param>
		/// <returns>A mapping entry that is a close match for the desired generated code location</returns>
		public virtual MappingEntry GetMappingEntryForGeneratedSourcePosition(SourcePosition generatedSourcePosition)
		{
			if (ParsedMappings == null)
			{
				return null;
			}

			MappingEntry result = MappingEntryForGeneratedSourcePositionExact(generatedSourcePosition);

			// Attempt to find a nearby result if there is no exact match.
			if (result == null)
			{
				// The mappings from Google Closure Advanced mode often have column numbers that are off by 1 
				result =
					MappingEntryForGeneratedSourcePositionExact(new SourcePosition
					{
						ZeroBasedColumnNumber = generatedSourcePosition.ZeroBasedColumnNumber - 1,
						ZeroBasedLineNumber = generatedSourcePosition.ZeroBasedLineNumber
					});

			}

			return result;
		}

		private MappingEntry MappingEntryForGeneratedSourcePositionExact(SourcePosition generatedSourcePosition)
		{
			MappingEntry mappingEntryToFind = new MappingEntry
			{
				GeneratedSourcePosition = generatedSourcePosition
			};

			int index = ParsedMappings.BinarySearch(mappingEntryToFind,
				Comparer<MappingEntry>.Create((a, b) => a.GeneratedSourcePosition.CompareTo(b.GeneratedSourcePosition)));

			return index >= 0 ? ParsedMappings[index] : null;
		}
	}
}
