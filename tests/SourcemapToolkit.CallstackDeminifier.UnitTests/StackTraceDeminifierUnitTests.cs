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
			IStackTraceParser stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			string stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString)).Return(new List<StackFrame>());

			IStackFrameDeminifier stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();

			StackTraceDeminifier stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			DeminifyStackTraceResult result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(0, result.DeminifiedStackFrameResults.Count);
		}

		[Fact]
		public void DeminifyStackTrace_UnableToDeminifyStackTrace_ResultContainsNullDeminifiedFrame()
		{
			// Arrange
			IStackTraceParser stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			List<StackFrame> minifiedStackFrames = new List<StackFrame> { new StackFrame() };
			string stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString)).Return(minifiedStackFrames);

			IStackFrameDeminifier stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();
			stackFrameDeminifier.Stub(x => x.DeminifyStackFrame(minifiedStackFrames[0])).Return(null);
			
			StackTraceDeminifier stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			DeminifyStackTraceResult result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(1, result.DeminifiedStackFrameResults.Count);
			Assert.Equal(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.Null(result.DeminifiedStackFrameResults[0]);
		}

		[Fact]
		public void DeminifyStackTrace_AbleToDeminifyStackTrace_ResultContainsDeminifiedFrame()
		{
			// Arrange
			IStackTraceParser stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			List<StackFrame> minifiedStackFrames = new List<StackFrame> { new StackFrame() };
			string stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString)).Return(minifiedStackFrames);

			IStackFrameDeminifier stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();
			StackFrameDeminificationResult stackFrameDeminification = new StackFrameDeminificationResult();
			stackFrameDeminifier.Stub(x => x.DeminifyStackFrame(minifiedStackFrames[0])).Return(stackFrameDeminification);

			StackTraceDeminifier stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			DeminifyStackTraceResult result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(1, result.DeminifiedStackFrameResults.Count);
			Assert.Equal(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.Equal(stackFrameDeminification, result.DeminifiedStackFrameResults[0]);
		}
	}
}