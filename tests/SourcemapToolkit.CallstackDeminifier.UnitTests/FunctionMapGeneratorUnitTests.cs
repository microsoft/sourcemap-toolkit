using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class FunctionMapGeneratorUnitTests
	{
		[TestMethod]
		public void GenerateFunctionMap_NoFunctionsInSource_EmptyFunctionList()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "bar();";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(0, functionMap.Count);
		}

		[TestMethod]
		public void GenerateFunctionMap_SingleLineFunctionInSource_CorrectZeroBasedColumnNumbers()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){bar();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(1, functionMap.Count);
			Assert.AreEqual("foo", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GenerateFunctionMap_MultiLineFunctionInSource_CorrectColumnAndZeroBasedLineNumbers()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo()" + Environment.NewLine + "{" + Environment.NewLine + "bar();" +
								Environment.NewLine + "}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(1, functionMap.Count);
			Assert.AreEqual("foo", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(1, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(3, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(1, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GenerateFunctionMap_TwoSingleLineFunctions_TwoFunctionMapEntries()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){bar();}function bar(){baz();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("bar", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(31, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(36, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(44, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].FunctionName);
			Assert.AreEqual(0, functionMap[1].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[1].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GenerateFunctionMap_TwoNestedSingleLineFunctions_TwoFunctionMapEntries()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "function foo(){function bar(){baz();}}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("bar", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(24, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(29, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(37, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].FunctionName);
			Assert.AreEqual(0, functionMap[1].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(9, functionMap[1].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(38, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GenerateFunctionMap_FunctionAssignedToVariable_FunctionMapEntryGenerated()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){bar();}";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(1, functionMap.Count);

			Assert.AreEqual("foo", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(28, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GenerateFunctionMap_FunctionAssignedToObjectProperty_FunctionMapEntryGenerated()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){};foo.bar = function() { baz(); }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("foo.bar", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(23, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(44, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(54, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].FunctionName);
			Assert.AreEqual(0, functionMap[1].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(20, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}

		[TestMethod]
		public void GenerateFunctionMap_FunctionAssignedToObjectPrototype_FunctionMapEntryGenerated()
		{
			// Arrange
			IFunctionMapGenerator functionMapGenerator = new FunctionMapGenerator();
			string sourceCode = "var foo = function(){} foo.prototype = { bar: function () { baz(); } }";

			// Act
			List<FunctionMapEntry> functionMap = functionMapGenerator.GenerateFunctionMap(sourceCode);

			// Assert
			Assert.AreEqual(2, functionMap.Count);

			Assert.AreEqual("bar", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(41, functionMap[0].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(29, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(37, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("foo", functionMap[1].FunctionName);
			Assert.AreEqual(0, functionMap[1].FunctionNameSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(4, functionMap[1].FunctionNameSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(58, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(68, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}
	}
}