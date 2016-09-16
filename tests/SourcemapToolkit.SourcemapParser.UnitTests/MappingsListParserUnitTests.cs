using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class MappingsListParserUnitTests
	{
		[TestMethod]
		public void ParseSingleMappingSegment_NoPreviousStateSingleSegment_GeneratedColumnSet()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 16 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(16, result.GeneratedColumnNumber);
			Assert.IsFalse(result.OriginalSourceFileIndex.HasValue);
			Assert.IsFalse(result.OriginalLineNumber.HasValue);
			Assert.IsFalse(result.OriginalColumnNumber.HasValue);
			Assert.IsFalse(result.OriginalNameIndex.HasValue);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_NoPreviousState4Segments_OriginalNameIndexNotSet()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 1, 2, 4 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(1, result.GeneratedColumnNumber);
			Assert.AreEqual(1, result.OriginalSourceFileIndex);
			Assert.AreEqual(2, result.OriginalLineNumber);
			Assert.AreEqual(4, result.OriginalColumnNumber);
			Assert.IsFalse(result.OriginalNameIndex.HasValue);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_NoPreviousState5Segments_AllFieldsSet()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			List<int> segmentFields = new List<int> { 1, 3, 6, 10, 15 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(1, result.GeneratedColumnNumber);
			Assert.AreEqual(3, result.OriginalSourceFileIndex);
			Assert.AreEqual(6, result.OriginalLineNumber);
			Assert.AreEqual(10, result.OriginalColumnNumber);
			Assert.AreEqual(15, result.OriginalNameIndex);
		}

		[TestMethod]
		public void ParseSingleMappingSegment_HasPreviousState5Segments_AllFieldsSetIncrementally()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState
			{
				CurrentGeneratedColumnBase = 6,
				SourcesListIndexBase = 7,
				OriginalSourceStartingLineBase = 8,
				OriginalSourceStartingColumnBase = 9,
				NamesListIndexBase = 10

			};
			List<int> segmentFields = new List<int> { 1, 2, 3, 4, 5 };

			// Act 
			MappingEntry result = mappingsListParser.ParseSingleMappingSegment(segmentFields, mappingsParserState);

			// Assert
			Assert.AreEqual(0, result.GeneratedLineNumber);
			Assert.AreEqual(7, result.GeneratedColumnNumber);
			Assert.AreEqual(9, result.OriginalSourceFileIndex);
			Assert.AreEqual(11, result.OriginalLineNumber);
			Assert.AreEqual(13, result.OriginalColumnNumber);
			Assert.AreEqual(15, result.OriginalNameIndex);
		}

		[TestMethod]
		public void UpdateMappingParserStateAfterSegmentParse_NoPreviousState5Segments_AllStateFieldsInitialized()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState();
			MappingEntry mappingEntry = new MappingEntry
			{
				GeneratedColumnNumber = 1,
				OriginalSourceFileIndex = 2,
				OriginalLineNumber = 3,
				OriginalColumnNumber = 4,
				OriginalNameIndex = 5
			};

			// Act
			mappingsListParser.UpdateMappingParserStateAfterSegmentParse(mappingsParserState, mappingEntry);

			// Assert
			Assert.AreEqual(0, mappingsParserState.CurrentGeneratedLineNumber);
			Assert.AreEqual(1, mappingsParserState.CurrentGeneratedColumnBase);
			Assert.AreEqual(2, mappingsParserState.SourcesListIndexBase);
			Assert.AreEqual(3, mappingsParserState.OriginalSourceStartingLineBase);
			Assert.AreEqual(4, mappingsParserState.OriginalSourceStartingColumnBase);
			Assert.AreEqual(5, mappingsParserState.NamesListIndexBase);
		}

		[TestMethod]
		public void UpdateMappingParserStateAfterSegmentParse_HasPreviousState5Segments_AllStateFieldsOverridden()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState
			{
				CurrentGeneratedLineNumber = 20,
				CurrentGeneratedColumnBase = 6,
				SourcesListIndexBase = 7,
				OriginalSourceStartingLineBase = 8,
				OriginalSourceStartingColumnBase = 9,
				NamesListIndexBase = 10
			};
			MappingEntry mappingEntry = new MappingEntry
			{
				GeneratedColumnNumber = 1,
				OriginalSourceFileIndex = 2,
				OriginalLineNumber = 3,
				OriginalColumnNumber = 4,
				OriginalNameIndex = 5
			};

			// Act
			mappingsListParser.UpdateMappingParserStateAfterSegmentParse(mappingsParserState, mappingEntry);

			// Assert
			Assert.AreEqual(20, mappingsParserState.CurrentGeneratedLineNumber);
			Assert.AreEqual(1, mappingsParserState.CurrentGeneratedColumnBase);
			Assert.AreEqual(2, mappingsParserState.SourcesListIndexBase);
			Assert.AreEqual(3, mappingsParserState.OriginalSourceStartingLineBase);
			Assert.AreEqual(4, mappingsParserState.OriginalSourceStartingColumnBase);
			Assert.AreEqual(5, mappingsParserState.NamesListIndexBase);
		}

		[TestMethod]
		public void UpdateMappingParserStateAfterSegmentParse_HasPreviousState4Segments_NamesListIndexUnchanged()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			MappingsParserState mappingsParserState = new MappingsParserState
			{
				CurrentGeneratedLineNumber = 20,
				CurrentGeneratedColumnBase = 6,
				SourcesListIndexBase = 7,
				OriginalSourceStartingLineBase = 8,
				OriginalSourceStartingColumnBase = 9,
				NamesListIndexBase = 10
			};
			MappingEntry mappingEntry = new MappingEntry
			{
				GeneratedColumnNumber = 1,
				OriginalSourceFileIndex = 2,
				OriginalLineNumber = 3,
				OriginalColumnNumber = 4
			};

			// Act
			mappingsListParser.UpdateMappingParserStateAfterSegmentParse(mappingsParserState, mappingEntry);

			// Assert
			Assert.AreEqual(20, mappingsParserState.CurrentGeneratedLineNumber);
			Assert.AreEqual(1, mappingsParserState.CurrentGeneratedColumnBase);
			Assert.AreEqual(2, mappingsParserState.SourcesListIndexBase);
			Assert.AreEqual(3, mappingsParserState.OriginalSourceStartingLineBase);
			Assert.AreEqual(4, mappingsParserState.OriginalSourceStartingColumnBase);
			Assert.AreEqual(10, mappingsParserState.NamesListIndexBase);
		}

		[TestMethod]
		public void ParseMappings_SingleSemicolon_GeneratedLineNumberNotIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "AAaAA,CAACC;";

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString);

			// Assert
			Assert.AreEqual(2, mappingsList.Count);

			Assert.AreEqual(0, mappingsList[0].GeneratedLineNumber);
			Assert.AreEqual(0, mappingsList[0].GeneratedColumnNumber);
			Assert.AreEqual(0, mappingsList[0].OriginalSourceFileIndex);
			Assert.AreEqual(13, mappingsList[0].OriginalLineNumber);
			Assert.AreEqual(0, mappingsList[0].OriginalColumnNumber);
			Assert.AreEqual(0, mappingsList[0].OriginalNameIndex);

			Assert.AreEqual(0, mappingsList[1].GeneratedLineNumber);
			Assert.AreEqual(1, mappingsList[1].GeneratedColumnNumber);
			Assert.AreEqual(0, mappingsList[1].OriginalSourceFileIndex);
			Assert.AreEqual(13, mappingsList[1].OriginalLineNumber);
			Assert.AreEqual(1, mappingsList[1].OriginalColumnNumber);
			Assert.AreEqual(1, mappingsList[1].OriginalNameIndex);
		}

		[TestMethod]
		public void ParseMappings_TwoSemicolons_GeneratedLineNumberIncremented()
		{
			// Arrange
			MappingsListParser mappingsListParser = new MappingsListParser();
			string mappingsString = "CEGegB;CACf;";

			// Act
			List<MappingEntry> mappingsList = mappingsListParser.ParseMappings(mappingsString);

			// Assert
			Assert.AreEqual(2, mappingsList.Count);

			Assert.AreEqual(0, mappingsList[0].GeneratedLineNumber);
			Assert.AreEqual(1, mappingsList[0].GeneratedColumnNumber);
			Assert.AreEqual(2, mappingsList[0].OriginalSourceFileIndex);
			Assert.AreEqual(3, mappingsList[0].OriginalLineNumber);
			Assert.AreEqual(15, mappingsList[0].OriginalColumnNumber);
			Assert.AreEqual(16, mappingsList[0].OriginalNameIndex);

			Assert.AreEqual(1, mappingsList[1].GeneratedLineNumber);
			Assert.AreEqual(1, mappingsList[1].GeneratedColumnNumber);
			Assert.AreEqual(2, mappingsList[1].OriginalSourceFileIndex);
			Assert.AreEqual(4, mappingsList[1].OriginalLineNumber);
			Assert.AreEqual(0, mappingsList[1].OriginalColumnNumber);
			Assert.IsFalse(mappingsList[1].OriginalNameIndex.HasValue);
		}
	}
}