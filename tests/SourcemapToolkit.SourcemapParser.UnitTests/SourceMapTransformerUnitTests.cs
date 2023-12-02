using System.Collections.Generic;

using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	/// <summary>
	/// Summary description for SourceMapTransformerUnitTests
	/// </summary>
	public class SourceMapTransformerUnitTests
	{

		[Fact]
		public void FlattenMap_ReturnsOnlyLineInformation()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 2);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 2);
			var mappingEntry = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceOne.js");

			var map = new SourceMap(
				version: default(int),
				file: "generated.js",
				mappings: default(string),
				sources: new List<string>() { "sourceOne.js" },
				names: default(IReadOnlyList<string>),
				parsedMappings: new List<MappingEntry> { mappingEntry },
				sourcesContent: new List<string> { "var a = b" });

			// Act
			var linesOnlyMap = SourceMapTransformer.Flatten(map);

			// Assert
			Assert.NotNull(linesOnlyMap);
			Assert.Single(linesOnlyMap.Sources);
			Assert.Single(linesOnlyMap.SourcesContent);
			Assert.Single(linesOnlyMap.ParsedMappings);
			Assert.Equal(1, linesOnlyMap.ParsedMappings[0].SourceMapPosition.Line);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].SourceMapPosition.Column);
			Assert.Equal(2, linesOnlyMap.ParsedMappings[0].SourcePosition.Line);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].SourcePosition.Column);
		}

		[Fact]
		public void FlattenMap_MultipleMappingsSameLine_ReturnsOnlyOneMappingPerLine()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 2);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 2);
			var mappingEntry = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceOne.js");

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 5);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 5);
			var mappingEntry2 = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var map = new SourceMap(
				version: default(int),
				file: "generated.js",
				mappings: default(string),
				sources: new List<string>() { "sourceOne.js" },
				names: default(IReadOnlyList<string>),
				parsedMappings: new List<MappingEntry> { mappingEntry, mappingEntry2 },
				sourcesContent: new List<string> { "var a = b" });

			// Act
			var linesOnlyMap = SourceMapTransformer.Flatten(map);

			// Assert
			Assert.NotNull(linesOnlyMap);
			Assert.Single(linesOnlyMap.Sources);
			Assert.Single(linesOnlyMap.SourcesContent);
			Assert.Single(linesOnlyMap.ParsedMappings);
			Assert.Equal(1, linesOnlyMap.ParsedMappings[0].SourceMapPosition.Line);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].SourceMapPosition.Column);
			Assert.Equal(2, linesOnlyMap.ParsedMappings[0].SourcePosition.Line);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].SourcePosition.Column);
		}

		[Fact]
		public void FlattenMap_MultipleOriginalLineToSameGeneratedLine_ReturnsFirstOriginalLine()
		{
			// Arrange
			var generated1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 2);
			var original1 = UnitTestUtils.GenerateSourcePosition(lineNumber: 2, colNumber: 2);
			var mappingEntry = UnitTestUtils.GetSimpleEntry(generated1, original1, "sourceOne.js");

			var generated2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 1, colNumber: 3);
			var original2 = UnitTestUtils.GenerateSourcePosition(lineNumber: 3, colNumber: 5);
			var mappingEntry2 = UnitTestUtils.GetSimpleEntry(generated2, original2, "sourceOne.js");

			var map = new SourceMap(
				version: default(int),
				file: "generated.js",
				mappings: default(string),
				sources: new List<string>() { "sourceOne.js" },
				names: default(IReadOnlyList<string>),
				parsedMappings: new List<MappingEntry> { mappingEntry, mappingEntry2 },
				sourcesContent: new List<string> { "var a = b" });

			// Act
			var linesOnlyMap = SourceMapTransformer.Flatten(map);

			// Assert
			Assert.NotNull(linesOnlyMap);
			Assert.Single(linesOnlyMap.Sources);
			Assert.Single(linesOnlyMap.SourcesContent);
			Assert.Single(linesOnlyMap.ParsedMappings);
			Assert.Equal(1, linesOnlyMap.ParsedMappings[0].SourceMapPosition.Line);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].SourceMapPosition.Column);
			Assert.Equal(2, linesOnlyMap.ParsedMappings[0].SourcePosition.Line);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].SourcePosition.Column);
		}
	}
}
