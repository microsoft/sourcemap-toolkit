using System;
using System.Collections.Generic;
using Xunit;
using SourcemapToolkit.SourcemapParser.UnitTests;
using Rhino.Mocks;
using SourcemapToolkit.SourcemapParser;
using System.Dynamic;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class FunctionMapGeneratorUnitTests
	{
		[Fact]
		public void GenerateFunctionMap_NullSourceMap_ReturnsNull()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(UnitTestUtils.StreamReaderFromString(sourceCode), null);

			// Assert
			Assert.Null(functionMap);
		}


		[Fact]
		public void ParseSourceCode_NullInput_ReturnsNull()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(null);

			// Assert
			Assert.Null(functionMap);
		}

		[Fact]
		public void ParseSourceCode_NoFunctionsInSource_EmptyFunctionList()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "bar();";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(0, functionMap.Count);
		}

		[Fact]
		public void ParseSourceCode_SingleLineFunctionInSource_CorrectZeroBasedColumnNumbers()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){bar();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(1, functionMap.Count);
			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(9, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(14, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_MultiLineFunctionInSource_CorrectColumnAndZeroBasedLineNumbers()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo()" + Environment.NewLine + "{" + Environment.NewLine + "bar();" +
								Environment.NewLine + "}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(1, functionMap.Count);
			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(9, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(1, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(3, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(1, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_TwoSingleLineFunctions_TwoFunctionMapEntries()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){bar();}function bar(){baz();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("bar", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(31, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(36, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(44, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(9, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(14, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_TwoNestedSingleLineFunctions_TwoFunctionMapEntries()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){function bar(){baz();}}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("bar", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(24, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(29, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(37, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(9, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(14, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(38, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_FunctionAssignedToVariable_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){bar();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(1, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(28, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_StaticMethod_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){};foo.bar = function() { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal("bar", functionMap[0].Bindings[1].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(44, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(54, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_InstanceMethod_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype.bar = function () { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo.prototype", functionMap[0].Bindings[0].Name);
			Assert.Equal("bar", functionMap[0].Bindings[1].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(55, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(65, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_InstanceMethodInObjectInitializer_FunctionMapEntryGenerated()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype = { bar: function () { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal("bar", functionMap[0].Bindings[1].Name);
			Assert.Equal(0, functionMap[0].Bindings[1].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(41, functionMap[0].Bindings[1].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(58, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(68, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_FunctionAssignedToVariableAndHasName_FunctionMapEntryGeneratedForVariableName()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function myCoolFunctionName(){ bar(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(1, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(39, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(49, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_StaticMethodAndFunctionHasName_FunctionMapEntryGeneratedForPropertyName()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){};foo.bar = function myCoolFunctionName() { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal("bar", functionMap[0].Bindings[1].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(63, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(73, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_InstanceMethodAndFunctionHasName_FunctionMapEntryGeneratedForObjectPrototype()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype.bar = function myCoolFunctionName() { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo.prototype", functionMap[0].Bindings[0].Name);
			Assert.Equal("bar", functionMap[0].Bindings[1].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(73, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(83, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void ParseSourceCode_InstanceMethodWithObjectInitializerAndFunctionHasName_FunctionMapEntryGeneratedForObjectPrototype()
		{
			// Arrange
			FunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype = { bar: function myCoolFunctionName() { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal("bar", functionMap[0].Bindings[1].Name);
			Assert.Equal(0, functionMap[0].Bindings[1].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(41, functionMap[0].Bindings[1].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(76, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(86, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.Equal("foo", functionMap[1].Bindings[0].Name);
			Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_NullFunctionMapEntry_ThrowsException()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = null;
			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();

			// Act
			Assert.Throws<ArgumentNullException>( ()=> FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap));
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_NullSourceMap_ThrowsException()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = CreateFunctionMapEntry();
			SourceMap sourceMap = null;

			// Act
			Assert.Throws<ArgumentNullException>( ()=> FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap));
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_NoBinding_ReturnNullMethodName()
		{
			// Arrange
			FunctionMapEntry functionMapEntry = CreateFunctionMapEntry();
			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();

			// Act
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.Null(result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>()
				{
					new BindingInformation
					{
						SourcePosition = new SourcePosition {ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 15}
					}
				};
			FunctionMapEntry functionMapEntry = CreateFunctionMapEntry(bindings);

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(Arg<SourcePosition>.Is.Anything)).Return(null);

			// Act
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.Null(result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_HasSingleBindingMatchingMapping_ReturnsMethodName()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>()
				{
					new BindingInformation
					{
						SourcePosition = new SourcePosition {ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 8}
					}
				};
			FunctionMapEntry functionMapEntry = CreateFunctionMapEntry(bindings);

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
			Assert.Equal("foo", result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_MatchingMappingMultipleBindingsMissingPrototypeMapping_ReturnsMethodName()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>
				{
					new BindingInformation
					{
						SourcePosition = new SourcePosition {ZeroBasedLineNumber = 86, ZeroBasedColumnNumber = 52}
					},
					new BindingInformation
					{
						SourcePosition = new SourcePosition {ZeroBasedLineNumber = 88, ZeroBasedColumnNumber = 78}
					}
				};
			FunctionMapEntry functionMapEntry = CreateFunctionMapEntry(bindings);

			SourceMap sourceMap = MockRepository.GenerateStub<SourceMap>();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 86 && y.ZeroBasedColumnNumber == 52)))
				.Return(null);

			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 88 && y.ZeroBasedColumnNumber == 78)))
				.Return(new MappingEntry
				{
					OriginalName = "baz",
				});

			// Act
			string result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.Equal("baz", result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_MatchingMappingMultipleBindings_ReturnsMethodNameWithFullBinding()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>
				{
					new BindingInformation
					{
						SourcePosition = new SourcePosition {ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 5}
					},
					new BindingInformation
					{
						SourcePosition = new SourcePosition {ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 10}
					}
				};
			FunctionMapEntry functionMapEntry = CreateFunctionMapEntry(bindings);

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
			Assert.Equal("bar.baz", result);
			sourceMap.VerifyAllExpectations();
		}

		private FunctionMapEntry CreateFunctionMapEntry(IReadOnlyList<BindingInformation> bindings = default)
		{
			return new FunctionMapEntry(
				bindings: bindings,
				startSourcePosition: default,
				endSourcePosition: default);
		}
	}
}