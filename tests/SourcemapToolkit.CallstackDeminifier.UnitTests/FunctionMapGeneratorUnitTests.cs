using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using SourcemapToolkit.SourcemapParser;
using SourcemapToolkit.SourcemapParser.UnitTests;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class FunctionMapGeneratorUnitTests
{
	[Fact]
	public void GenerateFunctionMap_NullSourceMap_ReturnsNull()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "";

		// Act
		var functionMap = functionMapGenerator.GenerateFunctionMap(UnitTestUtils.StreamReaderFromString(sourceCode), null);

		// Assert
		Assert.Null(functionMap);
	}


	[Fact]
	public void ParseSourceCode_NullInput_ReturnsNull()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(null, sourceMap);

		// Assert
		Assert.Null(functionMap);
	}

	[Fact]
	public void ParseSourceCode_NoFunctionsInSource_EmptyFunctionList()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "bar();";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Empty(functionMap);
	}

	[Fact]
	public void ParseSourceCode_SingleLineFunctionInSource_CorrectZeroBasedColumnNumbers()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "function foo(){bar();}";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Single(functionMap);
		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(9, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(14, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[0].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_MultiLineFunctionInSource_CorrectColumnAndZeroBasedLineNumbers()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "function foo()" + Environment.NewLine + "{" + Environment.NewLine + "bar();" +
		                 Environment.NewLine + "}";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Single(functionMap);
		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(9, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(1, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(3, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(1, functionMap[0].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_TwoSingleLineFunctions_TwoFunctionMapEntries()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "function foo(){bar();}function bar(){baz();}";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("bar", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(31, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(36, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(44, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(9, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(14, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_TwoNestedSingleLineFunctions_TwoFunctionMapEntries()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "function foo(){function bar(){baz();}}";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("bar", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(24, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(29, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(37, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(9, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(14, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(38, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_FunctionAssignedToVariable_FunctionMapEntryGenerated()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){bar();}";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Single(functionMap);

		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(28, functionMap[0].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_StaticMethod_FunctionMapEntryGenerated()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){};foo.bar = function() { baz(); }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal("bar", functionMap[0].Bindings[1].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(44, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(54, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_InstanceMethod_FunctionMapEntryGenerated()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){} foo.prototype.bar = function () { baz(); }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("foo.prototype", functionMap[0].Bindings[0].Name);
		Assert.Equal("bar", functionMap[0].Bindings[1].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(55, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(65, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_InstanceMethodInObjectInitializer_FunctionMapEntryGenerated()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){} foo.prototype = { bar: function () { baz(); } }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal("bar", functionMap[0].Bindings[1].Name);
		Assert.Equal(0, functionMap[0].Bindings[1].SourcePosition.Line);
		Assert.Equal(41, functionMap[0].Bindings[1].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(58, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(68, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_FunctionAssignedToVariableAndHasName_FunctionMapEntryGeneratedForVariableName()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function myCoolFunctionName(){ bar(); }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Single(functionMap);

		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(39, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(49, functionMap[0].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_StaticMethodAndFunctionHasName_FunctionMapEntryGeneratedForPropertyName()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){};foo.bar = function myCoolFunctionName() { baz(); }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal("bar", functionMap[0].Bindings[1].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(63, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(73, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_InstanceMethodAndFunctionHasName_FunctionMapEntryGeneratedForObjectPrototype()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){} foo.prototype.bar = function myCoolFunctionName() { baz(); } }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("foo.prototype", functionMap[0].Bindings[0].Name);
		Assert.Equal("bar", functionMap[0].Bindings[1].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(73, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(83, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	[Fact]
	public void ParseSourceCode_InstanceMethodWithObjectInitializerAndFunctionHasName_FunctionMapEntryGeneratedForObjectPrototype()
	{
		// Arrange
		var functionMapGenerator = new FunctionMapGenerator();
		var sourceCode = "var foo = function(){} foo.prototype = { bar: function myCoolFunctionName() { baz(); } }";
		var sourceMap = CreateSourceMapMock();

		// Act
		var functionMap = functionMapGenerator.ParseSourceCode(UnitTestUtils.StreamReaderFromString(sourceCode), sourceMap);

		// Assert
		Assert.Equal(2, functionMap.Count);

		Assert.Equal("foo", functionMap[0].Bindings[0].Name);
		Assert.Equal(0, functionMap[0].Bindings[0].SourcePosition.Line);
		Assert.Equal(23, functionMap[0].Bindings[0].SourcePosition.Column);
		Assert.Equal("bar", functionMap[0].Bindings[1].Name);
		Assert.Equal(0, functionMap[0].Bindings[1].SourcePosition.Line);
		Assert.Equal(41, functionMap[0].Bindings[1].SourcePosition.Column);
		Assert.Equal(0, functionMap[0].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[0].EndSourcePosition.Line);
		Assert.Equal(76, functionMap[0].StartSourcePosition.Column);
		Assert.Equal(86, functionMap[0].EndSourcePosition.Column);

		Assert.Equal("foo", functionMap[1].Bindings[0].Name);
		Assert.Equal(0, functionMap[1].Bindings[0].SourcePosition.Line);
		Assert.Equal(4, functionMap[1].Bindings[0].SourcePosition.Column);
		Assert.Equal(0, functionMap[1].StartSourcePosition.Line);
		Assert.Equal(0, functionMap[1].EndSourcePosition.Line);
		Assert.Equal(20, functionMap[1].StartSourcePosition.Column);
		Assert.Equal(22, functionMap[1].EndSourcePosition.Column);
	}

	private static SourceMap CreateSourceMapMock()
	{
		return new Mock<SourceMap>(MockBehavior.Default,
			0 /* version */,
			default(string) /* file */,
			default(string) /* mappings */,
			default(IReadOnlyList<string>) /* sources */,
			default(IReadOnlyList<string>) /* names */,
			default(IReadOnlyList<MappingEntry>) /* parsedMappings */,
			default(IReadOnlyList<string>)).Object;
	}
}