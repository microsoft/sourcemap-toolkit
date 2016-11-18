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
		private IStackFrameDeminifier GetStackFrameDeminifierWithMockDependencies(ISourceMapStore sourceMapStore = null, IFunctionMapStore functionMapStore = null, IFunctionMapConsumer functionMapConsumer = null, bool useSimpleStackFrameDeminier = false)
		{
			if (sourceMapStore == null)
			{
				sourceMapStore = MockRepository.GenerateStub<ISourceMapStore>();
			}

			if (functionMapStore == null)
			{
				functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			}

			if (functionMapConsumer == null)
			{
				functionMapConsumer = MockRepository.GenerateStub<IFunctionMapConsumer>();
			}

			if (useSimpleStackFrameDeminier)
			{
				return new SimpleStackFrameDeminifier(functionMapStore, functionMapConsumer);
			}
			else
			{
				return new StackFrameDeminifier(sourceMapStore, functionMapStore, functionMapConsumer);
			}
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
		public void DeminifyStackFrame_StackFrameNullProperties_DoesNotThrowException()
		{
			// Arrange
			StackFrame stackFrame = new StackFrame();
			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies();

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

		[TestMethod]
		public void SimpleStackFrameDeminierDeminifyStackFrame_FunctionMapReturnsNull_NoFunctionMapDeminificationError()
		{
			// Arrange
			string filePath = "foo";
			StackFrame stackFrame = new StackFrame {FilePath = filePath };
			IFunctionMapStore functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			functionMapStore.Stub(c => c.GetFunctionMapForSourceCode(filePath))
				.Return(null);

			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore, useSimpleStackFrameDeminier:true);

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.AreEqual(DeminificationError.NoSourceCodeProvided, stackFrameDeminification.DeminificationError);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

		[TestMethod]
		public void SimpleStackFrameDeminierDeminifyStackFrame_GetWRappingFunctionForSourceLocationReturnsNull_NoWrapingFunctionDeminificationError()
		{
			// Arrange
			string filePath = "foo";
			StackFrame stackFrame = new StackFrame { FilePath = filePath };
			IFunctionMapStore functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			functionMapStore.Stub(c => c.GetFunctionMapForSourceCode(filePath))
				.Return(new List<FunctionMapEntry>());
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStub<IFunctionMapConsumer>();
			functionMapConsumer.Stub(c => c.GetWrappingFunctionForSourceLocation(Arg<SourcePosition>.Is.Anything, Arg<List<FunctionMapEntry>>.Is.Anything))
				.Return(null);

			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore, functionMapConsumer: functionMapConsumer, useSimpleStackFrameDeminier: true);

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.AreEqual(DeminificationError.NoWrapingFunctionFound, stackFrameDeminification.DeminificationError);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

		[TestMethod]
		public void SimpleStackFrameDeminierDeminifyStackFrame_WrapingFunctionFound_NoDeminificationError()
		{
			// Arrange
			string filePath = "foo";
			FunctionMapEntry wrapingFunctionMapEntry = new FunctionMapEntry {DeminfifiedMethodName = "DeminifiedFoo"};
			StackFrame stackFrame = new StackFrame { FilePath = filePath };
			IFunctionMapStore functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			functionMapStore.Stub(c => c.GetFunctionMapForSourceCode(filePath))
				.Return(new List<FunctionMapEntry>());
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStub<IFunctionMapConsumer>();
			functionMapConsumer.Stub(c => c.GetWrappingFunctionForSourceLocation(Arg<SourcePosition>.Is.Anything, Arg<List<FunctionMapEntry>>.Is.Anything))
				.Return(wrapingFunctionMapEntry);

			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore, functionMapConsumer: functionMapConsumer, useSimpleStackFrameDeminier: true);

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.AreEqual(DeminificationError.None, stackFrameDeminification.DeminificationError);
			Assert.AreEqual(wrapingFunctionMapEntry.DeminfifiedMethodName, stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}


		[TestMethod]
		public void StackFrameDeminierDeminifyStackFrame_SourceMapProviderReturnsNull_NoSourcemapProvidedError()
		{
			// Arrange
			string filePath = "foo";
			FunctionMapEntry wrapingFunctionMapEntry = new FunctionMapEntry { DeminfifiedMethodName = "DeminifiedFoo" };
			StackFrame stackFrame = new StackFrame { FilePath = filePath };
			IFunctionMapStore functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			functionMapStore.Stub(c => c.GetFunctionMapForSourceCode(filePath))
				.Return(new List<FunctionMapEntry>());
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStub<IFunctionMapConsumer>();
			functionMapConsumer.Stub(c => c.GetWrappingFunctionForSourceLocation(Arg<SourcePosition>.Is.Anything, Arg<List<FunctionMapEntry>>.Is.Anything))
				.Return(wrapingFunctionMapEntry);

			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(functionMapStore: functionMapStore, functionMapConsumer: functionMapConsumer);

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.AreEqual(DeminificationError.NoSourceMap, stackFrameDeminification.DeminificationError);
			Assert.AreEqual(wrapingFunctionMapEntry.DeminfifiedMethodName, stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

		[TestMethod]
		public void StackFrameDeminierDeminifyStackFrame_SourceMapParsingNull_SourceMapFailedToParseError()
		{
			// Arrange
			string filePath = "foo";
			FunctionMapEntry wrapingFunctionMapEntry = new FunctionMapEntry { DeminfifiedMethodName = "DeminifiedFoo" };
			StackFrame stackFrame = new StackFrame { FilePath = filePath };
			IFunctionMapStore functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			functionMapStore.Stub(c => c.GetFunctionMapForSourceCode(filePath))
				.Return(new List<FunctionMapEntry>());
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStub<IFunctionMapConsumer>();
			functionMapConsumer.Stub(c => c.GetWrappingFunctionForSourceLocation(Arg<SourcePosition>.Is.Anything, Arg<List<FunctionMapEntry>>.Is.Anything))
				.Return(wrapingFunctionMapEntry);
			ISourceMapStore sourceMapStore = MockRepository.GenerateStub<ISourceMapStore>();
			sourceMapStore.Stub(c => c.GetSourceMapForUrl(Arg<string>.Is.Anything)).Return(new SourceMap());

			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore: sourceMapStore,functionMapStore: functionMapStore, functionMapConsumer: functionMapConsumer);

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.AreEqual(DeminificationError.SourceMapFailedToParse, stackFrameDeminification.DeminificationError);
			Assert.AreEqual(wrapingFunctionMapEntry.DeminfifiedMethodName, stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

		[TestMethod]
		public void StackFrameDeminierDeminifyStackFrame_SourceMapGeneratedMappingEntryNull_NoMatchingMapingInSourceMapError()
		{
			// Arrange
			string filePath = "foo";
			FunctionMapEntry wrapingFunctionMapEntry = new FunctionMapEntry { DeminfifiedMethodName = "DeminifiedFoo" };
			StackFrame stackFrame = new StackFrame { FilePath = filePath };
			IFunctionMapStore functionMapStore = MockRepository.GenerateStub<IFunctionMapStore>();
			functionMapStore.Stub(c => c.GetFunctionMapForSourceCode(filePath))
				.Return(new List<FunctionMapEntry>());
			ISourceMapStore sourceMapStore = MockRepository.GenerateStub<ISourceMapStore>();
			SourceMap sourceMap = new SourceMap() {ParsedMappings = new List<MappingEntry>()};

			sourceMapStore.Stub(c => c.GetSourceMapForUrl(Arg<string>.Is.Anything)).Return(sourceMap);
			IFunctionMapConsumer functionMapConsumer = MockRepository.GenerateStub<IFunctionMapConsumer>();
			functionMapConsumer.Stub(c => c.GetWrappingFunctionForSourceLocation(Arg<SourcePosition>.Is.Anything, Arg<List<FunctionMapEntry>>.Is.Anything))
				.Return(wrapingFunctionMapEntry);

			IStackFrameDeminifier stackFrameDeminifier = GetStackFrameDeminifierWithMockDependencies(sourceMapStore: sourceMapStore, functionMapStore: functionMapStore, functionMapConsumer: functionMapConsumer);

			// Act
			StackFrameDeminificationResult stackFrameDeminification = stackFrameDeminifier.DeminifyStackFrame(stackFrame);

			// Assert
			Assert.AreEqual(DeminificationError.NoMatchingMapingInSourceMap, stackFrameDeminification.DeminificationError);
			Assert.AreEqual(wrapingFunctionMapEntry.DeminfifiedMethodName, stackFrameDeminification.DeminifiedStackFrame.MethodName);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.SourcePosition);
			Assert.IsNull(stackFrameDeminification.DeminifiedStackFrame.FilePath);
		}

	}
}
