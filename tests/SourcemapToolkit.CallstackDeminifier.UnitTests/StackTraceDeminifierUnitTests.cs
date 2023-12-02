using System.Collections.Generic;
using Xunit;
using Rhino.Mocks;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class StackTraceDeminifierUnitTests
	{
		[Fact]
		public void DeminifyStackTrace_UnableToParseStackTraceString_ReturnsEmptyList()
		{
			// Arrange
			var stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			var stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString, out var message)).Return(new List<StackFrame>()).OutRef("Error example");

			var stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();

			var stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			var result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(0, result.DeminifiedStackFrameResults.Count);
		}

		[Fact]
		public void DeminifyStackTrace_UnableToDeminifyStackTrace_ResultContainsNullDeminifiedFrame()
		{
			// Arrange
			var stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			var minifiedStackFrames = new List<StackFrame> { new StackFrame() };
			var stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString, out var message)).Return(minifiedStackFrames).OutRef("Error example");

			var stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();
			stackFrameDeminifier.Stub(x => x.DeminifyStackFrame(minifiedStackFrames[0], null)).Return(null);
			
			var stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			var result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(1, result.DeminifiedStackFrameResults.Count);
			Assert.Equal(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.Null(result.DeminifiedStackFrameResults[0]);
		}

		[Fact]
		public void DeminifyStackTrace_AbleToDeminifyStackTrace_ResultContainsDeminifiedFrame()
		{
			// Arrange
			var stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			var minifiedStackFrames = new List<StackFrame> { new StackFrame() };
			var stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString, out var message)).Return(minifiedStackFrames).OutRef("Error example");

			var stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();
			var stackFrameDeminification = new StackFrameDeminificationResult();
			stackFrameDeminifier.Stub(x => x.DeminifyStackFrame(minifiedStackFrames[0], null)).Return(stackFrameDeminification);

			var stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			var result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(1, result.DeminifiedStackFrameResults.Count);
			Assert.Equal(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.Equal(stackFrameDeminification, result.DeminifiedStackFrameResults[0]);
		}
	}
}