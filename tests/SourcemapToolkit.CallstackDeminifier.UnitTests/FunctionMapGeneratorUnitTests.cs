using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SourcemapToolkit.SourcemapParser.UnitTests;
using Rhino.Mocks;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class FunctionMapGeneratorUnitTests
	{
		[TestMethod]
		public void GenerateFunctionMap_NullSourceMap_ReturnsNull()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(UnitTestUtils.StreamReaderFromString(sourceCode), null);

			// Assert
			Assert.IsNull(functionMap);
		}


		[TestMethod]
		public void ParseSourceCode_NullInput_ReturnsNull()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(null);

			// Assert
			Assert.IsNull(functionMap);
		}

		[TestMethod]
		public void ParseSourceCode_NoFunctionsInSource_EmptyFunctionList()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "bar();";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(0, functionMap.Count);
		}

		[TestMethod]
		public void ParseSourceCode_SingleLineFunctionInSource_CorrectZeroBasedColumnNumbers()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){bar();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(1, functionMap.Count);
			Assert.AreEqual("foo", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_MultiLineFunctionInSource_CorrectColumnAndZeroBasedLineNumbers()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo()" + Environment.NewLine + "{" + Environment.NewLine + "bar();" +
								Environment.NewLine + "}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(1, functionMap.Count);
			Assert.AreEqual("foo", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(1, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(3, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(1, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_TwoSingleLineFunctions_TwoFunctionMapEntries()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){bar();}function bar(){baz();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("bar", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(31, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(36, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(44, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_TwoNestedSingleLineFunctions_TwoFunctionMapEntries()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){function bar(){baz();}}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("bar", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(24, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(29, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(37, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(38, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_FunctionAssignedToVariable_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){bar();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(1, functionMap.Count);

			Assert.AreEqual("foo", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(28, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_StaticMethod_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){};foo.bar = function() { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.bar", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(44, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(54, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_InstanceMethod_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype.bar = function () { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.prototype.bar", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(55, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(65, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_InstanceMethodInObjectInitializer_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype = { bar: function () { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.prototype", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual("bar", functionMap[0].Bindings[1].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[1].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(41, functionMap[0].Bindings[1].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(58, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(68, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_FunctionAssignedToVariableAndHasName_FunctionMapEntryGeneratedForVariableName()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function myCoolFunctionName(){ bar(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(1, functionMap.Count);

			Assert.AreEqual("foo", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(39, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(49, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_StaticMethodAndFunctionHasName_FunctionMapEntryGeneratedForPropertyName()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){};foo.bar = function myCoolFunctionName() { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.bar", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(63, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(73, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_InstanceMethodAndFunctionHasName_FunctionMapEntryGeneratedForObjectPrototype()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype.bar = function myCoolFunctionName() { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.prototype.bar", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(73, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(83, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void ParseSourceCode_InstanceMethodWithObjectInitializerAndFunctionHasName_FunctionMapEntryGeneratedForObjectPrototype()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype = { bar: function myCoolFunctionName() { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.prototype", functionMap[0].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual("bar", functionMap[0].Bindings[1].Name);
			Assert.AreEqual(0, functionMap[0].Bindings[1].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(41, functionMap[0].Bindings[1].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(76, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(86, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].Bindings[0].Name);
			Assert.AreEqual(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GetDeminifiedMethodNameFromSourceMap_NoBinding_ReturnNullMethodName()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = new FunctionMapEntry();
			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();

			// Act
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.IsNull(result);
			sourceMap.VerifyAllExpectations();
		}

		[TestMethod]
		public void GetDeminifiedMethodNameFromSourceMap_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
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
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.IsNull(result);
			sourceMap.VerifyAllExpectations();
		}

		[TestMethod]
		public void GetDeminifiedMethodNameFromSourceMap_HasSingleBindingMatchingMapping_ReturnsMethodName()
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
				});

			// Act
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.AreEqual("foo", result);
			sourceMap.VerifyAllExpectations();
		}

		[TestMethod]
		public void GetDeminifiedMethodNameFromSourceMap_MatchingMappingMultipleBindings_ReturnsMethodNameWithFullBinding()
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
				});

			// Act
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.AreEqual("bar.baz", result);
			sourceMap.VerifyAllExpectations();
		}
	}
}