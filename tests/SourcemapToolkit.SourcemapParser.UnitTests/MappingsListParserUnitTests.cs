
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
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = null;

			// Act
			Assert.Throws<ArgumentNullException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_0SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int>();

			// Act
			Assert.Throws<ArgumentOutOfRangeException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_2SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 2 };

			// Act
			Assert.Throws<ArgumentOutOfRangeException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_3SegmentFields_ArgumentOutOfRangeException()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 2, 3 };

			// Act
			Assert.Throws<ArgumentOutOfRangeException>( () => mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState));
		}

		[Fact]
		public void ParseSingleMappingSegment_NoPreviousStateSingleSegment_GeneratedColumnSet()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 16 };

			// Act 
			NumericMappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.GeneratedLineNumber);
			Assert.Equal(16, result.GeneratedColumnNumber);
			Assert.False(result.OriginalSourceFileIndex.HasValue);
			Assert.False(result.OriginalLineNumber.HasValue);
			Assert.False(result.OriginalColumnNumber.HasValue);
			Assert.False(result.OriginalNameIndex.HasValue);
		}

		[Fact]
		public void ParseSingleMappingSegment_NoPreviousState4Segments_OriginalNameIndexNotSetInMappingEntry()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 1, 2, 4 };

			// Act 
			NumericMappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.GeneratedLineNumber);
			Assert.Equal(1, result.GeneratedColumnNumber);
			Assert.Equal(1, result.OriginalSourceFileIndex);
			Assert.Equal(2, result.OriginalLineNumber);
			Assert.Equal(4, result.OriginalColumnNumber);
			Assert.False(result.OriginalNameIndex.HasValue);
		}

		[Fact]
		public void ParseSingleMappingSegment_NoPreviousState5Segments_AllFieldsSetInMappingEntry()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 3, 6, 10, 15 };

			// Act 
			NumericMappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.GeneratedLineNumber);
			Assert.Equal(1, result.GeneratedColumnNumber);
			Assert.Equal(3, result.OriginalSourceFileIndex);
			Assert.Equal(6, result.OriginalLineNumber);
			Assert.Equal(10, result.OriginalColumnNumber);
			Assert.Equal(15, result.OriginalNameIndex);
		}

		[Fact]
		public void ParseSingleMappingSegment_HasPreviousState5Segments_AllFieldsSetIncrementally()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState(newGeneratedColumnBase: 6,
				newSourcesListIndexBase: 7,
				newOriginalSourceStartingLineBase: 8,
				newOriginalSourceStartingColumnBase: 9,
				newNamesListIndexBase: 10);
				List <int> segmentFields = new List<int> { 1, 2, 3, 4, 5 };

			// Act 
			NumericMappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.Equal(0, result.GeneratedLineNumber);
			Assert.Equal(7, result.GeneratedColumnNumber);
			Assert.Equal(9, result.OriginalSourceFileIndex);
			Assert.Equal(11, result.OriginalLineNumber);
			Assert.Equal(13, result.OriginalColumnNumber);
			Assert.Equal(15, result.OriginalNameIndex);
		}

		[Fact]
		public void ParseMappings_SingleSemicolon_GeneratedLineNumberNotIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "ktC,2iB;";
			List<string> names = new List<string>();
			List<string> sources = new List<string>();

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString, names, sources);

			// Assert
			Assert.Equal(2, mappingsList.Count);
			Assert.Equal(0, mappingsList[0].GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, mappingsList[1].GeneratedSourcePosition.ZeroBasedLineNumber);
		}

		[Fact]
		public void ParseMappings_TwoSemicolons_GeneratedLineNumberIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "ktC;2iB;";
			List<string> names = new List<string>();
			List<string> sources = new List<string>();

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString, names, sources);

			// Assert
			Assert.Equal(2, mappingsList.Count);
			Assert.Equal(0, mappingsList[0].GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(1, mappingsList[1].GeneratedSourcePosition.ZeroBasedLineNumber);
		}

		[Fact]
		public void ParseMappings_BackToBackSemiColons_GeneratedLineNumberIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "ktC;;2iB";
			List<string> names = new List<string>();
			List<string> sources = new List<string>();

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString, names, sources);

			// Assert
			Assert.Equal(2, mappingsList.Count);
			Assert.Equal(0, mappingsList[0].GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(2, mappingsList[1].GeneratedSourcePosition.ZeroBasedLineNumber);
		}
	}
}