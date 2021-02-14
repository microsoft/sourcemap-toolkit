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
			SourceMap sourceMap = new SourceMap();
			SourcePosition sourcePosition = new SourcePosition(zeroBasedLineNumber: 4, zeroBasedColumnNumber: 3);

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Null(result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoMatchingEntryInMappingList_ReturnNull()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 0)
				}
			};
			SourcePosition sourcePosition = new SourcePosition(zeroBasedLineNumber: 4, zeroBasedColumnNumber: 3);

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Null(result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_MatchingEntryInMappingList_ReturnMatchingEntry()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			MappingEntry matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 8, zeroBasedColumnNumber: 13)
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 0)
				},
				matchingMappingEntry
			};
			SourcePosition sourcePosition = new SourcePosition(zeroBasedLineNumber: 8, zeroBasedColumnNumber: 13);

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnSameLine_ReturnSimilarEntry()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			MappingEntry matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 10, zeroBasedColumnNumber: 13)
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 0)
				},
				matchingMappingEntry
			};
			SourcePosition sourcePosition = new SourcePosition(zeroBasedLineNumber: 10, zeroBasedColumnNumber: 14);

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnDifferentLinesLine_ReturnSimilarEntry()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			MappingEntry matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 23, zeroBasedColumnNumber: 15)
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 0)
				},
				matchingMappingEntry
			};
			SourcePosition sourcePosition = new SourcePosition(zeroBasedLineNumber: 24, zeroBasedColumnNumber: 0);

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetRootMappingEntryForGeneratedSourcePosition_NoChildren_ReturnsSameEntry()
		{
			// Arrange
			SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 5);
			SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 5);
			MappingEntry mappingEntry = UnitTestUtils.getSimpleEntry(generated1, original1, "generated.js");

			SourceMap sourceMap = new SourceMap
			{
				Sources = new List<string> { "generated.js" },
				ParsedMappings = new List<MappingEntry> { mappingEntry }
			};

			// Act
			MappingEntry rootEntry = sourceMap.GetMappingEntryForGeneratedSourcePosition(generated1);

			// Assert
			Assert.Equal(rootEntry, mappingEntry);
		}

		[Fact]
		public void ApplyMap_NullSubmap_ThrowsException()
		{
			// Arrange
			SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 5);
			SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 5);
			MappingEntry mapping = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

			SourceMap map = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mapping }
			};

			// Act
			Assert.Throws<ArgumentNullException>(() => map.ApplySourceMap(null));

			// Assert (decorated expected exception)
		}

		[Fact]
		public void ApplyMap_NoMatchingSources_ReturnsSameMap()
		{
			// Arrange
			SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 3);
			SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 2);
			MappingEntry childMapping = UnitTestUtils.getSimpleEntry(generated1, original1, "someOtherSource.js");

			SourceMap childMap = new SourceMap
			{
				File = "notSourceOne.js",
				Sources = new List<string> { "someOtherSource.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 7);
			SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 3);
			MappingEntry parentMapping = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

			SourceMap parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { parentMapping }
			};

			// Act
			SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			MappingEntry firstMapping = combinedMap.ParsedMappings[0];
			Assert.True(firstMapping.IsValueEqual(parentMapping));
		}

		[Fact]
		public void ApplyMap_NoMatchingMappings_ReturnsSameMap()
		{
			// Arrange
			SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 2);
			SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 10);
			MappingEntry childMapping = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceTwo.js");

			SourceMap childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 4);
			SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 5);
			MappingEntry parentMapping = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

			SourceMap parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { parentMapping }
			};

			// Act
			SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			MappingEntry firstMapping = combinedMap.ParsedMappings[0];
			Assert.True(firstMapping.IsValueEqual(parentMapping));
		}

		[Fact]
		public void ApplyMap_MatchingSources_ReturnsCorrectMap()
		{
			// Expect mapping with same source filename as the applied source-map to be replaced

			// Arrange
			SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 4);
			SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 3);
			MappingEntry childMapping = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceTwo.js");

			SourceMap childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 5);
			SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 4);
			MappingEntry parentMapping = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

			SourceMap parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { parentMapping }
			};

			// Act
			SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			Assert.Single(combinedMap.ParsedMappings);
			Assert.Single(combinedMap.Sources);
			MappingEntry rootMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2);
			Assert.Equal(0, rootMapping.OriginalSourcePosition.CompareTo(childMapping.OriginalSourcePosition));
		}

		[Fact]
		public void ApplyMap_PartialMatchingSources_ReturnsCorrectMap()
		{
			// Expect mappings with same source filename as the applied source-map to be replaced
			// mappings with a different source filename should stay the same

			// Arrange
			SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 10);
			SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 5);
			MappingEntry childMapping = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceTwo.js");

			SourceMap childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 2);
			SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 10);
			MappingEntry mapping = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

			SourcePosition generated3 = UnitTestUtils.generateSourcePosition(lineNumber: 4, colNumber: 3);
			SourcePosition original3 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 2);
			MappingEntry mapping2 = UnitTestUtils.getSimpleEntry(generated3, original3, "noMapForThis.js");

			SourceMap parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js", "noMapForThis.js" },
				ParsedMappings = new List<MappingEntry> { mapping, mapping2 }
			};

			// Act
			SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			Assert.Equal(2, combinedMap.ParsedMappings.Count);
			Assert.Equal(2, combinedMap.Sources.Count);
			MappingEntry firstCombinedMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
			Assert.True(firstCombinedMapping.IsValueEqual(mapping2));
			MappingEntry secondCombinedMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2);
			Assert.Equal(0, secondCombinedMapping.OriginalSourcePosition.CompareTo(childMapping.OriginalSourcePosition));
		}

		[Fact]
		public void ApplyMap_ExactMatchDeep_ReturnsCorrectMappingEntry()
		{
			// Arrange
			SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 5);
			SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 10);
			MappingEntry mapLevel2 = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceThree.js");

			SourceMap grandChildMap = new SourceMap
			{
				File = "sourceTwo.js",
				Sources = new List<string> { "sourceThree.js" },
				ParsedMappings = new List<MappingEntry> { mapLevel2 }
			};

			SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 4, colNumber: 3);
			SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 5);
			MappingEntry mapLevel1 = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceTwo.js");

			SourceMap childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { mapLevel1 }
			};

			SourcePosition generated3 = UnitTestUtils.generateSourcePosition(lineNumber: 5, colNumber: 5);
			SourcePosition original3 = UnitTestUtils.generateSourcePosition(lineNumber: 4, colNumber: 3);
			MappingEntry mapLevel0 = UnitTestUtils.getSimpleEntry(generated3, original3, "sourceOne.js");

			SourceMap parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mapLevel0 }
			};

			// Act
			SourceMap firstCombinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(firstCombinedMap);
			SourceMap secondCombinedMap = firstCombinedMap.ApplySourceMap(grandChildMap);
			Assert.NotNull(secondCombinedMap);
			MappingEntry rootMapping = secondCombinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
			Assert.Equal(0, rootMapping.OriginalSourcePosition.CompareTo(mapLevel2.OriginalSourcePosition));
		}
	}
}
