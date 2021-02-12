using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class FunctionMapConsumerUnitTests
	{
		[Fact]
		public void GetWrappingFunctionForSourceLocation_EmptyFunctionMap_ReturnNull()
		{
			// Arrange
			var sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 3
			};
			var functionMap = new List<FunctionMapEntry>();
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			var wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Null(wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_SingleIrrelevantFunctionMapEntry_ReturnNull()
		{
			// Arrange
			var sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 3
			};
			var functionMap = new List<FunctionMapEntry>
			{
				new FunctionMapEntry(
					null!,
					new SourcePosition {ZeroBasedLineNumber = 40, ZeroBasedColumnNumber = 10},
					new SourcePosition {ZeroBasedLineNumber = 50, ZeroBasedColumnNumber = 10})
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			var wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Null(wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_SingleRelevantFunctionMapEntry_ReturnWrappingFunction()
		{
			// Arrange
			var sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 41,
				ZeroBasedColumnNumber = 2
			};
			var functionMapEntry = new FunctionMapEntry(
				null!,
				new SourcePosition { ZeroBasedLineNumber = 40, ZeroBasedColumnNumber = 10 },
				new SourcePosition { ZeroBasedLineNumber = 50, ZeroBasedColumnNumber = 10 });

			var functionMap = new List<FunctionMapEntry>
			{
				functionMapEntry
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			var wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Equal(functionMapEntry, wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_MultipleFunctionMapEntriesSingleRelevantFunctionMapEntry_ReturnWrappingFunction()
		{
			// Arrange
			var sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 31,
				ZeroBasedColumnNumber = 0
			};
			var functionMapEntry = new FunctionMapEntry(
				null!,
				new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 10 },
				new SourcePosition { ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 30 });

			var functionMapEntry2 = new FunctionMapEntry(
				null!,
				new SourcePosition { ZeroBasedLineNumber = 30, ZeroBasedColumnNumber = 0 },
				new SourcePosition { ZeroBasedLineNumber = 40, ZeroBasedColumnNumber = 2 });

			var functionMap = new List<FunctionMapEntry>
			{
				functionMapEntry,
				functionMapEntry2,
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			var wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Equal(functionMapEntry2, wrappingFunction);
		}

		[Fact]
		public void GetWrappingFunctionForSourceLocation_MultipleFunctionMapEntriesMultipleRelevantFunctionMapEntry_ReturnClosestWrappingFunction()
		{
			// Arrange
			var sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 10,
				ZeroBasedColumnNumber = 25
			};
			var functionMapEntry = new FunctionMapEntry(
				null!,
				new SourcePosition { ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 10 },
				new SourcePosition { ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 30 });

			var functionMapEntry2 = new FunctionMapEntry(
				null!,
				new SourcePosition { ZeroBasedLineNumber = 9, ZeroBasedColumnNumber = 0 },
				new SourcePosition { ZeroBasedLineNumber = 15, ZeroBasedColumnNumber = 2 });

			var functionMap = new List<FunctionMapEntry>
			{
				functionMapEntry2,
				functionMapEntry
			};
			IFunctionMapConsumer functionMapConsumer = new FunctionMapConsumer();

			// Act
			var wrappingFunction = functionMapConsumer.GetWrappingFunctionForSourceLocation(sourcePosition, functionMap);

			// Assert
			Assert.Equal(functionMapEntry2, wrappingFunction);
		}
	}
}
