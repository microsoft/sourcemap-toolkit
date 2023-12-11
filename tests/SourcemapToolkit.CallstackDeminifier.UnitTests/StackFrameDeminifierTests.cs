using System;
using System.Collections.Generic;
using Xunit;
using SourcemapToolkit.SourcemapParser;
using Moq;
using SourcemapToolkit.CallstackDeminifier.Mapping;
using SourcemapToolkit.CallstackDeminifier.SourceProviders;
using SourcemapToolkit.CallstackDeminifier.StackFrameDeminifiers;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class StackFrameDeminifierTests
{ 
	private IStackFrameDeminifier GetStackFrameDeminifierWithMockDependencies(
		ISourceMapStore sourceMapStore = null, IFunctionMapStore functionMapStore = null,
		IFunctionMapConsumer functionMapConsumer = null, bool useSimpleStackFrameDeminifier = false)
	{
		if (sourceMapStore == null)
		{
			sourceMapStore = new Mock<ISourceMapStore>().Object;
		}

		if (functionMapStore == null)
		{
			functionMapStore = new Mock<IFunctionMapStore>().Object;
		}

		if (functionMapConsumer == null)
		{
			functionMapConsumer = new Mock<IFunctionMapConsumer>().Object;
		}

		if (useSimpleStackFrameDeminifier)
		{
			return new MethodNameStackFrameDeminifier(functionMapStore, functionMapConsumer);
		}
		else
		{
			return new StackFrameDeminifier(sourceMapStore, functionMapStore, functionMapConsumer);
		}
	}

	[Fact]
	public void DeminifyStackFrame_NullInputStackFrame_ThrowsException()
	{
		// Arrange
		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies();
		StackFrame stackFrame = null;

		// Act
		Assert.Throws<ArgumentNullException>(() => stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null));
	}

	[Fact]
	public void DeminifyStackFrame_StackFrameNullProperties_DoesNotThrowException()
	{
		// Arrange
		var stackFrame = new StackFrame();
		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies();

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Null(stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}

	[Fact]
	public void SimpleStackFrameDeminierDeminifyStackFrame_FunctionMapReturnsNull_NoFunctionMapDeminificationError()
	{
		// Arrange
		var filePath = "foo";
		var stackFrame = new StackFrame {FilePath = filePath };
		var functionMapStore = new Mock<IFunctionMapStore>();
		functionMapStore.Setup(c => c.GetFunctionMapForSourceCode(filePath))
			.Returns((IReadOnlyList<FunctionMapEntry>)null);

		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore.Object, useSimpleStackFrameDeminifier:true);

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Equal(DeminificationError.NoSourceCodeProvided, stackFrameDeminification.Error);
		Assert.Null(stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}

	[Fact]
	public void SimpleStackFrameDeminierDeminifyStackFrame_GetWRappingFunctionForSourceLocationReturnsNull_NoWrapingFunctionDeminificationError()
	{
		// Arrange
		var filePath = "foo";
		var stackFrame = new StackFrame { FilePath = filePath };
		var functionMapStore = new Mock<IFunctionMapStore>();
		functionMapStore.Setup(c => c.GetFunctionMapForSourceCode(filePath))
			.Returns(new List<FunctionMapEntry>());
		var functionMapConsumer = new Mock<IFunctionMapConsumer>();
		functionMapConsumer.Setup(c => c.GetWrappingFunction(It.IsAny<SourcePosition>(), It.IsAny<List<FunctionMapEntry>>()))
			.Returns((FunctionMapEntry)null);

		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore.Object, functionMapConsumer: functionMapConsumer.Object, useSimpleStackFrameDeminifier: true);

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Equal(DeminificationError.NoWrapingFunctionFound, stackFrameDeminification.Error);
		Assert.Null(stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}

	[Fact]
	public void SimpleStackFrameDeminierDeminifyStackFrame_WrapingFunctionFound_NoDeminificationError()
	{
		// Arrange
		var filePath = "foo";
		var wrapingFunctionMapEntry = CreateFunctionMapEntry(deminifiedMethodName: "DeminifiedFoo");
		var stackFrame = new StackFrame { FilePath = filePath };
		var functionMapStore = new Mock<IFunctionMapStore>();
		functionMapStore.Setup(c => c.GetFunctionMapForSourceCode(filePath))
			.Returns(new List<FunctionMapEntry>());
		var functionMapConsumer = new Mock<IFunctionMapConsumer>();
		functionMapConsumer.Setup(c => c.GetWrappingFunction(It.IsAny<SourcePosition>(), It.IsAny<List<FunctionMapEntry>>()))
			.Returns(wrapingFunctionMapEntry);

		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore.Object, functionMapConsumer: functionMapConsumer.Object, useSimpleStackFrameDeminifier: true);

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Equal(DeminificationError.None, stackFrameDeminification.Error);
		Assert.Equal(wrapingFunctionMapEntry.DeminifiedMethodName, stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}


	[Fact]
	public void StackFrameDeminierDeminifyStackFrame_SourceMapProviderReturnsNull_NoSourcemapProvidedError()
	{
		// Arrange
		var filePath = "foo";
		var wrapingFunctionMapEntry = CreateFunctionMapEntry(deminifiedMethodName: "DeminifiedFoo");
		var stackFrame = new StackFrame { FilePath = filePath };
		var functionMapStore = new Mock<IFunctionMapStore>();
		functionMapStore.Setup(c => c.GetFunctionMapForSourceCode(filePath))
			.Returns(new List<FunctionMapEntry>());
		var functionMapConsumer = new Mock<IFunctionMapConsumer>();
		functionMapConsumer.Setup(c => c.GetWrappingFunction(It.IsAny<SourcePosition>(), It.IsAny<List<FunctionMapEntry>>()))
			.Returns(wrapingFunctionMapEntry);

		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore.Object, functionMapConsumer: functionMapConsumer.Object);

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Equal(DeminificationError.NoSourceMap, stackFrameDeminification.Error);
		Assert.Equal(wrapingFunctionMapEntry.DeminifiedMethodName, stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}

	[Fact]
	public void StackFrameDeminierDeminifyStackFrame_SourceMapParsingNull_SourceMapFailedToParseError()
	{
		// Arrange
		var filePath = "foo";
		var wrapingFunctionMapEntry = CreateFunctionMapEntry(deminifiedMethodName: "DeminifiedFoo");
		var stackFrame = new StackFrame { FilePath = filePath };
		var functionMapStore = new Mock<IFunctionMapStore>();
		functionMapStore.Setup(c => c.GetFunctionMapForSourceCode(filePath))
			.Returns(new List<FunctionMapEntry>());
		var functionMapConsumer = new Mock<IFunctionMapConsumer>();
		functionMapConsumer.Setup(c => c.GetWrappingFunction(It.IsAny<SourcePosition>(), It.IsAny<List<FunctionMapEntry>>()))
			.Returns(wrapingFunctionMapEntry);
		var sourceMapStore = new Mock<ISourceMapStore>();
		sourceMapStore.Setup(c => c.GetSourceMapForUrl(It.IsAny<string>()))
			.Returns(CreateSourceMap());

		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore: sourceMapStore.Object,functionMapStore: functionMapStore.Object, functionMapConsumer: functionMapConsumer.Object);

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Equal(DeminificationError.SourceMapFailedToParse, stackFrameDeminification.Error);
		Assert.Equal(wrapingFunctionMapEntry.DeminifiedMethodName, stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}

	[Fact]
	public void StackFrameDeminierDeminifyStackFrame_SourceMapGeneratedMappingEntryNull_NoMatchingMapingInSourceMapError()
	{
		// Arrange
		var filePath = "foo";
		var wrapingFunctionMapEntry = CreateFunctionMapEntry(deminifiedMethodName: "DeminifiedFoo");
		var stackFrame = new StackFrame { FilePath = filePath };
		var functionMapStore = new Mock<IFunctionMapStore>();
		functionMapStore.Setup(c => c.GetFunctionMapForSourceCode(filePath))
			.Returns(new List<FunctionMapEntry>());
		var sourceMapStore = new Mock<ISourceMapStore>();
		var sourceMap = CreateSourceMap(parsedMappings: new());

		sourceMapStore.Setup(c => c.GetSourceMapForUrl(It.IsAny<string>()))
			.Returns(sourceMap);
		var functionMapConsumer = new Mock<IFunctionMapConsumer>();
		functionMapConsumer.Setup(c => c.GetWrappingFunction(It.IsAny<SourcePosition>(), It.IsAny<List<FunctionMapEntry>>()))
			.Returns(wrapingFunctionMapEntry);

		var stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore: sourceMapStore.Object, functionMapStore: functionMapStore.Object, functionMapConsumer: functionMapConsumer.Object);

		// Act
		var stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame, callerSymbolName: null);

		// Assert
		Assert.Equal(DeminificationError.NoMatchingMapingInSourceMap, stackFrameDeminification.Error);
		Assert.Equal(wrapingFunctionMapEntry.DeminifiedMethodName, stackFrameDeminification.StackFrame.MethodName);
		Assert.Equal(SourcePosition.NotFound, stackFrameDeminification.StackFrame.SourcePosition);
		Assert.Null(stackFrameDeminification.StackFrame.FilePath);
	}

	private static FunctionMapEntry CreateFunctionMapEntry(string deminifiedMethodName)
	{
		return new(
			bindings: default,
			deminifiedMethodName: deminifiedMethodName,
			startSourcePosition: default,
			endSourcePosition: default);
	}

	private static SourceMap CreateSourceMap(List<MappingEntry> parsedMappings = default)
	{
		return new(
			version: default,
			file: default,
			mappings: default,
			sources: default,
			names: default,
			parsedMappings: parsedMappings,
			sourcesContent: default);
	}
}