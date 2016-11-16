using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class StackTraceDeminifierUnitTests
	{
		[TestMethod]
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
			Assert.AreEqual(0, result.DeminifiedStackFrames.Count);
		}

		[TestMethod]
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
			Assert.AreEqual(1, result.DeminifiedStackFrames.Count);
			Assert.AreEqual(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.IsNull(result.DeminifiedStackFrames[0]);
		}

		[TestMethod]
		public void DeminifyStackTrace_AbleToDeminifyStackTrace_ResultContainsDeminifiedFrame()
		{
			// Arrange
			IStackTraceParser stackTraceParser = MockRepository.GenerateStrictMock<IStackTraceParser>();
			List<StackFrame> minifiedStackFrames = new List<StackFrame> { new StackFrame() };
			string stackTraceString = "foobar";
			stackTraceParser.Stub(x => x.ParseStackTrace(stackTraceString)).Return(minifiedStackFrames);

			IStackFrameDeminifier stackFrameDeminifier = MockRepository.GenerateStrictMock<IStackFrameDeminifier>();
			StackFrame deminifiedStackFrame = new StackFrame();
			stackFrameDeminifier.Stub(x => x.DeminifyStackFrame(minifiedStackFrames[0])).Return(deminifiedStackFrame);

			StackTraceDeminifier stackTraceDeminifier = new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);

			// Act
			DeminifyStackTraceResult result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.AreEqual(1, result.DeminifiedStackFrames.Count);
			Assert.AreEqual(minifiedStackFrames[0], result.MinifiedStackFrames[0]);
			Assert.AreEqual(deminifiedStackFrame, result.DeminifiedStackFrames[0]);
		}
	}
}