using System.Collections.Generic;
using Xunit;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class FunctionMapConsumerUnitTests
	{
		[Fact]
		public void GetWrappingFunctionForSourceLocation_EmptyFunctionMap_ReturnNull()
		{
			// Arrange
			SourcePosition sourcePosition = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 3);
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>();
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			FunctionMapEntry wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Null(wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_SingleIrrelevantFunctionMapEntry_ReturnNull()
		{
			// Arrange
			SourcePosition sourcePosition = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 3);
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>
			{
				new FunctionMapEntry(
					bindings: default,
					deminifiedMethodName: default,
					startSourcePosition: new SourcePosition(zeroBasedLineNumber: 40, zeroBasedColumnNumber: 10),
					endSourcePosition: new SourcePosition(zeroBasedLineNumber: 50, zeroBasedColumnNumber: 10))
				
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			FunctionMapEntry wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Null(wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_SingleRelevantFunctionMapEntry_ReturnWrappingFunction()
		{
			// Arrange
			SourcePosition sourcePosition = new SourcePosition(
				zeroBasedLineNumber: 41,
				zeroBasedColumnNumber: 2);
			FunctionMapEntry functionMapEntry = new FunctionMapEntry(
				bindings: default,
				deminifiedMethodName: default,
				startSourcePosition: new SourcePosition(zeroBasedLineNumber: 40, zeroBasedColumnNumber: 10),
				endSourcePosition: new SourcePosition(zeroBasedLineNumber: 50, zeroBasedColumnNumber: 10));
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>
			{
				functionMapEntry
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			FunctionMapEntry wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Equal(functionMapEntry, wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_MultipleFunctionMapEntriesSingleRelevantFunctionMapEntry_ReturnWrappingFunction()
		{
			// Arrange
			SourcePosition sourcePosition = new SourcePosition(
				zeroBasedLineNumber: 31,
				zeroBasedColumnNumber: 0);
			FunctionMapEntry functionMapEntry = new FunctionMapEntry(
				bindings: default,
				deminifiedMethodName: default,
				startSourcePosition: new SourcePosition(zeroBasedLineNumber: 10, zeroBasedColumnNumber: 10),
				endSourcePosition: new SourcePosition(zeroBasedLineNumber: 20, zeroBasedColumnNumber: 30));
			FunctionMapEntry functionMapEntry2 = new FunctionMapEntry(
				bindings: default,
				deminifiedMethodName: default,
				startSourcePosition: new SourcePosition(zeroBasedLineNumber: 30, zeroBasedColumnNumber: 0),
				endSourcePosition: new SourcePosition(zeroBasedLineNumber: 40, zeroBasedColumnNumber: 2));
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>
			{
				functionMapEntry,
				functionMapEntry2,
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			FunctionMapEntry wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Equal(functionMapEntry2, wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_MultipleFunctionMapEntriesMultipleRelevantFunctionMapEntry_ReturnClosestWrappingFunction()
		{
			// Arrange
			SourcePosition sourcePosition = new SourcePosition(
				zeroBasedLineNumber: 10,
				zeroBasedColumnNumber: 25);
			FunctionMapEntry functionMapEntry = new FunctionMapEntry(
				bindings: default,
				deminifiedMethodName: default,
				startSourcePosition: new SourcePosition(zeroBasedLineNumber: 5, zeroBasedColumnNumber: 10),
				endSourcePosition: new SourcePosition(zeroBasedLineNumber: 20, zeroBasedColumnNumber: 30));
			FunctionMapEntry functionMapEntry2 = new FunctionMapEntry(
				bindings: default,
				deminifiedMethodName: default,
				startSourcePosition: new SourcePosition(zeroBasedLineNumber: 9, zeroBasedColumnNumber: 0),
				endSourcePosition: new SourcePosition(zeroBasedLineNumber: 15, zeroBasedColumnNumber: 2));
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>
			{
				functionMapEntry2,
				functionMapEntry
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			FunctionMapEntry wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Equal(functionMapEntry2, wrappingFunction);
		}
	}
}
