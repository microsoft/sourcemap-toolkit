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
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_NullInputs_DoesNotThrowException()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = null;
			SourceMap sourceMap = null;
			SourcePosition generatedSourcePosition = null;

			// Act
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_NoBinding_ReturnNullMethodName()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = new FunctionMapEntry();
			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			SourcePosition generatedSourcePosition = new SourcePosition();

			// Act
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(DeminificationError.NoWrapingFunction, stackFrameDeminification.DeminificationError);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			sourceMap.VerifyAllExpectations();
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_HasSingleBindingSourceMapNotParsed_ReturnNullMethodName()
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
			
			// SourceMap failed to parse
			sourceMap.ParsedMappings = null;

			SourcePosition generatedSourcePosition = new SourcePosition();

			// Act
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(DeminificationError.SourceMapFailedToParse, stackFrameDeminification.DeminificationError);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			sourceMap.VerifyAllExpectations();
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
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
			sourceMap.ParsedMappings = new List<MappingEntry>();

			SourcePosition generatedSourcePosition = new SourcePosition();

			// Act
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(DeminificationError.NoMatchingMapingInSourceMap, stackFrameDeminification.DeminificationError);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			sourceMap.VerifyAllExpectations();
		}

		[TestMethod]
		public void ExtractFrameInformationFromSourceMap_HasSingleBindingMatchingMapping_ReturnsStackFrameWithMethodName()
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

			SourcePosition generatedSourcePosition = new SourcePosition
			{
				ZeroBasedColumnNumber = 25,
				ZeroBasedLineNumber = 85
			};

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 8)))
				.Return(new MappingEntry
				{
					OriginalName = "foo",
				});

			// Act
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(DeminificationError.None, stackFrameDeminification.DeminificationError);
			Assert.AreEqual("foo", stackFrameDeminification.DeminifiedStackFrame.MethodName);
			sourceMap.VerifyAllExpectations();
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

			SourcePosition generatedSourcePosition = new SourcePosition {ZeroBasedColumnNumber = 39, ZeroBasedLineNumber = 31};

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
				});

			// Act
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(DeminificationError.None, stackFrameDeminification.DeminificationError);
			Assert.AreEqual("bar.baz", stackFrameDeminification.DeminifiedStackFrame.MethodName);
			sourceMap.VerifyAllExpectations();
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
			StackFrameDeminificationResult stackFrameDeminification = StackFrameDeminifier.ExtractFrameInformationFromSourceMap(functionMapEntry, sourceMap, generatedSourcePosition);

			// Assert
			Assert.AreEqual(DeminificationError.NoWrapingFunction, stackFrameDeminification.DeminificationError);
			Assert.AreEqual(10, stackFrameDeminification.DeminifiedStackFrame.SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(20, stackFrameDeminification.DeminifiedStackFrame.SourcePosition.ZeroBasedLineNumber);
		}
	}
}