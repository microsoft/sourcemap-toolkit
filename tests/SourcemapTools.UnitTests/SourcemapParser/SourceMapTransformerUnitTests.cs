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

			var map = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mappingEntry },
				SourcesContent = new List<string>{"var a = b"}
			};

			// Act
			var linesOnlyMap = SourceMapTransformer.Flatten(map);

			// Assert
			Assert.NotNull(linesOnlyMap);
			Assert.Single(linesOnlyMap.Sources);
			Assert.Single(linesOnlyMap.SourcesContent);
			Assert.Single(linesOnlyMap.ParsedMappings);
			Assert.Equal(1, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(2, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedColumnNumber);
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

			var map = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mappingEntry, mappingEntry2 },
				SourcesContent = new List<string>{"var a = b"}
			};

			// Act
			var linesOnlyMap = SourceMapTransformer.Flatten(map);

			// Assert
			Assert.NotNull(linesOnlyMap);
			Assert.Single(linesOnlyMap.Sources);
			Assert.Single(linesOnlyMap.SourcesContent);
			Assert.Single(linesOnlyMap.ParsedMappings);
			Assert.Equal(1, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(2, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedColumnNumber);
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

			var map = new SourceMap
			{
				File = "generated.js",
				Sources = new List<string> { "sourceOne.js" },
				ParsedMappings = new List<MappingEntry> { mappingEntry, mappingEntry2 },
				SourcesContent = new List<string>{"var a = b"}
			};

			// Act
			var linesOnlyMap = SourceMapTransformer.Flatten(map);

			// Assert
			Assert.NotNull(linesOnlyMap);
			Assert.Single(linesOnlyMap.Sources);
			Assert.Single(linesOnlyMap.SourcesContent);
			Assert.Single(linesOnlyMap.ParsedMappings);
			Assert.Equal(1, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(2, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, linesOnlyMap.ParsedMappings[0].OriginalSourcePosition.ZeroBasedColumnNumber);
		}
	}
}
