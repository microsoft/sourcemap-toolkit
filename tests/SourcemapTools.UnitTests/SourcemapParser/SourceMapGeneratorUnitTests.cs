using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourceMapGeneratorUnitTests
	{
		[Fact]
		public void SerializeMappingEntry_DifferentLineNumber_SemicolonAdded()
		{
			// Arrange
			var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
			state.LastGeneratedPosition.ZeroBasedColumnNumber = 1;

			var entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
				OriginalFileName = state.Sources[0],
				OriginalName = state.Names[0],
				OriginalSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
			};

			// Act
			var result = new StringBuilder();
			SourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.True(result.ToString().IndexOf(';') >= 0);
		}

		[Fact]
		public void SerializeMappingEntry_NoOriginalFileName_OneSegment()
		{
			// Arrange
			var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });

			var entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 10 },
				OriginalSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 1 },
			};

			// Act
			var result = new StringBuilder();
			SourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.Equal("U", result.ToString());
		}

		[Fact]
		public void SerializeMappingEntry_WithOriginalFileNameNoOriginalName_FourSegments()
		{
			// Arrange
			var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" })
			{
				IsFirstSegment = false
			};

			var entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedColumnNumber = 10 },
				OriginalFileName = state.Sources[0],
				OriginalSourcePosition = new SourcePosition() { ZeroBasedColumnNumber = 5 },
			};

			// Act
			var result = new StringBuilder();
			SourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.Equal(",UAAK", result.ToString());
		}

		[Fact]
		public void SerializeMappingEntry_WithOriginalFileNameAndOriginalName_FiveSegments()
		{
			// Arrange
			var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
			state.LastGeneratedPosition.ZeroBasedLineNumber = 1;

			var entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 5 },
				OriginalSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 6 },
				OriginalFileName = state.Sources[0],
				OriginalName = state.Names[0],
			};

			// Act
			var result = new StringBuilder();
			SourceMapGenerator.SerializeMappingEntry(entry, state, result);

			// Assert
			Assert.Equal("KACMA", result.ToString());
		}

		[Fact]
		public void SerializeMapping_NullInput_ThrowsException()
		{
			// Arrange
			SourceMap? input = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => SourceMapGenerator.SerializeMapping(input!));
		}

		[Fact]
		public void SerializeMapping_SimpleSourceMap_CorrectlySerialized()
		{
			// Arrange
			var input = GetSimpleSourceMap();

			// Act
			var output = SourceMapGenerator.SerializeMapping(input);

			// Assert
			Assert.Equal("{\"version\":3,\"file\":\"CommonIntl\",\"mappings\":\"AACAA,aAAA,CAAc;\",\"sources\":[\"input/CommonIntl.js\"],\"names\":[\"CommonStrings\",\"afrikaans\"]}", output);
		}

		[Fact]
		public void SerializeMappingIntoBast64_NullInput_ThrowsException()
		{
			// Arrange
			SourceMap? input = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => SourceMapGenerator.GenerateSourceMapInlineComment(input!));
		}

		[Fact]
		public void SerializeMappingBase64_SimpleSourceMap_CorrectlySerialized()
		{
			// Arrange
			var input = GetSimpleSourceMap();

			// Act
			var output = SourceMapGenerator.GenerateSourceMapInlineComment(input);

			// Assert
			Assert.Equal("//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQ29tbW9uSW50bCIsIm1hcHBpbmdzIjoiQUFDQUEsYUFBQSxDQUFjOyIsInNvdXJjZXMiOlsiaW5wdXQvQ29tbW9uSW50bC5qcyJdLCJuYW1lcyI6WyJDb21tb25TdHJpbmdzIiwiYWZyaWthYW5zIl19", output);
		}

		private static SourceMap GetSimpleSourceMap()
		{
			var input = new SourceMap()
			{
				File = "CommonIntl",
				Names = new List<string>() { "CommonStrings", "afrikaans" },
				Sources = new List<string>() { "input/CommonIntl.js" },
				Version = 3,
			};
			input.ParsedMappings = new List<MappingEntry>()
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0 },
					OriginalFileName = input.Sources[0],
					OriginalName = input.Names[0],
					OriginalSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
				},
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 13 },
					OriginalFileName = input.Sources[0],
					OriginalSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
				},
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 14 },
					OriginalFileName = input.Sources[0],
					OriginalSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 14 },
				},
			};

			return input;
		}
	}
}
