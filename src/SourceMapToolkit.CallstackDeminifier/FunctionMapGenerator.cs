using System.Collections.Generic;
using System.IO;

using Microsoft.Ajax.Utilities;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal class FunctionMapGenerator : IFunctionMapGenerator
	{
		/// <summary>
		/// Returns a FunctionMap describing the locations of every funciton in the source code.
		/// The functions are to be sorted descending by start position.
		/// </summary>
		public IReadOnlyList<FunctionMapEntry> GenerateFunctionMap(StreamReader sourceCodeStreamReader, SourceMap sourceMap)
		{
			if (sourceCodeStreamReader == null || sourceMap == null)
			{
				return null;
			}

			IReadOnlyList<FunctionMapEntry> result;
			try
			{
				result = ParseSourceCode(sourceCodeStreamReader, sourceMap);
			}
			catch
			{
				// Failed to parse JavaScript source. This is common as the JS parser does not support ES2015+.
				// Continue to regular source map deminification.
				result = null;
			}

			return result;
		}

		/// <summary>
		/// Iterates over all the code in the JavaScript file to get a list of all the functions declared in that file.
		/// </summary>
		internal IReadOnlyList<FunctionMapEntry> ParseSourceCode(StreamReader sourceCodeStreamReader, SourceMap sourceMap)
		{
			if (sourceCodeStreamReader == null)
			{
				return null;
			}

			string sourceCode = sourceCodeStreamReader.ReadToEnd();
			sourceCodeStreamReader.Close();

			JSParser jsParser = new JSParser();

			// We just want the AST, don't let AjaxMin do any optimizations
			CodeSettings codeSettings = new CodeSettings { MinifyCode = false };

			Block block = jsParser.Parse(sourceCode, codeSettings);

			FunctionFinderVisitor functionFinderVisitor = new FunctionFinderVisitor(sourceMap);
			functionFinderVisitor.Visit(block);
			
			// Sort in descending order by start position
			functionFinderVisitor.FunctionMap.Sort((x, y) => y.StartSourcePosition.CompareTo(x.StartSourcePosition));

			return functionFinderVisitor.FunctionMap;
		}
	}
}
