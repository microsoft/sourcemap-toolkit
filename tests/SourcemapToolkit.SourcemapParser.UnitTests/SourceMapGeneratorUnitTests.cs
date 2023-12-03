using Xunit;
using System;
using System.Collections.Generic;
using System.Text;

namespace SourcemapToolkit.SourcemapParser.UnitTests;

public class SourceMapGeneratorUnitTests
{
	[Fact]
	public void SerializeMappingEntry_DifferentLineNumber_SemicolonAdded()
	{
		// Arrange
		var sourceMapGenerator = new SourceMapGenerator();

		var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
		state.UpdateLastGeneratedPositionColumn(zeroBasedColumnNumber: 1);

		var entry = new MappingEntry(
			sourceMapPosition: new(line: 1, column: 0),
			originalSourcePosition: new SourcePosition(line: 1, column: 0),
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
		var sourceMapGenerator = new SourceMapGenerator();

		var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });

		var entry = new MappingEntry(
			sourceMapPosition: new(line: 0, column: 10),
			originalSourcePosition: new SourcePosition(line: 0, column: 1));

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
		var sourceMapGenerator = new SourceMapGenerator();

		var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
		state.IsFirstSegment = false;

		var entry = new MappingEntry(
			sourceMapPosition: new(line: 0, column: 10),
			originalSourcePosition: new SourcePosition(line: 0, column: 5),
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
		var sourceMapGenerator = new SourceMapGenerator();

		var state = new MappingGenerateState(new List<string>() { "Name" }, new List<string>() { "Source" });
		state.AdvanceLastGeneratedPositionLine();

		var entry = new MappingEntry(
			sourceMapPosition: new(line: 1, column: 5),
			originalSourcePosition: new SourcePosition(line: 1, column: 6),
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
		var sourceMapGenerator = new SourceMapGenerator();
		SourceMap input = null;

		// Act
		Assert.Throws<ArgumentNullException>(() => sourceMapGenerator.SerializeMapping(input));
	}

	[Fact]
	public void SerializeMapping_SimpleSourceMap_CorrectlySerialized()
	{
		// Arrange
		var sourceMapGenerator = new SourceMapGenerator();
		var input = GetSimpleSourceMap();

		// Act
		var output = sourceMapGenerator.SerializeMapping(input);

		// Assert
		Assert.Equal("{\"version\":3,\"file\":\"CommonIntl\",\"mappings\":\"AACAA,aAAA,CAAc;\",\"sources\":[\"input/CommonIntl.js\"],\"names\":[\"CommonStrings\",\"afrikaans\"]}", output);
	}

	[Fact]
	public void SerializeMappingIntoBast64_NullInput_ThrowsException()
	{
		// Arrange
		var sourceMapGenerator = new SourceMapGenerator();
		SourceMap input = null;

		// Act
		Assert.Throws<ArgumentNullException>(() => sourceMapGenerator.GenerateSourceMapInlineComment(input));
	}

	[Fact]
	public void SerializeMappingBase64_SimpleSourceMap_CorrectlySerialized()
	{
		// Arrange
		var sourceMapGenerator = new SourceMapGenerator();
		var input = GetSimpleSourceMap();

		// Act
		var output = sourceMapGenerator.GenerateSourceMapInlineComment(input);

		// Assert
		Assert.Equal("//# sourceMappingURL=data:application/json;base64,eyJ2ZXJzaW9uIjozLCJmaWxlIjoiQ29tbW9uSW50bCIsIm1hcHBpbmdzIjoiQUFDQUEsYUFBQSxDQUFjOyIsInNvdXJjZXMiOlsiaW5wdXQvQ29tbW9uSW50bC5qcyJdLCJuYW1lcyI6WyJDb21tb25TdHJpbmdzIiwiYWZyaWthYW5zIl19", output);
	}

	private SourceMap GetSimpleSourceMap()
	{
		var sources = new List<string>() { "input/CommonIntl.js" };
		var names = new List<string>() { "CommonStrings", "afrikaans" };

		var parsedMappings = new List<MappingEntry>()
		{
			new(
				sourceMapPosition: new(line: 0, column: 0),
				originalSourcePosition: new SourcePosition(line: 1, column: 0),
				originalName: names[0],
				originalFileName: sources[0]),
			new(
				sourceMapPosition: new(line: 0, column: 13),
				originalSourcePosition: new SourcePosition(line: 1, column: 0),
				originalFileName: sources[0]),
			new(
				sourceMapPosition: new(line: 0, column: 14),
				originalSourcePosition: new SourcePosition(line: 1, column: 14),
				originalFileName: sources[0]),
		};

		var input = new SourceMap(
			version: 3,
			file: "CommonIntl",
			mappings: default,
			sources: sources,
			names: names,
			parsedMappings: parsedMappings,
			sourcesContent: default);

		return input;
	}
}