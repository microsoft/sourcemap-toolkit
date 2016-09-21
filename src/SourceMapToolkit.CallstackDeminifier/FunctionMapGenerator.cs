using System.Collections.Generic;
using Microsoft.Ajax.Utilities;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IFunctionMapGenerator
	{
		/// <summary>
		/// Returns a FunctionMap describing the locations of every funciton in the source code.
		/// The functions are to be sorted by start position.
		/// </summary>
		List<FunctionMapEntry> GenerateFunctionMap(string sourceCode);
	}

	internal class FunctionMapGenerator : IFunctionMapGenerator
	{
		/// <summary>
		/// Returns a FunctionMap describing the locations of every funciton in the source code.
		/// The functions are to be sorted by start position.
		/// </summary>
		public List<FunctionMapEntry> GenerateFunctionMap(string sourceCode)
		{
			JSParser jsParser = new JSParser();

			// We just want the AST, don't let AjaxMin do any optimizations
			CodeSettings codeSettings = new CodeSettings { MinifyCode = false };

			Block block = jsParser.Parse(sourceCode, codeSettings);

			FunctionFinderVisitor functionFinderVisitor = new FunctionFinderVisitor();
			functionFinderVisitor.Visit(block);

			functionFinderVisitor.FunctionMap.Sort((x, y) => x.StartSourcePosition.CompareTo(y.StartSourcePosition));
			return functionFinderVisitor.FunctionMap;
		}
	}
}
