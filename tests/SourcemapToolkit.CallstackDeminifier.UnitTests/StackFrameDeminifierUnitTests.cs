using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class StackFrameDeminifierUnitTests
	{
		private IStackFrameDeminifier GetStackFrameDeminifierWithMockDependencies(ISourceMapStore sourceMapStore = null, IFunctionMapStore functionMapStore = null, IFunctionMapConsumer functionMapConsumer = null)
		{
			if (sourceMapStore == null)
			{
				sourceMapStore = MockRepository.GenerateStrictMock<ISourceMapStore>();
			}

			if (functionMapStore == null)
			{
				functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			}

			if (functionMapConsumer == null)
			{
				functionMapConsumer = MockRepository.GenerateStrictMock<IFunctionMapConsumer>();
			}

			return new StackFrameDeminifier(sourceMapStore, functionMapStore, functionMapConsumer);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeminifyStackFrame_NullInputStackFrame_ThrowsException()
		{
			// Arrange
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies();
			StackFrame stackFrame = null;

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_HasMatchingGeneratedPositionMapping_ReturnsStackFrameWithSourcePositionAndFileName()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = null;
			SourcePosition generatedSourcePosition = new SourcePosition
			{
				ZeroBasedColumnNumber = 25,
				ZeroBasedLineNumber = 85
			};

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(generatedSourcePosition)).Return(new MappingEntry
			{
				OriginalSourcePosition = new SourcePosition { ZeroBasedColumnNumber = 10, ZeroBasedLineNumber = 20 }
			});

			// Act
			StackFrame deminifiedStackFrame = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(10, deminifiedStackFrame.SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(20, deminifiedStackFrame.SourcePosition.ZeroBasedLineNumber);
		}
	}
}