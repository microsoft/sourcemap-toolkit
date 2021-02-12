using System;
using System.Collections.Generic;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourceMapUnitTests
	{
		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NullMappingList_ReturnNull()
		{
			// Arrange
			var sourceMap = new SourceMap();
			var sourcePosition = new SourcePosition {ZeroBasedColumnNumber = 3, ZeroBasedLineNumber = 4};

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Null(result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoMatchingEntryInMappingList_ReturnNull()
		{
			// Arrange
			var sourceMap = new SourceMap
			{
				ParsedMappings = new List<MappingEntry>()
				{
					new MappingEntry
					{
						GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
					}
				}
			};
			var sourcePosition = new SourcePosition { ZeroBasedColumnNumber = 3, ZeroBasedLineNumber = 4 };

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Null(result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_MatchingEntryInMappingList_ReturnMatchingEntry()
		{
			// Arrange
			var sourceMap = new SourceMap();
			var matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 8, ZeroBasedColumnNumber = 13}
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
				},
				matchingMappingEntry
			};
			var sourcePosition = new SourcePosition { ZeroBasedLineNumber = 8, ZeroBasedColumnNumber = 13 };

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnSameLine_ReturnSimilarEntry()
		{
			// Arrange
			var sourceMap = new SourceMap();
			var matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 13 }
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
				},
				matchingMappingEntry
			};
			var sourcePosition = new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 14 };

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnDifferentLinesLine_ReturnSimilarEntry()
		{
			// Arrange
			var sourceMap = new SourceMap();
			var matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition { ZeroBasedLineNumber = 23, ZeroBasedColumnNumber = 15 }
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
				},
				matchingMappingEntry
			};
			var sourcePosition = new SourcePosition { ZeroBasedLineNumber = 24, ZeroBasedColumnNumber = 0 };

			// Act
			var result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.Equal(matchingMappingEntry, result);
		}

		[Fact]
		public void GetRootMappingEntryForGeneratedSourcePosition_NoChildren_ReturnsSameEntry()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber: 5);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber:1, colNumber: 5);
			var mappingEntry = UnitTestUtils.GetSimpleEntry(generated1, original1, "generated.js");

			var sourceMap = new SourceMap
			{
				Sources = new List<string> { "generated.js" },
				ParsedMappings = new List<MappingEntry> { mappingEntry }
			};

			// Act
			var rootEntry = sourceMap.GetMappingEntryForGeneratedSourcePosition(generated1);

			// Assert
			Assert.Equal(rootEntry, mappingEntry);
		}

		[Fact]
		public void ApplyMap_NullSubmap_ThrowsException()
		{
			// Arrange
			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber: 5);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber: 5);
			var mapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var map = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mapping }
			};

			// Act
			Assert.Throws<ArgumentNullException>(() => map.ApplySourceMap(null!));

			// Assert (decorated expected exception)
		}

		[Fact]
		public void ApplyMap_NoMatchingSources_ReturnsSameMap()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber: 3);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber:1, colNumber: 2);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "someOtherSource.js");

			var childMap = new SourceMap
			{
				File = "notSourceOne.js",
				Sources = new List<string> { "someOtherSource.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber: 7);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber: 3);
			var parentMapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { parentMapping }
			};

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			var firstMapping = combinedMap.ParsedMappings[0];
			Assert.True(firstMapping.IsValueEqual(parentMapping));
		}

		[Fact]
		public void ApplyMap_NoMatchingMappings_ReturnsSameMap()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber:2);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber:10);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceTwo.js");

			var childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber:4);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber:5);
			var parentMapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { parentMapping }
			};

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			var firstMapping = combinedMap.ParsedMappings[0];
			Assert.True(firstMapping.IsValueEqual(parentMapping));
		}

		[Fact]
		public void ApplyMap_MatchingSources_ReturnsCorrectMap()
		{
			// Expect mapping with same source filename as the applied source-map to be replaced

			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber:4);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber:1, colNumber:3);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceTwo.js");

			var childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber: 5);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber: 4);
			var parentMapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { parentMapping }
			};

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap)!;

			// Assert
			Assert.NotNull(combinedMap);
			Assert.Single(combinedMap.ParsedMappings);
			Assert.Single(combinedMap.Sources);
			var rootMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2)!;
			Assert.Equal(0, rootMapping.OriginalSourcePosition.CompareTo(childMapping.OriginalSourcePosition));
		}

		[Fact]
		public void ApplyMap_PartialMatchingSources_ReturnsCorrectMap()
		{
			// Expect mappings with same source filename as the applied source-map to be replaced
			// mappings with a different source filename should stay the same

			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber:10);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber:1, colNumber:5);
			var childMapping = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceTwo.js");

			var childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { childMapping }
			};

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber:2);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber: 10);
			var mapping = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var generated3 = UnitTestUtils.GenerateSourcePosition(lineNumber:4, colNumber:3);
			var original3 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber:2);
			var mapping2 = UnitTestUtils.GetSimpleEntry(generated3, original3, "noMapForThis.js");

			var parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js", "noMapForThis.js" },
				ParsedMappings = new List<MappingEntry> { mapping, mapping2 }
			};

			// Act
			var combinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(combinedMap);
			Assert.Equal(2, combinedMap.ParsedMappings.Count);
			Assert.Equal(2, combinedMap.Sources!.Count);
			var firstCombinedMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated3)!;
			Assert.True(firstCombinedMapping.IsValueEqual(mapping2));
			var secondCombinedMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2)!;
			Assert.Equal(0, secondCombinedMapping.OriginalSourcePosition.CompareTo(childMapping.OriginalSourcePosition));
		}

		[Fact]
		public void ApplyMap_ExactMatchDeep_ReturnsCorrectMappingEntry()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber:5);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber:2, colNumber:10);
			var mapLevel2 = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceThree.js");

			var grandChildMap = new SourceMap
			{
				File = "sourceTwo.js",
				Sources = new List<string> { "sourceThree.js" },
				ParsedMappings = new List<MappingEntry> { mapLevel2 }
			};

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber:4, colNumber:3);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber:3, colNumber:5);
			var mapLevel1 = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceTwo.js");

			var childMap = new SourceMap
			{
				File = "sourceOne.js",
				Sources = new List<string> { "sourceTwo.js" },
				ParsedMappings = new List<MappingEntry> { mapLevel1 }
			};

			var generated3 = UnitTestUtils.GenerateSourcePosition(lineNumber:5, colNumber:5);
			var original3 = UnitTestUtils.GenerateSourcePosition(lineNumber:4, colNumber:3);
			var mapLevel0 = UnitTestUtils.GetSimpleEntry(generated3, original3, "sourceOne.js");

			var parentMap = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mapLevel0 }
			};

			// Act
			var firstCombinedMap = parentMap.ApplySourceMap(childMap);

			// Assert
			Assert.NotNull(firstCombinedMap);
			var secondCombinedMap = firstCombinedMap.ApplySourceMap(grandChildMap);
			Assert.NotNull(secondCombinedMap);
			var rootMapping = secondCombinedMap.GetMappingEntryForGeneratedSourcePosition(generated3)!;
			Assert.Equal(0, rootMapping.OriginalSourcePosition.CompareTo(mapLevel2.OriginalSourcePosition));
		}
	}
}
