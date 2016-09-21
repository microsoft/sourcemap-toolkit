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

			Assert.AreEqual("foo", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(22, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("bar", functionMap[1].FunctionName);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(36, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(44, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
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

			Assert.AreEqual("foo", functionMap[0].FunctionName);
			Assert.AreEqual(0, functionMap[0].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[0].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(14, functionMap[0].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(38, functionMap[0].EndSourcePosition.ZeroBasedColumnNumber);

			Assert.AreEqual("bar", functionMap[1].FunctionName);
			Assert.AreEqual(0, functionMap[1].StartSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(0, functionMap[1].EndSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(29, functionMap[1].StartSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(37, functionMap[1].EndSourcePosition.ZeroBasedColumnNumber);
		}
	}
}