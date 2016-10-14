using System.Collections.Generic;
using System.IO;
using Microsoft.Ajax.Utilities;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal class FunctionMapGenerator : IFunctionMapGenerator
	{
		/// <summary>
		/// Returns a FunctionMap describing the locations of every funciton in the source code.
		/// The functions are to be sorted descending by start position.
		/// </summary>
		public List<FunctionMapEntry> GenerateFunctionMap(StreamReader sourceCodeStreamReader)
		{
			if (sourceCodeStreamReader == null)
			{
				return null;
			}

			string sourceCode = sourceCodeStreamReader.ReadToEnd();

			JSParser jsParser = new JSParser();

			// We just want the AST, don't let AjaxMin do any optimizations
			CodeSettings codeSettings = new CodeSettings { MinifyCode = false };

			Block block = jsParser.Parse(sourceCode, codeSettings);

			FunctionFinderVisitor functionFinderVisitor = new FunctionFinderVisitor();
			functionFinderVisitor.Visit(block);
			
			// Sort in descending order by start position
			functionFinderVisitor.FunctionMap.Sort((x, y) => y.StartSourcePosition.CompareTo(x.StartSourcePosition));
			return functionFinderVisitor.FunctionMap;
		}
	}
}
