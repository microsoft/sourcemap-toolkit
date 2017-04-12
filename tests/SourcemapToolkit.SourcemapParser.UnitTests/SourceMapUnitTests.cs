﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyMap_NullSubmap_ThrowsException()
        {
            // Arrange
            SourcePosition generated2 = generateSourcePosition(3);
            SourcePosition original2 = generateSourcePosition(2);
            MappingEntry mapping = getSimpleEntry(generated2, original2, "sourceOne.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { mapping }
            };

            // Act
            SourceMap combinedMap = map.ApplySourceMap(null);

            // Assert (decorated expected exception)
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
            Assert.IsTrue(firstMapping.IsValueEqual(parentMapping));
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
            Assert.AreEqual(0, rootMapping.OriginalSourcePosition.CompareTo(childMapping.OriginalSourcePosition));
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
            MappingEntry firstMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated3);
            Assert.IsTrue(firstMapping.IsValueEqual(mapping2));
            MappingEntry secondMapping = combinedMap.GetMappingEntryForGeneratedSourcePosition(generated2);
            Assert.AreEqual(0, secondMapping.OriginalSourcePosition.CompareTo(childMapping.OriginalSourcePosition));
        }

        [TestMethod]
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
            Assert.AreEqual(0, rootMapping.OriginalSourcePosition.CompareTo(l2.OriginalSourcePosition));
        }
    }
}
