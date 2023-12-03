using System;
using System.Collections.Generic;
using Moq;
using SourcemapToolkit.CallstackDeminifier.SourceProviders;
using Xunit;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class SourceMapExtensionsTests
{
	[Fact]
	public void GetDeminifiedMethodName_NullBindings_ReturnsNull()
	{
		// Arrange
		IReadOnlyList<BindingInformation> bindings = null;
		var sourceMap = CreateSourceMapMock();

		// Act
		var deminifiedMethodName = SourceMapExtensions.GetDeminifiedMethodName(sourceMap.Object, bindings);

		// Assert
		Assert.Null(deminifiedMethodName);
	}

	[Fact]
	public void GetDeminifiedMethodName_NullSourceMap_ThrowsException()
	{
		// Arrange
		IReadOnlyList<BindingInformation> bindings = null;
		SourceMap sourceMap = null;

		// Act
		Assert.Throws<ArgumentNullException>(() => SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings));
	}

	[Fact]
	public void GetDeminifiedMethodName_EmptyBinding_ReturnNullMethodName()
	{
		// Arrange
		IReadOnlyList<BindingInformation> bindings = new List<BindingInformation>();
		var sourceMap = CreateSourceMapMock();

		// Act
		var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap.Object, bindings);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void GetDeminifiedMethodName_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
	{
		// Arrange
		var bindings = new List<BindingInformation>()
		{
			new(
				name: default,
				sourcePosition: new(line: 20, column: 15))
		};

		var sourceMap = CreateSourceMapMock();
		sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(It.IsAny<SourcePosition>()))
			.Returns<MappingEntry?>(null!);

		// Act
		var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap.Object, bindings);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public void GetDeminifiedMethodName_HasSingleBindingMatchingMapping_ReturnsMethodName()
	{
		// Arrange
		var bindings = new List<BindingInformation>()
		{
			new(
				name: default,
				sourcePosition: new(line: 5, column: 8))
		};

		var sourceMap = CreateSourceMapMock();
		sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(It.Is<SourcePosition>(y => y.Line == 5 && y.Column == 8)))
			.Returns(new MappingEntry(sourceMapPosition: default, originalName: "foo"));

		// Act
		var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap.Object, bindings);

		// Assert
		Assert.Equal("foo", result);
	}

	[Fact]
	public void GetDeminifiedMethodName_MatchingMappingMultipleBindingsMissingPrototypeMapping_ReturnsMethodName()
	{
		// Arrange
		var bindings = new List<BindingInformation>
		{
			new(
				name: default,
				sourcePosition: new(line: 86, column: 52)),
			new(
				name: default,
				sourcePosition: new(line: 88, column: 78))
		};

		var sourceMap = CreateSourceMapMock();
		
		sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(
				It.Is<SourcePosition>(y => y.Line == 86 && y.Column == 52)))
			.Returns((MappingEntry?)null);
		
		sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(
				It.Is<SourcePosition>(y => y.Line == 88 && y.Column == 78)))
			.Returns(new MappingEntry(sourceMapPosition: default, originalName: "baz"));

		// Act
		var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap.Object, bindings);

		// Assert
		Assert.Equal("baz", result);
	}

	[Fact]
	public void GetDeminifiedMethodName_MatchingMappingMultipleBindings_ReturnsMethodNameWithFullBinding()
	{
		// Arrange
		var bindings = new List<BindingInformation>
		{
			new(
				name: default,
				sourcePosition: new(line: 5, column: 5)),
			new(
				name: default,
				sourcePosition: new(line: 20, column: 10))
		};

		var sourceMap = CreateSourceMapMock();
		sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(
				It.Is<SourcePosition>(y => y.Line == 5 && y.Column == 5)))
			.Returns(new MappingEntry(sourceMapPosition: default, originalName: "bar"));

		sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(
				It.Is<SourcePosition>(y => y.Line == 20 && y.Column == 10)))
			.Returns(new MappingEntry(sourceMapPosition: default, originalName: "baz"));
		// Act
		var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap.Object, bindings);

		// Assert
		Assert.Equal("bar.baz", result);
	}

	private static Mock<SourceMap> CreateSourceMapMock()
	{
		return new Mock<SourceMap>(MockBehavior.Default,
			0 /* version */,
			default(string) /* file */,
			default(string) /* mappings */,
			default(IReadOnlyList<string>) /* sources */,
			default(IReadOnlyList<string>) /* names */,
			default(IReadOnlyList<MappingEntry>) /* parsedMappings */,
			default(IReadOnlyList<string>));
	}
}