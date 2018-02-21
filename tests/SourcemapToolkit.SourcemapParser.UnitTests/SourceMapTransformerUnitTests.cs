using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
    /// <summary>
    /// Summary description for SourceMapTransformerUnitTests
    /// </summary>
    [TestClass]
    public class SourceMapTransformerUnitTests
    {

        [TestMethod]
        public void FlattenMap_ReturnsOnlyLineInformation()
        {
            // Arrange
            SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 2);
            SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 2);
            MappingEntry mappingEntry = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceOne.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { mappingEntry },
                SourcesContent = new List<string>{"var a = b"}
            };

            // Act
            SourceMap linesOnlyMap = SourceMapTransformer.Flatten(map);

            // Assert
            Assert.IsNotNull(linesOnlyMap);
            Assert.AreEqual(1, linesOnlyMap.Sources.Count);
            Assert.AreEqual(1, linesOnlyMap.SourcesContent.Count);
            Assert.AreEqual(1, linesOnlyMap.ParsedMappings.Count);
            Assert.AreEqual(1, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedColumnNumber);
            Assert.AreEqual(2, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedColumnNumber);
        }

        [TestMethod]
        public void FlattenMap_MultipleMappingsSameLine_ReturnsOnlyOneMappingPerLine()
        {
            // Arrange
            SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 2);
            SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 2);
            MappingEntry mappingEntry = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceOne.js");

            SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 5);
            SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 5);
            MappingEntry mappingEntry2 = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { mappingEntry, mappingEntry2 },
                SourcesContent = new List<string>{"var a = b"}
            };

            // Act
            SourceMap linesOnlyMap = SourceMapTransformer.Flatten(map);

            // Assert
            Assert.IsNotNull(linesOnlyMap);
            Assert.AreEqual(1, linesOnlyMap.Sources.Count);
            Assert.AreEqual(1, linesOnlyMap.SourcesContent.Count);
            Assert.AreEqual(1, linesOnlyMap.ParsedMappings.Count);
            Assert.AreEqual(1, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedColumnNumber);
            Assert.AreEqual(2, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedColumnNumber);
        }

        [TestMethod]
        public void FlattenMap_MultipleOriginalLineToSameGeneratedLine_ReturnsFirstOriginalLine()
        {
            // Arrange
            SourcePosition generated1 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 2);
            SourcePosition original1 = UnitTestUtils.generateSourcePosition(lineNumber: 2, colNumber: 2);
            MappingEntry mappingEntry = UnitTestUtils.getSimpleEntry(generated1, original1, "sourceOne.js");

            SourcePosition generated2 = UnitTestUtils.generateSourcePosition(lineNumber: 1, colNumber: 3);
            SourcePosition original2 = UnitTestUtils.generateSourcePosition(lineNumber: 3, colNumber: 5);
            MappingEntry mappingEntry2 = UnitTestUtils.getSimpleEntry(generated2, original2, "sourceOne.js");

            SourceMap map = new SourceMap
            {
                File = "generated.js",
                Sources = new List<string> { "sourceOne.js" },
                ParsedMappings = new List<MappingEntry> { mappingEntry, mappingEntry2 },
                SourcesContent = new List<string>{"var a = b"}
            };

            // Act
            SourceMap linesOnlyMap = SourceMapTransformer.Flatten(map);

            // Assert
            Assert.IsNotNull(linesOnlyMap);
            Assert.AreEqual(1, linesOnlyMap.Sources.Count);
            Assert.AreEqual(1, linesOnlyMap.SourcesContent.Count);
            Assert.AreEqual(1, linesOnlyMap.ParsedMappings.Count);
            Assert.AreEqual(1, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedColumnNumber);
            Assert.AreEqual(2, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedLineNumber);
            Assert.AreEqual(0, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedColumnNumber);
        }
    }
}
