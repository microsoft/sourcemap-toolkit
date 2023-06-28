﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SourcemapToolkit.SourcemapParser
{

	/// <summary>
	/// Class to track the internal state during source map serialize
	/// </summary>
	internal class MappingGenerateState
	{
		/// <summary>
		/// Last location of the code in the transformed code
		/// </summary>
		public SourcePosition LastGeneratedPosition { get; private set; } = default(SourcePosition);

		/// <summary>
		/// Last location of the code in the source code
		/// </summary>
		public SourcePosition LastOriginalPosition { get; set; } = default(SourcePosition);

		/// <summary>
		/// List that contains the symbol names
		/// </summary>
		public readonly IReadOnlyList<string> Names;

		/// <summary>
		/// List that contains the file sources
		/// </summary>
		public readonly IReadOnlyList<string> Sources;

		/// <summary>
		/// Index of last file source
		/// </summary>
		public int LastSourceIndex { get; set; }

		/// <summary>
		/// Index of last symbol name
		/// </summary>
		public int LastNameIndex { get; set; }

		/// <summary>
		/// Whether this is the first segment in current line
		/// </summary>
		public bool IsFirstSegment { get; set; }

		public MappingGenerateState(IReadOnlyList<string> names, IReadOnlyList<string> sources)
		{
			Names = names;
			Sources = sources;
			IsFirstSegment = true;
		}

		public void AdvanceLastGeneratedPositionLine()
		{
			LastGeneratedPosition = new SourcePosition(
				zeroBasedLineNumber: LastGeneratedPosition.ZeroBasedLineNumber + 1,
				zeroBasedColumnNumber: 0);
		}

		public void UpdateLastGeneratedPositionColumn(int zeroBasedColumnNumber)
		{
			LastGeneratedPosition = new SourcePosition(
				zeroBasedLineNumber: LastGeneratedPosition.ZeroBasedLineNumber,
				zeroBasedColumnNumber: zeroBasedColumnNumber);
		}
	}

	public class SourceMapGenerator
	{
        /// <summary>
        /// Convenience wrapper around SerializeMapping, but returns a base 64 encoded string instead
        /// </summary>
        public string GenerateSourceMapInlineComment(SourceMap sourceMap, JsonSerializerSettings jsonSerializerSettings = null)
        {
            string mappings = SerializeMapping(sourceMap, jsonSerializerSettings);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(mappings);
            var encoded = Convert.ToBase64String(bytes);

            return @"//# sourceMappingURL=data:application/json;base64," + encoded;
        }


        /// <summary>
        /// Serialize SourceMap object to json string with given serialize settings
        /// </summary>
        public string SerializeMapping(SourceMap sourceMap, JsonSerializerSettings jsonSerializerSettings = null)
		{
			if (sourceMap == null)
			{
				throw new ArgumentNullException(nameof(sourceMap));
			}

			string mappings = null;
			if (sourceMap.ParsedMappings != null && sourceMap.ParsedMappings.Count > 0)
			{
				MappingGenerateState state = new MappingGenerateState(sourceMap.Names, sourceMap.Sources);
				StringBuilder output = new StringBuilder();

				foreach (MappingEntry entry in sourceMap.ParsedMappings)
				{
					SerializeMappingEntry(entry, state, output);
				}

				output.Append(';');

				mappings = output.ToString();
			}

			SourceMap mapToSerialize = new SourceMap(
				version: sourceMap.Version,
				file: sourceMap.File,
				mappings: mappings,
				sources: sourceMap.Sources,
				names: sourceMap.Names,
				parsedMappings: default(IReadOnlyList<MappingEntry>),
				sourcesContent: sourceMap.SourcesContent);

			return JsonConvert.SerializeObject(mapToSerialize,
				jsonSerializerSettings ?? new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver(),
					NullValueHandling = NullValueHandling.Ignore,
				});
		}

		/// <summary>
		/// Convert each mapping entry to VLQ encoded segments
		/// </summary>
		internal void SerializeMappingEntry(MappingEntry entry, MappingGenerateState state, StringBuilder output)
		{
			if (state.LastGeneratedPosition.ZeroBasedLineNumber > entry.GeneratedSourcePosition.ZeroBasedLineNumber)
			{
				throw new InvalidOperationException($"Invalid sourmap detected. Please check the line {entry.GeneratedSourcePosition.ZeroBasedLineNumber}");
			}
			
			// Each line of generated code is separated using semicolons
			while (entry.GeneratedSourcePosition.ZeroBasedLineNumber != state.LastGeneratedPosition.ZeroBasedLineNumber)
			{
				state.AdvanceLastGeneratedPositionLine();
				state.IsFirstSegment = true;
				output.Append(';');
			}

			// The V3 source map format calls for all Base64 VLQ segments to be seperated by commas.
			if (!state.IsFirstSegment)
				output.Append(',');

			state.IsFirstSegment = false;

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

			Base64VlqEncoder.Encode(output, entry.GeneratedSourcePosition.ZeroBasedColumnNumber - state.LastGeneratedPosition.ZeroBasedColumnNumber);
			state.UpdateLastGeneratedPositionColumn(entry.GeneratedSourcePosition.ZeroBasedColumnNumber);

			if (entry.OriginalFileName != null)
			{
				int sourceIndex = state.Sources.IndexOf(entry.OriginalFileName);
				if (sourceIndex < 0)
				{
					throw new SerializationException("Source map contains original source that cannot be found in provided sources array");
				}

				Base64VlqEncoder.Encode(output, sourceIndex - state.LastSourceIndex);
				state.LastSourceIndex = sourceIndex;

				Base64VlqEncoder.Encode(output, entry.OriginalSourcePosition.ZeroBasedLineNumber - state.LastOriginalPosition.ZeroBasedLineNumber);
				Base64VlqEncoder.Encode(output, entry.OriginalSourcePosition.ZeroBasedColumnNumber - state.LastOriginalPosition.ZeroBasedColumnNumber);

				state.LastOriginalPosition = new SourcePosition(
					zeroBasedLineNumber: entry.OriginalSourcePosition.ZeroBasedLineNumber,
					zeroBasedColumnNumber: entry.OriginalSourcePosition.ZeroBasedColumnNumber);

				if (entry.OriginalName != null)
				{
					int nameIndex = state.Names.IndexOf(entry.OriginalName);
					if (nameIndex < 0)
					{
						throw new SerializationException("Source map contains original name that cannot be found in provided names array");
					}

					Base64VlqEncoder.Encode(output, nameIndex - state.LastNameIndex);
					state.LastNameIndex = nameIndex;
				}
			}
		}
	}
}
