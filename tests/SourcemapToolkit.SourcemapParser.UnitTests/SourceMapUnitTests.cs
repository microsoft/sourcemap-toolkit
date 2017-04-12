using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
    [TestClass]
    public class SourceMapUnitTests
    {
        [TestMethod]
        public void GetMappingEntryForGeneratedSourcePosition_NullMappingList_ReturnNull()
        {
            // Arrange
            SourceMap sourceMap = new SourceMap();
            SourcePosition sourcePosition = new SourcePosition {ZeroBasedColumnNumber = 3, ZeroBasedLineNumber = 4}; 

            // Act
            MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

            // Asset
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetMappingEntryForGeneratedSourcePosition_NoMatchingEntryInMappingList_ReturnNull()
        {
            // Arrange
            SourceMap sourceMap = new SourceMap();
            sourceMap.ParsedMappings = new List<MappingEntry>
            {
                new MappingEntry
                {
                    GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
                }
            };
            SourcePosition sourcePosition = new SourcePosition { ZeroBasedColumnNumber = 3, ZeroBasedLineNumber = 4 };

            // Act
            MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

            // Asset
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetMappingEntryForGeneratedSourcePosition_MatchingEntryInMappingList_ReturnMatchingEntry()
        {
            // Arrange
            SourceMap sourceMap = new SourceMap();
            MappingEntry matchingMappingEntry = new MappingEntry
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
            SourcePosition sourcePosition = new SourcePosition { ZeroBasedLineNumber = 8, ZeroBasedColumnNumber = 13 };

            // Act
            MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

            // Asset
            Assert.AreEqual(matchingMappingEntry, result);
        }

        [TestMethod]
        public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnSameLine_ReturnSimilarEntry()
        {
            // Arrange
            SourceMap sourceMap = new SourceMap();
            MappingEntry matchingMappingEntry = new MappingEntry
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
            SourcePosition sourcePosition = new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 14 };

            // Act
            MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

            // Asset
            Assert.AreEqual(matchingMappingEntry, result);
        }

        [TestMethod]
        public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnDifferentLinesLine_ReturnSimilarEntry()
        {
            // Arrange
            SourceMap sourceMap = new SourceMap();
            MappingEntry matchingMappingEntry = new MappingEntry
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
            SourcePosition sourcePosition = new SourcePosition { ZeroBasedLineNumber = 24, ZeroBasedColumnNumber = 0 };

            // Act
            MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

            // Asset
            Assert.AreEqual(matchingMappingEntry, result);
        }

        private MappingEntry getSimpleEntry(SourcePosition generatedSourcePosition, SourcePosition originalSourcePosition, string originalFileName)
        {
            return new MappingEntry
            {
                GeneratedSourcePosition = generatedSourcePosition,
                OriginalSourcePosition = originalSourcePosition,
                OriginalFileName = originalFileName
            };
        }

        private SourcePosition generateSourcePosition(int lineNumber, int colNumber = 0)
        {
            return new SourcePosition
            {
                ZeroBasedLineNumber = lineNumber,
                ZeroBasedColumnNumber = colNumber
            };
        }

        [TestMethod]
        public void GetRootMappingEntryForGeneratedSourcePosition_NoChildren_ReturnsSameEntry()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2);
            SourcePosition original1 = generateSourcePosition(1);
            MappingEntry mappingEntry = getSimpleEntry(generated1, original1, "generated.js");

            SourceMap sourceMap = new SourceMap
            {
                Sources = new List<string> { "generated.js" },
                ParsedMappings = new List<MappingEntry> { mappingEntry }
            };

            // Act
            MappingEntry rootEntry = sourceMap.GetMappingEntryForGeneratedSourcePosition(generated1);

            // Assert
            Assert.AreEqual(rootEntry, mappingEntry);
        }

        [TestMethod]
        public void ApplyMap_NoMatchingSources_ReturnsSameMap()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2);
            SourcePosition original1 = generateSourcePosition(1);
            MappingEntry childMapping = getSimpleEntry(generated1, original1, "someOtherSource.js");

            SourceMap childMap = new SourceMap
            {
                File = "notSourceOne.js",
                Sources = new List<string> { "someOtherSource.js" },
                ParsedMappings = new List<MappingEntry> { childMapping }
            };

            SourcePosition generated2 = generateSourcePosition(3);
            SourcePosition original2 = generateSourcePosition(2);
            MappingEntry parentMapping = getSimpleEntry(generated2, original2, "sourceOne.js");

            SourceMap parentMap = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { parentMapping }
            };

            // Act
            SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

            // Assert
            Assert.IsNotNull(combinedMap);
            MappingEntry firstMapping = combinedMap.ParsedMappings[0];
            Assert.Equals(firstMapping, parentMap);
        }

        [TestMethod]
        public void ApplyMap_MatchingSources_ReturnsCorrectMap()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2);
            SourcePosition original1 = generateSourcePosition(1);
            MappingEntry childMapping = getSimpleEntry(generated1, original1, "sourceTwo.js");

            SourceMap childMap = new SourceMap
            {
                File = "sourceOne.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { childMapping }
            };

            SourcePosition generated2 = generateSourcePosition(3);
            SourcePosition original2 = generateSourcePosition(2);
            MappingEntry parentMapping = getSimpleEntry(generated2, original2, "sourceOne.js");

            SourceMap parentMap = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { parentMapping }
            };

            // Act
            SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

            // Assert
            Assert.IsNotNull(combinedMap);
            Assert.AreEqual(1, combinedMap.ParsedMappings.Count);
            Assert.AreEqual(1, combinedMap.Sources.Count);
            MappingEntry rootMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2);
            Assert.AreEqual(rootMapping, childMapping);
        }


        [TestMethod]
        public void ApplyMap_PartialMatchingSources_ReturnsCorrectMap()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2);
            SourcePosition original1 = generateSourcePosition(1);
            MappingEntry childMapping = getSimpleEntry(generated1, original1, "sourceTwo.js");

            SourceMap childMap = new SourceMap
            {
                File = "sourceOne.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { childMapping }
            };

            SourcePosition generated2 = generateSourcePosition(3);
            SourcePosition original2 = generateSourcePosition(2);
            MappingEntry mapping = getSimpleEntry(generated2, original2, "sourceOne.js");

            SourcePosition generated3 = generateSourcePosition(4);
            SourcePosition original3 = generateSourcePosition(3);
            MappingEntry mapping2 = getSimpleEntry(generated3, original3, "noMapForThis.js");

            SourceMap parentMap = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js", "noMapForThis.js" },
                ParsedMappings = new List<MappingEntry> { mapping, mapping2 }
            };

            // Act
            SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

            // Assert
            Assert.IsNotNull(combinedMap);
            Assert.AreEqual(2, combinedMap.ParsedMappings.Count);
            Assert.AreEqual(2, combinedMap.Sources.Count);
            MappingEntry rootMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
            Assert.AreEqual(rootMapping, parentMap);
        }

        public void ApplyMap_ExactMatchDeep_ReturnsCorrectMappingEntry()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(3);
            SourcePosition original1 = generateSourcePosition(2);
            MappingEntry l2 = getSimpleEntry(generated1, original1, "sourceThree.js");

            SourceMap grandChildMap = new SourceMap
            {
                File = "sourceTwo.js",
                Sources = new List<string> { "sourceThree.js" },
                ParsedMappings = new List<MappingEntry> { l2 }
            };

            SourcePosition generated2 = generateSourcePosition(4);
            SourcePosition original2 = generateSourcePosition(3);
            MappingEntry l1 = getSimpleEntry(generated2, original2, "sourceTwo.js");

            SourceMap childMap = new SourceMap
            {
                File = "sourceOne.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { l1 }
            };

            SourcePosition generated3 = generateSourcePosition(5);
            SourcePosition original3 = generateSourcePosition(4);
            MappingEntry l0 = getSimpleEntry(generated3, original3, "sourceOne.js");

            SourceMap parentMap = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { l0 }
            };

            // Act
            SourceMap firstCombinedMap = parentMap.ApplySourceMap(childMap);

            // Assert
            Assert.IsNotNull(firstCombinedMap);
            SourceMap secondCombinedMap = firstCombinedMap.ApplySourceMap(grandChildMap);
            Assert.IsNotNull(secondCombinedMap);
            MappingEntry rootMapping = secondCombinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
            Assert.Equals(rootMapping, l2);
        }

        [TestMethod]
        public void ApplyMapping_MultipleLineMatch_ReturnsCorrectLineNumber()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2, 1);
            SourcePosition original1 = generateSourcePosition(1, 1);
            SourcePosition generated2 = generateSourcePosition(2, 2);
            SourcePosition original2 = generateSourcePosition(1, 2);

            MappingEntry childMapping = getSimpleEntry(generated1, original1, "sourceTwo.js");
            MappingEntry childMappingenerated2 = getSimpleEntry(generated2, original2, "sourceTwo.js");

            SourceMap childMap = new SourceMap
            {
                File = "sourceOne.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { childMapping, childMappingenerated2 }
            };

            SourcePosition generated3 = generateSourcePosition(3, 10);
            SourcePosition original3 = generateSourcePosition(2, 5);
            MappingEntry parentMapping = getSimpleEntry(generated3, original3, "sourceOne.js");

            SourceMap parentMap = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { parentMapping }
            };

            // Act
            SourceMap combinedMap = parentMap.ApplySourceMap(childMap);

            // Assert
            Assert.IsNotNull(combinedMap);
            MappingEntry rootEntry = parentMap.GetMappingEntryForGeneratedSourcePosition(generated3);
            Assert.AreEqual(1, rootEntry.OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual("sourceTwo.js", rootEntry.OriginalFileName);
        }

        [TestMethod]
        public void ApplyMapping_LineMatchMultipleLevels_ReturnsCorrectLineNumber()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(1, 10);
            SourcePosition original1 = generateSourcePosition(0, 10);
            MappingEntry grandChildMapping = getSimpleEntry(generated1, original1, "sourceThree.js");

            SourceMap grandChildMap = new SourceMap
            {
                File = "sourceTwo.js",
                Sources = new List<string> { "sourceThree.js" },
                ParsedMappings = new List<MappingEntry> { grandChildMapping }
            };

            SourcePosition generated2 = generateSourcePosition(2, 1);
            SourcePosition original2 = generateSourcePosition(1, 1);
            MappingEntry childMapping = getSimpleEntry(generated2, original2, "sourceTwo.js");

            SourceMap childMap = new SourceMap
            {
                File = "sourceOne.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { childMapping }
            };

            SourcePosition generated3 = generateSourcePosition(3, 10);
            SourcePosition original3 = generateSourcePosition(2, 5);
            MappingEntry parentMapping = getSimpleEntry(generated3, original3, "sourceOne.js");

            SourceMap parentMap = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { parentMapping }
            };

            // Act
            SourceMap firstCombinedMap = parentMap.ApplySourceMap(childMap);

            // Assert
            Assert.IsNotNull(firstCombinedMap);
            SourceMap secondCombinedMap = firstCombinedMap.ApplySourceMap(grandChildMap);
            Assert.IsNotNull(secondCombinedMap);
            MappingEntry rootMapping = secondCombinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
            Assert.AreEqual(0, rootMapping.OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, rootMapping.OriginalSourcePosition.ZeroBasedColumnNumber);
            Assert.AreEqual("sourceThree.js", rootMapping.OriginalFileName);
        }

        [TestMethod]
        public void CombineSourceMap_ReturnsCorrectSourceMap()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(1);
            SourcePosition original1 = generateSourcePosition(1);
            MappingEntry mapping = getSimpleEntry(generated1, original1, "sourceOne.js");
            SourcePosition generated2 = generateSourcePosition(2);
            SourcePosition original2 = generateSourcePosition(1);
            MappingEntry mapping2 = getSimpleEntry(generated2, original2, "sourceTwo.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js", "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { mapping, mapping2 }
            };

            SourcePosition generated3 = generateSourcePosition(3);
            SourcePosition original3 = generateSourcePosition(1);
            MappingEntry mapping3 = getSimpleEntry(generated2, original2, "sourceThree.js");

            SourceMap map2 = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceThree.js" },
                ParsedMappings = new List<MappingEntry> { mapping3 }
            };

            // Act
            SourceMap combinedMap = SourceMap.CombineMaps(new List<SourceMap> { map, map2 });

            // Assert
            Assert.IsNotNull(combinedMap);
            Assert.AreEqual(3, combinedMap.ParsedMappings.Count);
        }

        [TestMethod]
        public void CombineSourceMap_DuplicateSourceSameOriginal_ReturnsCorrectSourceMap()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2, 5);
            SourcePosition original1 = generateSourcePosition(1, 3);
            MappingEntry mapping = getSimpleEntry(generated1, original1, "sourceOne.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { mapping }
            };

            SourcePosition generated2 = generateSourcePosition(2, 5);
            SourcePosition original2 = generateSourcePosition(1, 3);
            MappingEntry mapping2 = getSimpleEntry(generated2, original2, "sourceTwo.js");

            SourceMap map2 = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { mapping2 }
            };

            // Act
            SourceMap combinedMap = SourceMap.CombineMaps(new List<SourceMap> { map, map2 });

            // Assert
            Assert.IsNotNull(combinedMap);
            Assert.AreEqual(1, combinedMap.ParsedMappings.Count);
            MappingEntry firstEntry = combinedMap.ParsedMappings[0];
            Assert.Equals(original1.ZeroBasedLineNumber, firstEntry.OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.Equals(original1.ZeroBasedColumnNumber, firstEntry.OriginalSourcePosition.ZeroBasedColumnNumber);
        }

        [TestMethod]
        public void CombineSourceMap_DuplicateSourceDifferentOriginal_ReturnsCorrectSourceMap()
        {
            // Arrange
            SourcePosition generated1 = generateSourcePosition(2, 5);
            SourcePosition original1 = generateSourcePosition(1, 3);
            MappingEntry mapping = getSimpleEntry(generated1, original1, "sourceOne.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { mapping }
            };

            SourcePosition generated2 = generateSourcePosition(2, 5);
            SourcePosition original2 = generateSourcePosition(10, 30);
            MappingEntry mapping2 = getSimpleEntry(generated2, original2, "sourceTwo.js");

            SourceMap map2 = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceTwo.js" },
                ParsedMappings = new List<MappingEntry> { mapping2 }
            };

            // Act
            SourceMap combinedMap = SourceMap.CombineMaps(new List<SourceMap> { map, map2 });

            // Assert
            Assert.IsNotNull(combinedMap);
            Assert.AreEqual(1, combinedMap.ParsedMappings.Count);
            MappingEntry firstEntry = combinedMap.ParsedMappings[0];

            // assume the first entry was kept
            Assert.Equals(original1.ZeroBasedLineNumber, firstEntry.OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.Equals(original1.ZeroBasedColumnNumber, firstEntry.OriginalSourcePosition.ZeroBasedColumnNumber);
        }
    }
}
