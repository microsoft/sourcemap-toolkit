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

			var sourceCode = sourceCodeStreamReader.ReadToEnd();
			sourceCodeStreamReader.Close();

			var jsParser = new JSParser();

			// We just want the AST, don't let AjaxMin do any optimizations
			var codeSettings = new CodeSettings { MinifyCode = false };

			var block = jsParser.Parse(sourceCode, codeSettings);

			var functionFinderVisitor = new FunctionFinderVisitor(sourceMap);
			functionFinderVisitor.Visit(block);

			// Sort in descending order by start position.  This allows the first result found in a linear search to be the "closest function to the [consumer's] source position".
			//
			// ATTN: It may be possible to do this with an ascending order sort, followed by a series of binary searches on rows & columns.
			//       Our current profiles show the memory pressure being a bigger issue than the stack lookup, so I'm leaving this for now. 
			functionFinderVisitor.FunctionMap.Sort((x, y) => y.StartSourcePosition.CompareTo(x.StartSourcePosition));

			return functionFinderVisitor.FunctionMap;
		}
	}
}
