using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class StackTraceDeminifierUnitTests
	{
		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void StackTraceDeminifier_NullSourceMapProvider_ThrowsException()
		{
			// Arrange
			ISourceCodeProvider sourceCodeProvider = MockRepository.GenerateStrictMock<ISourceCodeProvider>();
			ISourceMapProvider sourceMapProvider = null;

			// Act
			StackTraceDeminifier stackTraceDeminifier = new StackTraceDeminifier(sourceMapProvider, sourceCodeProvider);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void StackTraceDeminifier_NullSourceCodeProvider_ThrowsException()
		{
			// Arrange
			ISourceCodeProvider sourceCodeProvider = null;
			ISourceMapProvider sourceMapProvider = MockRepository.GenerateStrictMock<ISourceMapProvider>();

			// Act
			StackTraceDeminifier stackTraceDeminifier = new StackTraceDeminifier(sourceMapProvider, sourceCodeProvider);
		}

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
			List<DeminifyStackFrameResult> result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.AreEqual(0, result.Count);
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
			List<DeminifyStackFrameResult> result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(result[0].MinifiedStackFrame, minifiedStackFrames[0]);
			Assert.IsNull(result[0].DeminifiedStackFrame);
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
			List<DeminifyStackFrameResult> result = stackTraceDeminifier.DeminifyStackTrace(stackTraceString);

			// Assert
			Assert.AreEqual(1, result.Count);
			Assert.AreEqual(minifiedStackFrames[0], result[0].MinifiedStackFrame);
			Assert.AreEqual(deminifiedStackFrame, result[0].DeminifiedStackFrame);
		}
	}
}