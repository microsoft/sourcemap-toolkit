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
			SourcePosition sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 3
			};
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
			SourcePosition sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 3
			};
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>
			{
				new FunctionMapEntry
				{
					StartSourcePosition = new SourcePosition {ZeroBasedLineNumber = 40, ZeroBasedColumnNumber = 10},
					EndSourcePosition = new SourcePosition {ZeroBasedLineNumber = 50, ZeroBasedColumnNumber = 10}
				}
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
			SourcePosition sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 41,
				ZeroBasedColumnNumber = 2
			};
			FunctionMapEntry functionMapEntry = new FunctionMapEntry
			{
				StartSourcePosition = new SourcePosition { ZeroBasedLineNumber = 40, ZeroBasedColumnNumber = 10 },
				EndSourcePosition = new SourcePosition { ZeroBasedLineNumber = 50, ZeroBasedColumnNumber = 10 }
			};
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
			SourcePosition sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 31,
				ZeroBasedColumnNumber = 0
			};
			FunctionMapEntry functionMapEntry = new FunctionMapEntry
			{
				StartSourcePosition = new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 10 },
				EndSourcePosition = new SourcePosition { ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 30 }
			};
			FunctionMapEntry functionMapEntry2 = new FunctionMapEntry
			{
				StartSourcePosition = new SourcePosition { ZeroBasedLineNumber = 30, ZeroBasedColumnNumber = 0 },
				EndSourcePosition = new SourcePosition { ZeroBasedLineNumber = 40, ZeroBasedColumnNumber = 2 }
			};
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
			SourcePosition sourcePosition = new SourcePosition
			{
				ZeroBasedLineNumber = 10,
				ZeroBasedColumnNumber = 25
			};
			FunctionMapEntry functionMapEntry = new FunctionMapEntry
			{
				StartSourcePosition = new SourcePosition { ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 10 },
				EndSourcePosition = new SourcePosition { ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 30 }
			};
			FunctionMapEntry functionMapEntry2 = new FunctionMapEntry
			{
				StartSourcePosition = new SourcePosition { ZeroBasedLineNumber = 9, ZeroBasedColumnNumber = 0 },
				EndSourcePosition = new SourcePosition { ZeroBasedLineNumber = 15, ZeroBasedColumnNumber = 2 }
			};
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
