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
			functionMapConsumer.VerifyAllExpectations();
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
			functionMapConsumer.VerifyAllExpectations();
			sourceMapStore.VerifyAllExpectations();
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_NoBinding_ReturnNull()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = new FunctionMapEntry();
			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();

			// Act
			StackFrame deminifiedStackFrame = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.IsNull(deminifiedStackFrame);
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_HasSingleBindingNoMatchingMapping_ReturnNull()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = new FunctionMapEntry
			{
				Bindings =
					new List<BindingInformation>
					{
						new BindingInformation
						{
							SourcePosition = new SourcePosition {ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 15}
						}
					}
			};

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(Arg<SourcePosition>.Is.Anything)).Return(null);

			// Act
			StackFrame deminifiedStackFrame = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.IsNull(deminifiedStackFrame);

		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_HasSingleBindingMatchingMapping_ReturnsStackFrame()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = new FunctionMapEntry
			{
				Bindings =
					new List<BindingInformation>
					{
						new BindingInformation
						{
							SourcePosition = new SourcePosition {ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 8}
						}
					}
			};

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 8)))
				.Return(new MappingEntry
				{
					OriginalName = "foo",
					OriginalSourcePosition = new SourcePosition { ZeroBasedColumnNumber = 10, ZeroBasedLineNumber = 20}
				});

			// Act
			StackFrame deminifiedStackFrame = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.AreEqual("foo", deminifiedStackFrame.MethodName);
			Assert.AreEqual(10, deminifiedStackFrame.SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(20, deminifiedStackFrame.SourcePosition.ZeroBasedLineNumber);
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_MatchingMappingMultipleBindings_ReturnsStackFrameWithFullBinding()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = new FunctionMapEntry
			{
				Bindings =
					new List<BindingInformation>
					{
						new BindingInformation
						{
							SourcePosition = new SourcePosition {ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 5}
						},
						new BindingInformation
						{
							SourcePosition = new SourcePosition {ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 10}
						}
					}
			};

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 5)))
				.Return(new MappingEntry
				{
					OriginalName = "bar"
				});

			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 20 && y.ZeroBasedColumnNumber == 10)))
				.Return(new MappingEntry
				{
					OriginalName = "baz",
					OriginalSourcePosition = new SourcePosition { ZeroBasedColumnNumber = 20, ZeroBasedLineNumber = 30 }
				});

			// Act
			StackFrame deminifiedStackFrame = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.AreEqual("bar.baz", deminifiedStackFrame.MethodName);
			Assert.AreEqual(20, deminifiedStackFrame.SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(30, deminifiedStackFrame.SourcePosition.ZeroBasedLineNumber);
		}
	}
}