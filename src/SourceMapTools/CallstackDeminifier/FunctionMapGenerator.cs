using System;
using System.Collections.Generic;
using System.IO;
using Esprima;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal class FunctionMapGenerator : IFunctionMapGenerator
	{
		/// <summary>
		/// Returns a FunctionMap describing the locations of every funciton in the source code.
		/// The functions are to be sorted descending by start position.
		/// </summary>
		List<FunctionMapEntry>? IFunctionMapGenerator.GenerateFunctionMap(Stream? sourceCodeStream, SourceMap? sourceMap)
		{
			if (sourceCodeStream == null || sourceMap == null)
			{
				return null;
			}

			List<FunctionMapEntry>? result;
			try
			{
				result = ParseSourceCode(sourceCodeStream);

				foreach (var functionMapEntry in result)
				{
					functionMapEntry.DeminfifiedMethodName = GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);
				}
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
		internal static List<FunctionMapEntry> ParseSourceCode(Stream sourceCodeStream)
		{
			string sourceCode;
			using (sourceCodeStream)
			using (var sr = new StreamReader(sourceCodeStream))
			{
				sourceCode = sr.ReadToEnd();
			}

			var jsParser = new JavaScriptParser(sourceCode);

			var script = jsParser.ParseScript();

			var functionFinderVisitor = new FunctionFinderVisitor();
			functionFinderVisitor.Visit(script);

			// Sort in descending order by start position
			functionFinderVisitor.FunctionMap.Sort((x, y) => y.StartSourcePosition.CompareTo(x.StartSourcePosition));

			return functionFinderVisitor.FunctionMap;
		}

		/// <summary>
		/// Gets the original name corresponding to a function based on the information provided in the source map.
		/// </summary>
		internal static string? GetDeminifiedMethodNameFromSourceMap(FunctionMapEntry wrappingFunction, SourceMap sourceMap)
		{
			if (wrappingFunction == null)
			{
				throw new ArgumentNullException(nameof(wrappingFunction));
			}

			if (sourceMap == null)
			{
				throw new ArgumentNullException(nameof(sourceMap));
			}

			if (wrappingFunction.Bindings != null && wrappingFunction.Bindings.Count > 0)
			{
				var entryNames = new List<string>();

				foreach (var binding in wrappingFunction.Bindings)
				{
					var entry = sourceMap.GetMappingEntryForGeneratedSourcePosition(binding.SourcePosition);
					if (entry != null && entry.OriginalName != null)
					{
						entryNames.Add(entry.OriginalName);
					}
				}

				// // The object name already contains the method name, so do not append it
				if (entryNames.Count > 1 && entryNames[entryNames.Count - 2].EndsWith("." + entryNames[entryNames.Count - 1], StringComparison.Ordinal))
				{
					entryNames.RemoveAt(entryNames.Count - 1);
				}

				if (entryNames.Count > 2 && entryNames[entryNames.Count - 2] == "prototype")
				{
					entryNames.RemoveAt(entryNames.Count - 2);
				}

				if (entryNames.Count > 0)
				{
					return string.Join(".", entryNames);
				}
			}

			return null;
		}
	}
}
