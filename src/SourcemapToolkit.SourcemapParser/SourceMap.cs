using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SourcemapToolkit.SourcemapParser.Extensions;

namespace SourcemapToolkit.SourcemapParser;

/// <summary>
/// A seemingly silly class, but unfortunately one which appears necessary.
/// While the SourceMap class is serializable, it is not deserializable directly.
/// The problem is that the IReadOnlyList generics throw the following exception:
/// 'Newtonsoft.Json.JsonSerializationException: Cannot create and populate list type'
/// when attempting to deserialize from a stream using a JsonTextReader.  This
/// intermediary class mitigates the problem by reading the data into a mutable list
/// first.
/// </summary>
internal class SourceMapDeserializable
{
	/// <summary><see cref="SourceMap.Version"/></summary>
	public int Version;

	/// <summary><see cref="SourceMap.File"/></summary>
	public string File;

	/// <summary><see cref="SourceMap.Mappings"/></summary>
	public string Mappings;

	/// <summary><see cref="SourceMap.Sources"/></summary>
	public List<string> Sources;

	/// <summary><see cref="SourceMap.Names"/></summary>
	public List<string> Names;

	/// <summary><see cref="SourceMap.SourcesContent"/></summary>
	public List<string> SourcesContent;
}

public class SourceMap
{
	/// <summary>
	///  Cache the comparer to save the allocation
	/// </summary>
	[JsonIgnore]
	private static Comparer<MappingEntry> _comparer = Comparer<MappingEntry>.Create((a, b) => a.SourceMapPosition.CompareTo(b.SourceMapPosition));

	/// <summary>
	/// The version of the source map specification being used
	/// </summary>
	public int Version { get; }

	/// <summary>
	/// The name of the generated file to which this source map corresponds
	/// </summary>
	public string File { get; }

	/// <summary>
	/// The raw, unparsed mappings entry of the soure map
	/// </summary>
	public string Mappings { get; }

	/// <summary>
	/// The list of source files that were the inputs used to generate this output file
	/// </summary>
	public IReadOnlyList<string> Sources { get; }

	/// <summary>
	/// A list of known original names for entries in this file
	/// </summary>
	public IReadOnlyList<string> Names { get; }

	/// <summary>
	/// Parsed version of the mappings string that is used for getting original names and source positions
	/// </summary>
	/// <remarks>Marked as JsonIgnore to because it isn't part of the actual format.  See: https://sourcemaps.info/spec.html#h.mofvlxcwqzej </remarks>
	[JsonIgnore]
	public IReadOnlyList<MappingEntry> ParsedMappings { get; }

	/// <summary>
	/// A list of content source files
	/// </summary>
	public IReadOnlyList<string> SourcesContent { get; }
		
	public SourceMap(
		int version,
		string file,
		string mappings,
		IReadOnlyList<string> sources,
		IReadOnlyList<string> names,
		IReadOnlyList<MappingEntry> parsedMappings,
		IReadOnlyList<string> sourcesContent)
	{
		Version = version;
		File = file;
		Mappings = mappings;
		Sources = sources;
		Names = names;
		ParsedMappings = parsedMappings;
		SourcesContent = sourcesContent;
	}

	public SourceMap Clone()
	{
		return new(
			version: Version,
			file: File,
			mappings: Mappings,
			sources: Sources,
			names: Names,
			parsedMappings: ParsedMappings,
			sourcesContent: SourcesContent);
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
	public SourceMap ApplySourceMap(SourceMap submap, string sourceFile = null)
	{
		if (submap == null)
		{
			throw new ArgumentNullException(nameof(submap));
		}

		if (sourceFile == null)
		{
			if (submap.File == null)
			{
				throw new($"{nameof(ApplySourceMap)} expects either the explicit source file to the map, or submap's 'file' property");
			}

			sourceFile = submap.File;
		}

		var sources = new HashSet<string>(StringComparer.Ordinal);
		var names = new HashSet<string>(StringComparer.Ordinal);
		var parsedMappings = new List<MappingEntry>(ParsedMappings.Count);

		// transform mappings in this source map
		foreach (var mappingEntry in ParsedMappings)
		{
			var newMappingEntry = mappingEntry;

			if (mappingEntry.OriginalFileName == sourceFile && mappingEntry.SourcePosition != SourcePosition.NotFound)
			{
				var correspondingSubMapMappingEntry = submap.GetMappingEntryForGeneratedPosition(mappingEntry.SourcePosition);

				if (correspondingSubMapMappingEntry != null)
				{
					// Copy the mapping
					newMappingEntry = new(
						sourceMapPosition: mappingEntry.SourceMapPosition,
						originalSourcePosition: correspondingSubMapMappingEntry.Value.SourcePosition,
						originalName: correspondingSubMapMappingEntry.Value.OriginalName ?? mappingEntry.OriginalName,
						originalFileName: correspondingSubMapMappingEntry.Value.OriginalFileName ?? mappingEntry.OriginalFileName);
				}
			}

			// Copy into "Sources" and "Names"
			var originalFileName = newMappingEntry.OriginalFileName;
			var originalName = newMappingEntry.OriginalName;

			if (originalFileName != null)
			{
				sources.Add(originalFileName);
			}

			if (originalName != null)
			{
				names.Add(originalName);
			}

			parsedMappings.Add(newMappingEntry);
		}

		var newSourceMap = new SourceMap(
			version: Version,
			file: File,
			mappings: default,
			sources: sources.ToList(),
			names: names.ToList(),
			parsedMappings: parsedMappings,
			sourcesContent: new List<string>());

		return newSourceMap;
	}

	/// <summary>
	/// Finds the mapping entry for the generated source position. If no exact match is found, it will attempt
	/// to return a nearby mapping that should map to the same piece of code.
	/// </summary>
	/// <param name="generatedSourcePosition">The location in generated code for which we want to discover a mapping entry</param>
	/// <returns>A mapping entry that is a close match for the desired generated code location</returns>
	public virtual MappingEntry? GetMappingEntryForGeneratedPosition(SourcePosition generatedSourcePosition)
	{
		if (ParsedMappings == null)
		{
			return null;
		}

		var mappingEntryToFind = new MappingEntry(sourceMapPosition: generatedSourcePosition);

		var index = ParsedMappings.BinarySearch(mappingEntryToFind, _comparer);

		// If we didn't get an exact match, let's try to return the closest piece of code to the given line
		if (index < 0)
		{
			// The BinarySearch method returns the bitwise complement of the nearest element that is larger than the desired element when there isn't a match.
			// Based on tests with source maps generated with the Closure Compiler, we should consider the closest source position that is smaller than the target value when we don't have a match.
			var correctIndex = ~index - 1;

			if (correctIndex >= 0 && ParsedMappings[correctIndex].SourceMapPosition.IsEqualish(generatedSourcePosition))
			{
				index = correctIndex;
			}
		}

		return index >= 0 ? (MappingEntry?)ParsedMappings[index] : null;
	}
}