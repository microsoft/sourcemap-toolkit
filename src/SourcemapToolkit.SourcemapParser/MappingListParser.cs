using System;
using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser
{
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

		public MappingsParserState(MappingsParserState previousMappingsParserState = new MappingsParserState(),
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
		/// <summary>
		/// Parses a single "segment" of the mapping field for a source map. A segment describes one piece of code in the generated source.
		/// In the mapping string "AAaAA,CAACC;", AAaAA and CAACC are both segments. This method assumes the segments have already been decoded 
		/// from Base64 VLQ into a list of integers.
		/// </summary>
		/// <param name="segmentFields">The integer values for the segment fields</param>
		/// <param name="mappingsParserState">The current state of the state variables for the parser</param>
		/// <returns></returns>
		internal MappingEntry ParseSingleMappingSegment(List<int> segmentFields, MappingsParserState mappingsParserState)
		{
			if (segmentFields == null)
			{
				throw new ArgumentNullException(nameof(segmentFields));
			}

			if (segmentFields.Count == 0 || segmentFields.Count == 2 || segmentFields.Count == 3)
			{
				throw new ArgumentOutOfRangeException(nameof(segmentFields));
			}

			MappingEntry mappingEntry = new MappingEntry
			{
				GeneratedLineNumber = mappingsParserState.CurrentGeneratedLineNumber,
				GeneratedColumnNumber = mappingsParserState.CurrentGeneratedColumnBase + segmentFields[0]
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
				mappingEntry.OriginalSourceFileIndex = mappingsParserState.SourcesListIndexBase + segmentFields[1];
				mappingEntry.OriginalLineNumber = mappingsParserState.OriginalSourceStartingLineBase + segmentFields[2];
				mappingEntry.OriginalColumnNumber = mappingsParserState.OriginalSourceStartingColumnBase + segmentFields[3];
			}

			if (segmentFields.Count >= 5)
			{
				mappingEntry.OriginalNameIndex = mappingsParserState.NamesListIndexBase + segmentFields[4];
			}

			return mappingEntry;
		}

		/// <summary>
		/// Top level API that should be called for decoding the MappingsString element. It will convert the string containing Base64 
		/// VLQ encoded segments into a list of MappingEntries.
		/// </summary>
		internal List<MappingEntry> ParseMappings(string mappingString)
		{
			List<MappingEntry> mappingEntries = new List<MappingEntry>();
			MappingsParserState currentMappingsParserState = new MappingsParserState();

			// The V3 source map format calls for all Base64 VLQ segments to be seperated by commas.
			// Each line of generated code is separated using semicolons. The count of semicolons encountered gives the current line number.
			string[] lines = mappingString.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

			for (int lineNumber = 0; lineNumber < lines.Length; lineNumber += 1)
			{
				// The only value that resets when encountering a semicolon is the starting column.
				currentMappingsParserState = new MappingsParserState(currentMappingsParserState, newGeneratedLineNumber:lineNumber, newGeneratedColumnBase: 0);
				string[] segmentsForLine = lines[lineNumber].Split(',');

				foreach (string segment in segmentsForLine)
				{
					MappingEntry mappingEntry = ParseSingleMappingSegment(Base64VlqDecoder.Decode(segment), currentMappingsParserState);
					mappingEntries.Add(mappingEntry);

					// Update the current MappingParserState based on the generated MappingEntry
					currentMappingsParserState = new MappingsParserState(currentMappingsParserState,
						newGeneratedColumnBase: mappingEntry.GeneratedColumnNumber,
						newSourcesListIndexBase: mappingEntry.OriginalSourceFileIndex,
						newOriginalSourceStartingLineBase: mappingEntry.OriginalLineNumber,
						newOriginalSourceStartingColumnBase: mappingEntry.OriginalColumnNumber,
						newNamesListIndexBase: mappingEntry.OriginalNameIndex);
				}
			}
			return mappingEntries;
		}
	}
}
