using System.Collections.Generic;
using Xunit;
using System;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourceMapUnitTests
	{
		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NullMappingList_ReturnNull()
		{
			// Arrange
			var sourceMap = CreateSourceMap();
			var sourcePosition = new SourcePosition(line: 4, column: 3);

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Null(result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoMatchingEntryInMappingList_ReturnNull()
		{
			// Arrange
			var parsedMappings = new List<MappingEntry>
			{
				new MappingEntry(
					sourceMapPosition: new SourcePosition(line: 0, column: 0))
			};

			var sourceMap = CreateSourceMap(parsedMappings: parsedMappings);
			var sourcePosition = new SourcePosition(line: 4, column: 3);

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Null(result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_MatchingEntryInMappingList_ReturnMatchingEntry()
		{
			// Arrange
			var matchingMappingEntry = new MappingEntry(
				sourceMapPosition: new SourcePosition(line: 8, column: 13));
			var parsedMappings = new List<MappingEntry>
			{
				new MappingEntry(
					sourceMapPosition: new SourcePosition(line: 0, column: 0)),
				matchingMappingEntry
			};

			var sourceMap = CreateSourceMap(parsedMappings: parsedMappings);
			var sourcePosition = new SourcePosition(line: 8, column: 13);

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnSameLine_ReturnSimilarEntry()
		{
			// Arrange
			var matchingMappingEntry = new MappingEntry(
				sourceMapPosition: new SourcePosition(line: 10, column: 13));
			var parsedMappings = new List<MappingEntry>
			{
				new MappingEntry(
					sourceMapPosition: new SourcePosition(line: 0, column: 0)),
				matchingMappingEntry
			};
			var sourceMap = CreateSourceMap(parsedMappings: parsedMappings);
			var sourcePosition = new SourcePosition(line: 10, column: 14);

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnDifferentLinesLine_ReturnSimilarEntry()
		{
			// Arrange
			var matchingMappingEntry = new MappingEntry(
				sourceMapPosition: new SourcePosition(line: 23, column: 15));
			var parsedMappings = new List<MappingEntry>
			{
				new MappingEntry(
					sourceMapPosition: new SourcePosition(line: 0, column: 0)),
				matchingMappingEntry
			};
			var sourceMap = CreateSourceMap(parsedMappings: parsedMappings);
			var sourcePosition = new SourcePosition(line: 24, column: 0);

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetRootMappingEntryForGeneratedSourcePosition_NoChildren_ReturnsSameEntry()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 5);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 5);
			var mappingEntry = UnitTestUtils.GetSimpleEntry(generated1, original1, "generated.js");

			var sourceMap = CreateSourceMap(
				sources: new List<string>() { "generated.js" },
				parsedMappings: new List<MappingEntry> { mappingEntry });

			// Act
			var rootEntry = sourceMap.GetMappingEntryForGeneratedSourcePosition(generated1);

			// Assert
			Assert.Equal(rootEntry, mappingEntry);
		}

		[Fact]
		public void ApplyMap_NullSubmap_ThrowsException()
		{
			// Arrange
			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 5);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 5);
			var mapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var map = CreateSourceMap(
				file: "generated.js",
				sources: new List<string>() { "sourceOne.js" },
				parsedMappings: new List<MappingEntry> { mapping });

			// Act
			Assert.Throws<ArgumentNullException>(() => map.ApplySourceMap(null));

			// Assert (decorated expected exception)
		}

		[Fact]
		public void ApplyMap_NoMatchingSources_ReturnsSameMap()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 3);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 2);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "someOtherSource.js");

			var childMap = CreateSourceMap(
				file: "notSourceOne.js",
				sources: new List<string>() { "someOtherSource.js" },
				parsedMappings: new List<MappingEntry> { childMapping });

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 7);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 3);
			var parentMapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var parentMap = CreateSourceMap(
				file: "generated.js",
				sources: new List<string>() { "sourceOne.js" },
				parsedMappings: new List<MappingEntry> { parentMapping });

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			var firstMapping = combinedMap.ParsedMappings[0];
			Assert.True(firstMapping.Equals(parentMapping));
		}

		[Fact]
		public void ApplyMap_NoMatchingMappings_ReturnsSameMap()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 2);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 10);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceTwo.js");

			var childMap = CreateSourceMap(
				file: "sourceOne.js",
				sources: new List<string>() { "sourceTwo.js" },
				parsedMappings: new List<MappingEntry> { childMapping });

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 4);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 5);
			var parentMapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var parentMap = CreateSourceMap(
				file: "generated.js",
				sources: new List<string>() { "sourceOne.js" },
				parsedMappings: new List<MappingEntry> { parentMapping });

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			var firstMapping = combinedMap.ParsedMappings[0];
			Assert.True(firstMapping.Equals(parentMapping));
		}

		[Fact]
		public void ApplyMap_MatchingSources_ReturnsCorrectMap()
		{
			// Expect mapping with same source filename as the applied source-map to be replaced

			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 4);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 3);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceTwo.js");

			var childMap = CreateSourceMap(
				file: "sourceOne.js",
				sources: new List<string>() { "sourceTwo.js" },
				parsedMappings: new List<MappingEntry> { childMapping });

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 5);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 4);
			var parentMapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var parentMap = CreateSourceMap(
				file: "generated.js",
				sources: new List<string>() { "sourceOne.js" },
				parsedMappings: new List<MappingEntry> { parentMapping });

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			Assert.Single(combinedMap.ParsedMappings);
			Assert.Single(combinedMap.Sources);
			var rootMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2);
			Assert.Equal(0, rootMapping.Value.SourcePosition.CompareTo(childMapping.SourcePosition));
		}

		[Fact]
		public void ApplyMap_PartialMatchingSources_ReturnsCorrectMap()
		{
			// Expect mappings with same source filename as the applied source-map to be replaced
			// mappings with a different source filename should stay the same

			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 10);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 5);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceTwo.js");

			var childMap = CreateSourceMap(
				file: "sourceOne.js",
				sources: new List<string>() { "sourceTwo.js" },
				parsedMappings: new List<MappingEntry> { childMapping });

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 2);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 10);
			var mapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var generated3 = UnitTestUtils.GenerateSourcePosition(lineNumber: 4, colNumber: 3);
			var original3 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 2);
			var mapping2 = UnitTestUtils.GetSimpleEntry(generated3, original3, "noMapForThis.js");

			var parentMap = CreateSourceMap(
				file: "generated.js",
				sources: new List<string> { "sourceOne.js", "noMapForThis.js" },
				parsedMappings: new List<MappingEntry> { mapping, mapping2 });

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			Assert.Equal(2, combinedMap.ParsedMappings.Count);
			Assert.Equal(2, combinedMap.Sources.Count);
			var firstCombinedMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
			Assert.True(firstCombinedMapping.Equals(mapping2));
			var secondCombinedMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2);
			Assert.Equal(0, secondCombinedMapping.Value.SourcePosition.CompareTo(childMapping.SourcePosition));
		}

		[Fact]
		public void ApplyMap_ExactMatchDeep_ReturnsCorrectMappingEntry()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 5);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 10);
			var mapLevel2 = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceThree.js");

			var grandChildMap = CreateSourceMap(
				file: "sourceTwo.js",
				sources: new List<string>() { "sourceThree.js" },
				parsedMappings: new List<MappingEntry> { mapLevel2 });

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 4, colNumber: 3);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 5);
			var mapLevel1 = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceTwo.js");

			var childMap = CreateSourceMap(
				file: "sourceOne.js",
				sources: new List<string>() { "sourceTwo.js" },
				parsedMappings: new List<MappingEntry> { mapLevel1 });

			var generated3 = UnitTestUtils.GenerateSourcePosition(lineNumber: 5, colNumber: 5);
			var original3 = UnitTestUtils.GenerateSourcePosition(lineNumber: 4, colNumber: 3);
			var mapLevel0 = UnitTestUtils.GetSimpleEntry(generated3, original3, "sourceOne.js");

			var parentMap = CreateSourceMap(
				file: "generated.js",
				sources: new List<string>() { "sourceOne.js" },
				parsedMappings: new List<MappingEntry> { mapLevel0 });

			// Act
			var firstCombinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(firstCombinedMap);
			var secondCombinedMap = firstCombinedMap.ApplySourceMap(grandChildMap);
			Assert.NotNull(secondCombinedMap);
			var rootMapping = secondCombinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
			Assert.Equal(0, rootMapping.Value.SourcePosition.CompareTo(mapLevel2.SourcePosition));
		}

		private static SourceMap CreateSourceMap(
			int version = default(int),
			string file = default(string),
			string mappings = default(string),
			IReadOnlyList<string> sources = default(IReadOnlyList<string>),
			IReadOnlyList<string> names = default(IReadOnlyList<string>),
			IReadOnlyList<MappingEntry> parsedMappings = default(IReadOnlyList<MappingEntry>),
			IReadOnlyList<string> sourcesContent = default(IReadOnlyList<string>))
		{
			return new SourceMap(
				version: version,
				file: file,
				mappings: mappings,
				sources: sources,
				names: names,
				parsedMappings: parsedMappings,
				sourcesContent: sourcesContent);
		}
	}
}
