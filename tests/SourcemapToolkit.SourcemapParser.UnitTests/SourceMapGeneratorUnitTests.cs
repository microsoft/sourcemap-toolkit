using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourceMapGeneratorUnitTests
	{
		[Fact]
		public void SerializeMappingEntry_DifferentLineNumber_SemicolonAdded()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
			state.UpdateLastGeneratedPositionColumn(zeroBasedColumnNumber: 1);

			MappingEntry entry = new MappingEntry(
				generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 0),
				originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 0),
				originalName: state.Names[0],
				originalFileName: state.Sources[0]);

			// Act
			var result = new StringBuilder();
			sourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.True(result.ToString().IndexOf(';') >= 0);
		}

		[Fact]
		public void SerializeMappingEntry_NoOriginalFileName_OneSegment()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });

			MappingEntry entry = new MappingEntry(
				generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 10),
				originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 1));

			// Act
			var result = new StringBuilder();
			sourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.Equal("U", result.ToString());
		}

		[Fact]
		public void SerializeMappingEntry_WithOriginalFileNameNoOriginalName_FourSegments()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
			state.IsFirstSegment = false;

			MappingEntry entry = new MappingEntry(
				generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 10),
				originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 5),
				originalFileName: state.Sources[0]);

			// Act
			var result = new StringBuilder();
			sourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.Equal(",UAAK", result.ToString());
		}

		[Fact]
		public void SerializeMappingEntry_WithOriginalFileNameAndOriginalName_FiveSegments()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
			state.AdvanceLastGeneratedPositionLine();

			MappingEntry entry = new MappingEntry(
				generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 5),
				originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 6),
				originalName: state.Names[0],
				originalFileName: state.Sources[0]);

			// Act
			var result = new StringBuilder();
			sourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.Equal("KACMA", result.ToString());
		}

		[Fact]
		public void SerializeMapping_NullInput_ThrowsException()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();
			SourceMap input = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => sourceMapGenerator.SerializeMapping(input));
		}

		[Fact]
		public void SerializeMapping_SimpleSourceMap_CorrectlySerialized()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();
			SourceMap input = this.GetSimpleSourceMap();

			// Act
			string output = sourceMapGenerator.SerializeMapping(input);

			// Assert
			Assert.Equal("{\"version\":3,\"file\":\"CommonIntl\",\"mappings\":\"AACAA,aAAA,CAAc;\",\"sources\":[\"input/CommonIntl.js\"],\"names\":[\"CommonStrings\",\"afrikaans\"]}", output);
		}

		[Fact]
		public void SerializeMappingIntoBast64_NullInput_ThrowsException()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();
			SourceMap input = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => sourceMapGenerator.GenerateSourceMapInlineComment(input));
		}

		[Fact]
		public void SerializeMappingBase64_SimpleSourceMap_CorrectlySerialized()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();
			SourceMap input = this.GetSimpleSourceMap();

			// Act
			string output = sourceMapGenerator.GenerateSourceMapInlineComment(input);

			// Assert
			Assert.Equal("//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQ29tbW9uSW50bCIsIm1hcHBpbmdzIjoiQUFDQUEsYUFBQSxDQUFjOyIsInNvdXJjZXMiOlsiaW5wdXQvQ29tbW9uSW50bC5qcyJdLCJuYW1lcyI6WyJDb21tb25TdHJpbmdzIiwiYWZyaWthYW5zIl19", output);
		}

		private SourceMap GetSimpleSourceMap()
		{
			List<string> sources = new List<string>() { "input/CommonIntl.js" };
			List<string> names = new List<string>() { "CommonStrings", "afrikaans" };

			var parsedMappings = new List<MappingEntry>()
				{
					new MappingEntry(
						generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 0),
						originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 0),
						originalName: names[0],
						originalFileName: sources[0]),
					new MappingEntry(
						generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 13),
						originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 0),
						originalFileName: sources[0]),
					new MappingEntry(
						generatedSourcePosition: new SourcePosition(zeroBasedLineNumber: 0, zeroBasedColumnNumber: 14),
						originalSourcePosition: new SourcePosition(zeroBasedLineNumber: 1, zeroBasedColumnNumber: 14),
						originalFileName: sources[0]),
				};

			SourceMap input = new SourceMap(
				version: 3,
				file: "CommonIntl",
				mappings: default(string),
				sources: sources,
				names: names,
				parsedMappings: parsedMappings,
				sourcesContent: default(IReadOnlyList<string>));

			return input;
		}
	}
}
