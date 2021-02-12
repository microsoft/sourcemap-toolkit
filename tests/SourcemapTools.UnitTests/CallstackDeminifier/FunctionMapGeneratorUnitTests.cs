using System;
using System.Collections.Generic;
using Moq;
using SourcemapToolkit.SourcemapParser;
using SourcemapToolkit.SourcemapParser.UnitTests;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class FunctionMapGeneratorUnitTests
	{
		[Fact]
		public void GenerateFunctionMap_NullSourceMap_ReturnsNull()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			var sourceCode = "";

			// Act
			var functionMap = functionMapGenerator.GenerateFunctionMap(UnitTestUtils.StreamFromString(sourceCode), null);

			// Assert
			Assert.Null(functionMap);
		}


		[Fact]
		public void ParseSourceCode_NoFunctionsInSource_EmptyFunctionList()
		{
			// Arrange
			var sourceCode = "bar();";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Empty(functionMap);
		}

		[Fact]
		public void ParseSourceCode_SingleLineFunctionInSource_CorrectZeroBasedColumnNumbers()
		{
			// Arrange
			var sourceCode = "function foo(){bar();}";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Single(functionMap);
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
			var sourceCode = "function foo()" + Environment.NewLine + "{" + Environment.NewLine + "bar();" +
								Environment.NewLine + "}";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Single(functionMap);
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
			var sourceCode = "function foo(){bar();}function bar(){baz();}";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

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
			var sourceCode = "function foo(){function bar(){baz();}}";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

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
			var sourceCode = "var foo = function(){bar();}";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Single(functionMap);

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
			var sourceCode = "var foo = function(){};foo.bar = function() { baz(); }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

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
			var sourceCode = "var foo = function(){}; foo.prototype.bar = function () { baz(); }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal("prototype", functionMap[0].Bindings[1].Name);
			Assert.Equal("bar", functionMap[0].Bindings[2].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(24, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(56, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(66, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

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
			var sourceCode = "var foo = function(){}; foo.prototype = { bar: function () { baz(); } }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(24, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal("prototype", functionMap[0].Bindings[1].Name);
			Assert.Equal("bar", functionMap[0].Bindings[2].Name);
			Assert.Equal(0, functionMap[0].Bindings[2].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(42, functionMap[0].Bindings[2].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(59, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(69, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

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
			var sourceCode = "var foo = function myCoolFunctionName(){ bar(); }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Single(functionMap);

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
			var sourceCode = "var foo = function(){};foo.bar = function myCoolFunctionName() { baz(); }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

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
			var sourceCode = "var foo = function(){}; foo.prototype.bar = function myCoolFunctionName() { baz(); }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal("prototype", functionMap[0].Bindings[1].Name);
			Assert.Equal("bar", functionMap[0].Bindings[2].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(24, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(74, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(84, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

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
			var sourceCode = "var foo = function(){}; foo.prototype = { bar: function myCoolFunctionName() { baz(); } }";

			// Act
			var functionMap = FunctionMapGenerator.ParseSourceCode(UnitTestUtils.StreamFromString(sourceCode));

			// Assert
			Assert.Equal(2, functionMap.Count);

			Assert.Equal("foo", functionMap[0].Bindings[0].Name);
			Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(24, functionMap[0].Bindings[0].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal("prototype", functionMap[0].Bindings[1].Name);
			Assert.Equal("bar", functionMap[0].Bindings[2].Name);
			Assert.Equal(0, functionMap[0].Bindings[2].SourcePosition.ZeroBasedLineNumber);
			Assert.Equal(42, functionMap[0].Bindings[2].SourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(77, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(87, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

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
			FunctionMapEntry? functionMapEntry = null;
			var sourceMap = new Mock<SourceMap>().Object;

			// Act
			Assert.Throws<ArgumentNullException>(() => FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry!, sourceMap));
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_NullSourceMap_ThrowsException()
		{
			// Arrange
			var functionMapEntry = new FunctionMapEntry(null!, null!, null!);
			SourceMap? sourceMap = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap!));
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_NoBinding_ReturnNullMethodName()
		{
			// Arrange
			var functionMapEntry = new FunctionMapEntry(null!, null!, null!);
			var sourceMap = new Mock<SourceMap>().Object;

			// Act
			var result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
		{
			// Arrange
			var functionMapEntry = new FunctionMapEntry(
					new List<BindingInformation>
					{
						new BindingInformation(
							null!,
							new SourcePosition {ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 15})
					}, null!, null!);

			var sourceMap = new Mock<SourceMap>();
			sourceMap.Setup(x => x.GetMappingEntryForGeneratedSourcePosition(It.IsAny<SourcePosition>())).Returns<MappingEntry?>(null);

			// Act
			var result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap.Object);

			// Assert
			Assert.Null(result);
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_HasSingleBindingMatchingMapping_ReturnsMethodName()
		{
			// Arrange
			var functionMapEntry = new FunctionMapEntry(
					new List<BindingInformation>
					{
						new BindingInformation(
							null!,
							new SourcePosition {ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 8})
					}, null!, null!);

			var sourceMap = new Mock<SourceMap>();
			sourceMap.Setup(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						It.Is<SourcePosition>(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 8)))
				.Returns(new MappingEntry
				{
					OriginalName = "foo",
				});

			// Act
			var result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap.Object);

			// Assert
			Assert.Equal("foo", result);
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_MatchingMappingMultipleBindingsMissingPrototypeMapping_ReturnsMethodName()
		{
			// Arrange
			var functionMapEntry = new FunctionMapEntry(
					new List<BindingInformation>
					{
						new BindingInformation(
							null!,
							new SourcePosition {ZeroBasedLineNumber = 86, ZeroBasedColumnNumber = 52}),
						new BindingInformation(
							null!,
							new SourcePosition {ZeroBasedLineNumber = 88, ZeroBasedColumnNumber = 78})
					}, null!, null!);

			var sourceMap = new Mock<SourceMap>();
			sourceMap.Setup(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						It.Is<SourcePosition>(y => y.ZeroBasedLineNumber == 86 && y.ZeroBasedColumnNumber == 52)))
				.Returns<MappingEntry?>(null);

			sourceMap.Setup(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						It.Is<SourcePosition>(y => y.ZeroBasedLineNumber == 88 && y.ZeroBasedColumnNumber == 78)))
				.Returns(new MappingEntry
				{
					OriginalName = "baz",
				});

			// Act
			var result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap.Object);

			// Assert
			Assert.Equal("baz", result);
		}

		[Fact]
		public void GetDeminifiedMethodNameFromSourceMap_MatchingMappingMultipleBindings_ReturnsMethodNameWithFullBinding()
		{
			// Arrange
			var functionMapEntry = new FunctionMapEntry(
					new List<BindingInformation>
					{
						new BindingInformation(
							null!,
							new SourcePosition {ZeroBasedLineNumber = 5, ZeroBasedColumnNumber = 5}),
						new BindingInformation(
							null!,
							new SourcePosition {ZeroBasedLineNumber = 20, ZeroBasedColumnNumber = 10})
					}, null!, null!);

			var sourceMap = new Mock<SourceMap>();
			sourceMap.Setup(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						It.Is<SourcePosition>(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 5)))
				.Returns(new MappingEntry
				{
					OriginalName = "bar"
				});

			sourceMap.Setup(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						It.Is<SourcePosition>(y => y.ZeroBasedLineNumber == 20 && y.ZeroBasedColumnNumber == 10)))
				.Returns(new MappingEntry
				{
					OriginalName = "baz",
				});

			// Act
			var result = FunctionMapGenerator.GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap.Object);

			// Assert
			Assert.Equal("bar.baz", result);
		}
	}
}