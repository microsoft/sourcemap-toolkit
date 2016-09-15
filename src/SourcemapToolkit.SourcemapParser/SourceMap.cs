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
	}

	/// <summary>
	/// Corresponds to a single parsed entry in the source map mapping string
	/// </summary>
	public class MappingEntry
	{
		/// <summary>
		/// The zero-based line number in the generated code that corresponds to this mapping segment.
		/// </summary>
		public int GeneratedLineNumber;

		/// <summary>
		/// The zero-based column number in the generated code that corresponds to this mapping segment.
		/// </summary>
		public int GeneratedColumnNumber;

		/// <summary>
		/// The zero-based index into the sources array that corresponds to this mapping segment.
		/// </summary>
		public int? OriginalSourceFileIndex;

		/// <summary>
		/// The zero-based line number in the source code that corresponds to this mapping segment.
		/// </summary>
		public int? OriginalLineNumber;

		/// <summary>
		/// The zero-based line number in the source code that corresponds to this mapping segment.
		/// </summary>
		public int? OriginalColumnNumber;

		/// <summary>
		/// The zero-based index into the names array that can be used to identify names associated with this object.
		/// </summary>
		public int? OriginalNameIndex;
	}
}
