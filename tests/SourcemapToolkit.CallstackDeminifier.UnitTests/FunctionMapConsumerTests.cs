using System.Collections.Generic;
using SourcemapToolkit.CallstackDeminifier.Mapping;
using SourcemapToolkit.SourcemapParser;
using Xunit;


namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class FunctionMapConsumerTests
{
	[Fact]
	public void GetWrappingFunctionForSourceLocation_EmptyFunctionMap_ReturnNull()
	{
		// Arrange
		var sourcePosition = new SourcePosition(
			line: 2,
			column: 3);
		var functionMap = new List<FunctionMapEntry>();
		IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

		// Act
		var wrappingFunction = functionMapConsumer.GetWrappingFunction(sourcePosition, functionMap);

		// Assert
		Assert.Null(wrappingFunction);
	}

	[Fact]
	public void GetWrappingFunctionForSourceLocation_SingleIrrelevantFunctionMapEntry_ReturnNull()
	{
		// Arrange
		var sourcePosition = new SourcePosition(
			line: 2,
			column: 3);
		var functionMap = new List<FunctionMapEntry>
		{
			new(
				bindings: default,
				deminifiedMethodName: default,
				startSourcePosition: new(line: 40, column: 10),
				endSourcePosition: new(line: 50, column: 10))
				
		};
		IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

		// Act
		var wrappingFunction = functionMapConsumer.GetWrappingFunction(sourcePosition, functionMap);

		// Assert
		Assert.Null(wrappingFunction);
	}

	[Fact]
	public void GetWrappingFunctionForSourceLocation_SingleRelevantFunctionMapEntry_ReturnWrappingFunction()
	{
		// Arrange
		var sourcePosition = new SourcePosition(
			line: 41,
			column: 2);
		var functionMapEntry = new FunctionMapEntry(
			bindings: default,
			deminifiedMethodName: default,
			startSourcePosition: new(line: 40, column: 10),
			endSourcePosition: new(line: 50, column: 10));
		var functionMap = new List<FunctionMapEntry>
		{
			functionMapEntry
		};
		IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

		// Act
		var wrappingFunction = functionMapConsumer.GetWrappingFunction(sourcePosition, functionMap);

		// Assert
		Assert.Equal(functionMapEntry, wrappingFunction);
	}

	[Fact]
	public void GetWrappingFunctionForSourceLocation_MultipleFunctionMapEntriesSingleRelevantFunctionMapEntry_ReturnWrappingFunction()
	{
		// Arrange
		var sourcePosition = new SourcePosition(line: 31, column: 0);
		var functionMapEntry = new FunctionMapEntry(
			bindings: default,
			deminifiedMethodName: default,
			startSourcePosition: new(line: 10, column: 10),
			endSourcePosition: new(line: 20, column: 30));
		var functionMapEntry2 = new FunctionMapEntry(
			bindings: default,
			deminifiedMethodName: default,
			startSourcePosition: new(line: 30, column: 0),
			endSourcePosition: new(line: 40, column: 2));
		var functionMap = new List<FunctionMapEntry>
		{
			functionMapEntry,
			functionMapEntry2,
		};
		IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

		// Act
		var wrappingFunction = functionMapConsumer.GetWrappingFunction(sourcePosition, functionMap);

		// Assert
		Assert.Equal(functionMapEntry2, wrappingFunction);
	}

	[Fact]
	public void GetWrappingFunctionForSourceLocation_MultipleFunctionMapEntriesMultipleRelevantFunctionMapEntry_ReturnClosestWrappingFunction()
	{
		// Arrange
		var sourcePosition = new SourcePosition(
			line: 10,
			column: 25);
		var functionMapEntry = new FunctionMapEntry(
			bindings: default,
			deminifiedMethodName: default,
			startSourcePosition: new(line: 5, column: 10),
			endSourcePosition: new(line: 20, column: 30));
		var functionMapEntry2 = new FunctionMapEntry(
			bindings: default,
			deminifiedMethodName: default,
			startSourcePosition: new(line: 9, column: 0),
			endSourcePosition: new(line: 15, column: 2));
		var functionMap = new List<FunctionMapEntry>
		{
			functionMapEntry2,
			functionMapEntry
		};
		IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

		// Act
		var wrappingFunction = functionMapConsumer.GetWrappingFunction(sourcePosition, functionMap);

		// Assert
		Assert.Equal(functionMapEntry2, wrappingFunction);
	}
}