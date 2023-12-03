﻿using System;
using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser.Base64;
using SourcemapToolkit.SourcemapParser.Extensions;

namespace SourcemapToolkit.SourcemapParser;

/// <summary>
/// Corresponds to a single parsed entry in the source map mapping string that is used internally by the parser.
/// The public API exposes the MappingEntry object, which is more useful to consumers of the library.
/// </summary>
internal struct NumericMappingEntry
{
	/// <summary>
	/// The zero-based line number in the generated code that corresponds to this mapping segment.
	/// </summary>
	public int MappedLine;

	/// <summary>
	/// The zero-based column number in the generated code that corresponds to this mapping segment.
	/// </summary>
	public int MappedColumn;

	/// <summary>
	/// The zero-based index into the sources array that corresponds to this mapping segment.
	/// </summary>
	public int? SourceFileIndex;

	/// <summary>
	/// The zero-based line number in the source code that corresponds to this mapping segment.
	/// </summary>
	public int? SourceLine;

	/// <summary>
	/// The zero-based line number in the source code that corresponds to this mapping segment.
	/// </summary>
	public int? SourceColumn;

	/// <summary>
	/// The zero-based index into the names array that can be used to identify names associated with this object.
	/// </summary>
	public int? SourceNameIndex;

	public MappingEntry ToMappingEntry(IReadOnlyList<string> names, IReadOnlyList<string> sources)
	{
		SourcePosition originalSourcePosition;

		if (SourceColumn.HasValue && SourceLine.HasValue)
		{
			originalSourcePosition = new(
				line: SourceLine.Value,
				column: SourceColumn.Value);
		}
		else
		{
			originalSourcePosition = SourcePosition.NotFound;
		}
			
		string originalName = null;
		if (SourceNameIndex.HasValue)
		{
			try
			{
				originalName = names[SourceNameIndex.Value];
			}
			catch (IndexOutOfRangeException e)
			{
				throw new IndexOutOfRangeException("Source map contains original name index that is outside the range of the provided names array", e);
			}
		}

		string originalFileName = null;
		if (SourceFileIndex.HasValue)
		{
			try
			{
				originalFileName = sources[SourceFileIndex.Value];
			}
			catch (IndexOutOfRangeException e)
			{
				throw new IndexOutOfRangeException("Source map contains original source index that is outside the range of the provided sources array", e);
			}
		}

		var result = new MappingEntry(
			sourceMapPosition: new(
				line: MappedLine,
				column: MappedColumn),
			originalSourcePosition: originalSourcePosition,
			originalName: originalName,
			originalFileName: originalFileName);

		return result;
	}
}

/// <summary>
/// The various fields within a segment of the Mapping parser are relative to the previous value we parsed.
/// This class tracks this state throughout the parsing process. 
/// </summary>
internal struct MappingsParserState
{
	public readonly int CurrentGeneratedLineNumber;
	public readonly int CurrentGeneratedColumnBase;
	public readonly int SourcesListIndexBase;
	public readonly int OriginalSourceStartingLineBase;
	public readonly int OriginalSourceStartingColumnBase;
	public readonly int NamesListIndexBase;

	public MappingsParserState(
		MappingsParserState previousMappingsParserState = new(),
		int? newGeneratedLineNumber = null,
		int? newGeneratedColumnBase = null,
		int? newSourcesListIndexBase = null,
		int? newOriginalSourceStartingLineBase = null,
		int? newOriginalSourceStartingColumnBase = null,
		int? newNamesListIndexBase = null)
	{
		CurrentGeneratedLineNumber = newGeneratedLineNumber ?? previousMappingsParserState.CurrentGeneratedLineNumber;
		CurrentGeneratedColumnBase = newGeneratedColumnBase ?? previousMappingsParserState.CurrentGeneratedColumnBase;
		SourcesListIndexBase = newSourcesListIndexBase ?? previousMappingsParserState.SourcesListIndexBase;
		OriginalSourceStartingLineBase = newOriginalSourceStartingLineBase ?? previousMappingsParserState.OriginalSourceStartingLineBase;
		OriginalSourceStartingColumnBase = newOriginalSourceStartingColumnBase ?? previousMappingsParserState.OriginalSourceStartingColumnBase;
		NamesListIndexBase = newNamesListIndexBase ?? previousMappingsParserState.NamesListIndexBase;
	}
}

/// <summary>
/// One of the entries of the V3 source map is a base64 VLQ encoded string providing metadata about a particular line of generated code.
/// This class is responsible for converting this string into a more friendly format.
/// </summary>
internal class MappingsListParser
{
	private static readonly char[] LineDelimiter = new[] { ',' };

	/// <summary>
	/// Parses a single "segment" of the mapping field for a source map. A segment describes one piece of code in the generated source.
	/// In the mapping string "AAaAA,CAACC;", AAaAA and CAACC are both segments. This method assumes the segments have already been decoded 
	/// from Base64 VLQ into a list of integers.
	/// </summary>
	/// <param name="segmentFields">The integer values for the segment fields</param>
	/// <param name="mappingsParserState">The current state of the state variables for the parser</param>
	/// <returns></returns>
	internal NumericMappingEntry ParseSingleMappingSegment(IReadOnlyList<int> segmentFields, MappingsParserState mappingsParserState)
	{
		if (segmentFields == null)
		{
			throw new ArgumentNullException(nameof(segmentFields));
		}

		if (segmentFields.Count == 0 || segmentFields.Count == 2 || segmentFields.Count == 3)
		{
			throw new ArgumentOutOfRangeException(nameof(segmentFields));
		}

		var numericMappingEntry = new NumericMappingEntry()
		{
			MappedLine = mappingsParserState.CurrentGeneratedLineNumber,
			MappedColumn = mappingsParserState.CurrentGeneratedColumnBase + segmentFields[0]
		};

		/*
		 *	The following description was taken from the Sourcemap V3 spec https://docs.google.com/document/d/1U1RGAehQwRypUTovF1KRlpiOFze0b-_2gc6fAH0KY0k/mobilebasic?pref=2&pli=1
		 *  The Sourcemap V3 spec is under a Creative Commons Attribution-ShareAlike 3.0 Unported License. https://creativecommons.org/licenses/by-sa/3.0/
		 *
		 *  Each VLQ segment has 1, 4, or 5 variable length fields.
		 *  The fields in each segment are:
		 *  1. The zero-based starting column of the line in the generated code that the segment represents.
		 *     If this is the first field of the first segment, or the first segment following a new generated line(“;”),
		 *     then this field holds the whole base 64 VLQ.Otherwise, this field contains a base 64 VLQ that is relative to
		 *     the previous occurrence of this field.Note that this is different than the fields below because the previous
		 *     value is reset after every generated line.
		 *  2. If present, an zero - based index into the “sources” list.This field is a base 64 VLQ relative to the previous
		 *     occurrence of this field, unless this is the first occurrence of this field, in which case the whole value is represented.
		 *  3. If present, the zero-based starting line in the original source represented. This field is a base 64 VLQ relative to the
		 *     previous occurrence of this field, unless this is the first occurrence of this field, in which case the whole value is
		 *     represented.Always present if there is a source field.
		 *  4. If present, the zero - based starting column of the line in the source represented.This field is a base 64 VLQ relative to
		 *     the previous occurrence of this field, unless this is the first occurrence of this field, in which case the whole value is
		 *     represented.Always present if there is a source field.
		 *  5. If present, the zero - based index into the “names” list associated with this segment.This field is a base 64 VLQ relative
		 *     to the previous occurrence of this field, unless this is the first occurrence of this field, in which case the whole value
		 *     is represented.
		 */
		if (segmentFields.Count > 1)
		{
			numericMappingEntry.SourceFileIndex = mappingsParserState.SourcesListIndexBase + segmentFields[1];
			numericMappingEntry.SourceLine = mappingsParserState.OriginalSourceStartingLineBase + segmentFields[2];
			numericMappingEntry.SourceColumn = mappingsParserState.OriginalSourceStartingColumnBase + segmentFields[3];
		}

		if (segmentFields.Count >= 5)
		{
			numericMappingEntry.SourceNameIndex = mappingsParserState.NamesListIndexBase + segmentFields[4];
		}

		return numericMappingEntry;
	}

	/// <summary>
	/// Top level API that should be called for decoding the MappingsString element. It will convert the string containing Base64 
	/// VLQ encoded segments into a list of MappingEntries.
	/// </summary>
	internal List<MappingEntry> ParseMappings(string mappingString, IReadOnlyList<string> names, IReadOnlyList<string> sources)
	{
		var mappingEntries = new List<MappingEntry>();
		var currentMappingsParserState = new MappingsParserState();

		// The V3 source map format calls for all Base64 VLQ segments to be seperated by commas.
		// Each line of generated code is separated using semicolons. The count of semicolons encountered gives the current line number.
		var lines = mappingString.SplitFast(';');

		for (var lineNumber = 0; lineNumber < lines.Length; lineNumber++)
		{
			// The only value that resets when encountering a semicolon is the starting column.
			currentMappingsParserState = new(
				currentMappingsParserState,
				newGeneratedLineNumber: lineNumber,
				newGeneratedColumnBase: 0);

			var segmentsForLine = lines[lineNumber].Split(LineDelimiter, StringSplitOptions.RemoveEmptyEntries);

			foreach (var segment in segmentsForLine)
			{
				// Reuse the numericMappingEntry to ease GC allocations.
				var numericMappingEntry = ParseSingleMappingSegment(Base64VlqDecoder.Decode(segment), currentMappingsParserState);
				mappingEntries.Add(numericMappingEntry.ToMappingEntry(names, sources));

				// Update the current MappingParserState based on the generated MappingEntry
				currentMappingsParserState = new(
					currentMappingsParserState,
					newGeneratedColumnBase: numericMappingEntry.MappedColumn,
					newSourcesListIndexBase: numericMappingEntry.SourceFileIndex,
					newOriginalSourceStartingLineBase: numericMappingEntry.SourceLine,
					newOriginalSourceStartingColumnBase: numericMappingEntry.SourceColumn,
					newNamesListIndexBase: numericMappingEntry.SourceNameIndex);
			}
		}
		return mappingEntries;
	}
}