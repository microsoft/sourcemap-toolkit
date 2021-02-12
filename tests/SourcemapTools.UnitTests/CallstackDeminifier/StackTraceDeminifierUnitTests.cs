using System.Collections.Generic;
using Moq;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class StackTraceDeminifierUnitTests
	{
		[Fact]
		public void DeminifyStackTrace_UnableToParseStackTraceString_ReturnsEmptyList()
		{
			// Arrange
			var stackTraceParser = new Mock<IStackTraceParser>();
			var stackTraceString = "foobar";
			var message = "Error example";
			stackTraceParser.Setup(x => x.ParseStackTrace(stackTraceString, out message)).Returns(new List<StackFrame>());

			var stackFrameDeminifier = new Mock<IStackFrameDeminifier>().Object;

			var stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser.Object);

			// Act
			var result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(0, result.DeminifiedStackFrameResults.Count);
		}

		[Fact]
		public void DeminifyStackTrace_AbleToDeminifyStackTrace_ResultContainsDeminifiedFrame()
		{
			// Arrange
			var stackTraceParser = new Mock<IStackTraceParser>();
			var minifiedStackFrames = new List<StackFrame> { new StackFrame() };
			var stackTraceString = "foobar";
			var message = "Error example";
			stackTraceParser.Setup(x => x.ParseStackTrace(stackTraceString, out message)).Returns(minifiedStackFrames);

			var stackFrameDeminifier = new Mock<IStackFrameDeminifier>();
			var stackFrameDeminification = new StackFrameDeminificationResult(default, null!);
			stackFrameDeminifier.Setup(x => x.DeminifyStackFrame(minifiedStackFrames[0], null)).Returns(stackFrameDeminification);

			var stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier.Object, stackTraceParser.Object);

			// Act
			var result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.Equal(1, result.DeminifiedStackFrameResults.Count);
			Assert.Equal(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.Equal(stackFrameDeminification, result.DeminifiedStackFrameResults[0]);
		}
	}
}