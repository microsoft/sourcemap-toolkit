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

	    public MappingEntry GetMappingEntryForGeneratedSourcePosition(SourcePosition generatedSourcePosition)
	    {
	        if (ParsedMappings == null)
	        {
	            return null;
	        }

            foreach (MappingEntry mappingEntry in ParsedMappings)
            {
                if (mappingEntry.GeneratedSourcePosition.CompareTo(generatedSourcePosition) == 0)
                {
                    return mappingEntry;
                }
            }

	        return null;
	    }
	}
}
