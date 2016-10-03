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
			IFunctionMapStore functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			functionMapStore.Stub(x => x.GetFunctionMapForSourceCode("http://localhost/file.js")).Return(null);
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore);
			StackFrame stackFrame = null;

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);
		}

		[TestMethod]
		public void DeminifyStackFrame_NoMatchingFunctionMap_ReturnsNull()
		{
			// Arrange
			IFunctionMapStore functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			functionMapStore.Stub(x => x.GetFunctionMapForSourceCode("http://localhost/file.js")).Return(null);
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore);
			StackFrame stackFrame = new StackFrame
			{
				FilePath = "http://localhost/file.js",
				MethodName = "foobar",
				SourcePosition = new SourcePosition {ZeroBasedColumnNumber = 23, ZeroBasedLineNumber = 43}
			};

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Asset
			Assert.IsNull(deminifiedStackFrame);
			functionMapStore.VerifyAllExpectations();
		}

		[TestMethod]
		public void DeminifyStackFrame_NoWrappingFunction_ReturnsNull()
		{
			// Arrange
			SourcePosition generatedSourcePosition = new SourcePosition
			{
				ZeroBasedColumnNumber = 23,
				ZeroBasedLineNumber = 43
			};
			IFunctionMapStore functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>();
			functionMapStore.Stub(x => x.GetFunctionMapForSourceCode("http://localhost/file.js")).Return(functionMap);
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStrictMock<IFunctionMapConsumer>();
			functionMapConsumer.Stub(x => x.GetWrappingFunctionForSourceLocation(generatedSourcePosition, functionMap)).Return(null);
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore, functionMapConsumer: functionMapConsumer);
			StackFrame stackFrame = new StackFrame
			{
				FilePath = "http://localhost/file.js",
				MethodName = "foobar",
				SourcePosition = generatedSourcePosition
			};

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Asset
			Assert.IsNull(deminifiedStackFrame);
			functionMapStore.VerifyAllExpectations();
		}

		[TestMethod]
		public void DeminifyStackFrame_NoMatchingSourceMap_ReturnsNull()
		{
			// Arrange
			SourcePosition generatedSourcePosition = new SourcePosition
			{
				ZeroBasedColumnNumber = 23,
				ZeroBasedLineNumber = 43
			};
			IFunctionMapStore functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>();
			string filePath = "http://localhost/file.js";
			functionMapStore.Stub(x => x.GetFunctionMapForSourceCode(filePath)).Return(functionMap);
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStrictMock<IFunctionMapConsumer>();
			FunctionMapEntry functionMapEntry = new FunctionMapEntry();
			functionMapConsumer.Stub(x => x.GetWrappingFunctionForSourceLocation(generatedSourcePosition, functionMap)).Return(functionMapEntry);
			ISourceMapStore sourceMapStore = MockRepository.GenerateStrictMock<ISourceMapStore>();
			sourceMapStore.Stub(x => x.GetSourceMapForUrl(filePath)).Return(null);
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore, functionMapStore, functionMapConsumer);
			StackFrame stackFrame = new StackFrame
			{
				FilePath = filePath,
				MethodName = "foobar",
				SourcePosition = generatedSourcePosition
			};

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Asset
			Assert.IsNull(deminifiedStackFrame);
			functionMapStore.VerifyAllExpectations();
		}

		[TestMethod]
		public void DeminifyStackFrame_MatchingMappingEntry_ReturnsStackFrame()
		{
			// Arrange
			SourcePosition generatedSourcePosition = new SourcePosition
			{
				ZeroBasedColumnNumber = 23,
				ZeroBasedLineNumber = 43
			};
			IFunctionMapStore functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>();
			string filePath = "http://localhost/file.js";
			functionMapStore.Stub(x => x.GetFunctionMapForSourceCode(filePath)).Return(functionMap);
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStrictMock<IFunctionMapConsumer>();
			FunctionMapEntry functionMapEntry = new FunctionMapEntry {FunctionNameSourcePosition = new SourcePosition()};
			functionMapConsumer.Stub(x => x.GetWrappingFunctionForSourceLocation(generatedSourcePosition, functionMap)).Return(functionMapEntry);
			ISourceMapStore sourceMapStore = MockRepository.GenerateStrictMock<ISourceMapStore>();
			SourceMap sourceMap = MockRepository.GenerateStrictMock<SourceMap>();
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(functionMapEntry.FunctionNameSourcePosition)).Return(null);
			sourceMapStore.Stub(x => x.GetSourceMapForUrl(filePath)).Return(sourceMap);
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore, functionMapStore, functionMapConsumer);
			StackFrame stackFrame = new StackFrame
			{
				FilePath = filePath,
				MethodName = "foobar",
				SourcePosition = generatedSourcePosition
			};

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Asset
			Assert.IsNull(deminifiedStackFrame);
			functionMapStore.VerifyAllExpectations();
		}

		[TestMethod]
		public void DeminifyStackFrame_NullMatchingMappingEntry_ReturnsNull()
		{
			// Arrange
			SourcePosition generatedSourcePosition = new SourcePosition
			{
				ZeroBasedColumnNumber = 23,
				ZeroBasedLineNumber = 43
			};
			IFunctionMapStore functionMapStore = MockRepository.GenerateStrictMock<IFunctionMapStore>();
			List<FunctionMapEntry> functionMap = new List<FunctionMapEntry>();
			string filePath = "http://localhost/file.js";
			functionMapStore.Stub(x => x.GetFunctionMapForSourceCode(filePath)).Return(functionMap);
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStrictMock<IFunctionMapConsumer>();
			FunctionMapEntry functionMapEntry = new FunctionMapEntry {StartSourcePosition = new SourcePosition()};
			functionMapConsumer.Stub(x => x.GetWrappingFunctionForSourceLocation(generatedSourcePosition, functionMap)).Return(functionMapEntry);
			ISourceMapStore sourceMapStore = MockRepository.GenerateStrictMock<ISourceMapStore>();
			SourceMap sourceMap = MockRepository.GenerateStrictMock<SourceMap>();
			SourcePosition originalSourcePosition = new SourcePosition{ZeroBasedColumnNumber = 44, ZeroBasedLineNumber = 88};
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(functionMapEntry.FunctionNameSourcePosition)).Return(new MappingEntry
			{
				OriginalSourcePosition = originalSourcePosition,
				OriginalName = "realmethodname",
				OriginalFileName = "http://localhost/originalfoo.js"
			});
			sourceMapStore.Stub(x => x.GetSourceMapForUrl(filePath)).Return(sourceMap);
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore, functionMapStore, functionMapConsumer);
			StackFrame stackFrame = new StackFrame
			{
				FilePath = filePath,
				MethodName = "foobar",
				SourcePosition = generatedSourcePosition
			};

			// Act
			StackFrame deminifiedStackFrame = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Asset
			Assert.AreEqual("realmethodname", deminifiedStackFrame.MethodName);
			Assert.AreEqual(originalSourcePosition, deminifiedStackFrame.SourcePosition);
			Assert.AreEqual("http://localhost/originalfoo.js", deminifiedStackFrame.FilePath);
			functionMapStore.VerifyAllExpectations();
		}
	}
}