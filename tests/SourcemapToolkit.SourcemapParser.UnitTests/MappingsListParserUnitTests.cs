
using System;
using System.Collections.Generic;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class MappingsListParserUnitTests
	{
		[Fact]
		public void ParseSingleMappingSegment_NullSegmentFields_ThrowArgumentNullException()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			List<int> segmentFields = null;

			// Act
			Assert.Throws<ArgumentNullException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_0SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			var segmentFields = new List<int>();

			// Act
			Assert.Throws<ArgumentOutOfRangeException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_2SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			var segmentFields = new List<int> { 1, 2 };

			// Act
			Assert.Throws<ArgumentOutOfRangeException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_3SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			var segmentFields = new List<int> { 1, 2, 3 };

			// Act
			Assert.Throws<ArgumentOutOfRangeException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_NoPreviousStateSingleSegment_GeneratedColumnSet()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			var segmentFields = new List<int> { 16 };

			// Act 
			var result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.MappedLine);
			Assert.Equal(16, result.MappedColumn);
			Assert.False(result.SourceFileIndex.HasValue);
			Assert.False(result.SourceLine.HasValue);
			Assert.False(result.SourceColumn.HasValue);
			Assert.False(result.SourceNameIndex.HasValue);
		}

		[Fact]
		public void ParseSingleMappingSegment_NoPreviousState4Segments_OriginalNameIndexNotSetInMappingEntry()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			var segmentFields = new List<int> { 1, 1, 2, 4 };

			// Act 
			var result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.MappedLine);
			Assert.Equal(1, result.MappedColumn);
			Assert.Equal(1, result.SourceFileIndex);
			Assert.Equal(2, result.SourceLine);
			Assert.Equal(4, result.SourceColumn);
			Assert.False(result.SourceNameIndex.HasValue);
		}

		[Fact]
		public void ParseSingleMappingSegment_NoPreviousState5Segments_AllFieldsSetInMappingEntry()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState();
			var segmentFields = new List<int> { 1, 3, 6, 10, 15 };

			// Act 
			var result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.MappedLine);
			Assert.Equal(1, result.MappedColumn);
			Assert.Equal(3, result.SourceFileIndex);
			Assert.Equal(6, result.SourceLine);
			Assert.Equal(10, result.SourceColumn);
			Assert.Equal(15, result.SourceNameIndex);
		}

		[Fact]
		public void ParseSingleMappingSegment_HasPreviousState5Segments_AllFieldsSetIncrementally()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsParserState = new MappingsParserState(newGeneratedColumnBase: 6,
				newSourcesListIndexBase: 7,
				newOriginalSourceStartingLineBase: 8,
				newOriginalSourceStartingColumnBase: 9,
				newNamesListIndexBase: 10);
				var segmentFields = new List<int> { 1, 2, 3, 4, 5 };

			// Act 
			var result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.MappedLine);
			Assert.Equal(7, result.MappedColumn);
			Assert.Equal(9, result.SourceFileIndex);
			Assert.Equal(11, result.SourceLine);
			Assert.Equal(13, result.SourceColumn);
			Assert.Equal(15, result.SourceNameIndex);
		}

		[Fact]
		public void ParseMappings_SingleSemicolon_GeneratedLineNumberNotIncremented()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsString = "ktC,2iB;";
			var names = new List<string>();
			var sources = new List<string>();

			// Act
			var mappingsList = mappingsListParser.ParseMappings(mappingsString, names, sources);

			// Assert
			Assert.Equal(2, mappingsList.Count);
			Assert.Equal(0, mappingsList[0].SourceMapPosition.Line);
			Assert.Equal(0, mappingsList[1].SourceMapPosition.Line);
		}

		[Fact]
		public void ParseMappings_TwoSemicolons_GeneratedLineNumberIncremented()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsString = "ktC;2iB;";
			var names = new List<string>();
			var sources = new List<string>();

			// Act
			var mappingsList = mappingsListParser.ParseMappings(mappingsString, names, sources);

			// Assert
			Assert.Equal(2, mappingsList.Count);
			Assert.Equal(0, mappingsList[0].SourceMapPosition.Line);
			Assert.Equal(1, mappingsList[1].SourceMapPosition.Line);
		}

		[Fact]
		public void ParseMappings_BackToBackSemiColons_GeneratedLineNumberIncremented()
		{
			// Arrange
			var mappingsListParser = new MappingsListParser();
			var mappingsString = "ktC;;2iB";
			var names = new List<string>();
			var sources = new List<string>();

			// Act
			var mappingsList = mappingsListParser.ParseMappings(mappingsString, names, sources);

			// Assert
			Assert.Equal(2, mappingsList.Count);
			Assert.Equal(0, mappingsList[0].SourceMapPosition.Line);
			Assert.Equal(2, mappingsList[1].SourceMapPosition.Line);
		}
	}
}