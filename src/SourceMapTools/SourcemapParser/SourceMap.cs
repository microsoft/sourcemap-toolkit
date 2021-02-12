using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SourcemapToolkit.SourcemapParser
{
	public class SourceMap
	{
		/// <summary>
		/// The version of the source map specification being used
		/// </summary>
		[JsonPropertyName("version")]
		public int Version { get; set; }

		/// <summary>
		/// The name of the generated file to which this source map corresponds
		/// </summary>
		[JsonPropertyName("file")]
		public string? File { get; set; }

		/// <summary>
		/// The raw, unparsed mappings entry of the soure map
		/// </summary>
		[JsonPropertyName("mappings")]
		public string Mappings { get; set; } = null!;

		/// <summary>
		/// The list of source files that were the inputs used to generate this output file
		/// </summary>
		[JsonPropertyName("sources")]
		public List<string>? Sources { get; set; }

		/// <summary>
		/// A list of known original names for entries in this file
		/// </summary>
		[JsonPropertyName("names")]
		public List<string>? Names { get; set; }

		/// <summary>
		/// Parsed version of the mappings string that is used for getting original names and source positions
		/// </summary>
		[JsonIgnore]
		public List<MappingEntry> ParsedMappings { get; set; } = null!;

		/// <summary>
		/// A list of content source files
		/// </summary>
		public List<string>? SourcesContent { get; set; }

		public SourceMap Clone()
		{
			var sourceMap = new SourceMap
			{
				Version = Version,
				File = File,
				Mappings = Mappings,
				Sources = new List<string>(Sources),
				Names = new List<string>(Names),
				SourcesContent = new List<string>(SourcesContent)
			};

			sourceMap.ParsedMappings.AddRange(ParsedMappings.Select(m => m.Clone()));

			return sourceMap;
		}

		/// <summary>
		/// Applies the mappings of a sub source map to the current source map
		/// Each mapping to the supplied source file is rewritten using the supplied source map
		/// This is useful in situations where we have a to b to c, with mappings ba.map and cb.map
		/// Calling cb.ApplySourceMap(ba) will return mappings from c to a (ca)
		/// <param name="submap">The submap to apply</param>
		/// <param name="sourceFile">The filename of the source file. If not specified, submap's File property will be used</param>
		/// <returns>A new source map</returns>
		/// </summary>
		public SourceMap ApplySourceMap(SourceMap submap, string? sourceFile = null)
		{
			if (submap == null)
			{
				throw new ArgumentNullException(nameof(submap));
			}

			if (sourceFile == null)
			{
				if (submap.File == null)
				{
					throw new Exception("ApplySourceMap expects either the explicit source file to the map, or submap's 'file' property");
				}

				sourceFile = submap.File;
			}

			var newSourceMap = new SourceMap
			{
				File = File,
				Version = Version,
				Sources = new List<string>(),
				Names = new List<string>(),
				SourcesContent = new List<string>(),
				ParsedMappings = new List<MappingEntry>()
			};

			// transform mappings in this source map
			foreach (var mappingEntry in ParsedMappings)
			{
				var newMappingEntry = mappingEntry.Clone();

				if (mappingEntry.OriginalFileName == sourceFile && mappingEntry.OriginalSourcePosition != null)
				{
					var correspondingSubMapMappingEntry = submap.GetMappingEntryForGeneratedSourcePosition(mappingEntry.OriginalSourcePosition);

					if (correspondingSubMapMappingEntry != null)
					{
						// Copy the mapping
						newMappingEntry = new MappingEntry
						{
							GeneratedSourcePosition = mappingEntry.GeneratedSourcePosition.Clone(),
							OriginalSourcePosition = correspondingSubMapMappingEntry.OriginalSourcePosition.Clone(),
							OriginalName = correspondingSubMapMappingEntry.OriginalName ?? mappingEntry.OriginalName,
							OriginalFileName = correspondingSubMapMappingEntry.OriginalFileName ?? mappingEntry.OriginalFileName
						};
					}
				}

				// Copy into "Sources" and "Names"
				var originalFileName = newMappingEntry.OriginalFileName;
				var originalName = newMappingEntry.OriginalName;

				if (originalFileName != null && !newSourceMap.Sources.Contains(originalFileName))
				{
					newSourceMap.Sources.Add(originalFileName);
				}

				if (originalName != null && !newSourceMap.Names.Contains(originalName))
				{
					newSourceMap.Names.Add(originalName);
				}

				newSourceMap.ParsedMappings.Add(newMappingEntry);
			};

			return newSourceMap;
		}

		/// <summary>
		/// Finds the mapping entry for the generated source position. If no exact match is found, it will attempt
		/// to return a nearby mapping that should map to the same piece of code.
		/// </summary>
		/// <param name="generatedSourcePosition">The location in generated code for which we want to discover a mapping entry</param>
		/// <returns>A mapping entry that is a close match for the desired generated code location</returns>
		public virtual MappingEntry? GetMappingEntryForGeneratedSourcePosition(SourcePosition generatedSourcePosition)
		{
			if (ParsedMappings == null)
			{
				return null;
			}

			var mappingEntryToFind = new MappingEntry
			{
				GeneratedSourcePosition = generatedSourcePosition
			};

			var index = ParsedMappings.BinarySearch(mappingEntryToFind,
				Comparer<MappingEntry>.Create((a, b) => a.GeneratedSourcePosition.CompareTo(b.GeneratedSourcePosition)));

			// If we didn't get an exact match, let's try to return the closest piece of code to the given line
			if (index < 0)
			{
				// The BinarySearch method returns the bitwise complement of the nearest element that is larger than the desired element when there isn't a match.
				// Based on tests with source maps generated with the Closure Compiler, we should consider the closest source position that is smaller than the target value when we don't have a match.
				if (~index - 1 >= 0 && ParsedMappings[~index - 1].GeneratedSourcePosition.IsEqualish(generatedSourcePosition))
				{
					index = ~index - 1;
				}
			}

			return index >= 0 ? ParsedMappings[index] : null;
		}
	}
}
