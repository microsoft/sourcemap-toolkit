using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		public List<FunctionMapEntry> GenerateFunctionMap(StreamReader sourceCodeStreamReader, SourceMap sourceMap)
		{
			if (sourceCodeStreamReader == null || sourceMap == null)
			{
				return null;
			}

			List<FunctionMapEntry> result;
			try
			{
				result = ParseSourceCode(sourceCodeStreamReader);

				foreach (FunctionMapEntry functionMapEntry in result)
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
		internal List<FunctionMapEntry> ParseSourceCode(StreamReader sourceCodeStreamReader)
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

			FunctionFinderVisitor functionFinderVisitor = new FunctionFinderVisitor();
			functionFinderVisitor.Visit(block);
			
			// Sort in descending order by start position
			functionFinderVisitor.FunctionMap.Sort((x, y) => y.StartSourcePosition.CompareTo(x.StartSourcePosition));

			return functionFinderVisitor.FunctionMap;
		}

		/// <summary>
		/// Gets the original name corresponding to a function based on the information provided in the source map.
		/// </summary>
		internal static string GetDeminifiedMethodNameFromSourceMap(FunctionMapEntry wrappingFunction, SourceMap sourceMap)
		{
			if (wrappingFunction == null)
			{
				throw new ArgumentNullException(nameof(wrappingFunction));
			}

			if (sourceMap == null)
			{
				throw new ArgumentNullException(nameof(sourceMap));
			}

			string methodName = null;

			if (wrappingFunction.Bindings != null && wrappingFunction.Bindings.Count > 0)
			{
				MappingEntry objectProtoypeMappingEntry = null;
				if (wrappingFunction.Bindings.Count == 2)
				{
					objectProtoypeMappingEntry =
						sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings[0].SourcePosition);
				}

				MappingEntry mappingEntry =
					sourceMap.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings.Last().SourcePosition);

				if (mappingEntry?.OriginalName != null)
				{
					if (objectProtoypeMappingEntry?.OriginalName != null)
					{
						string objectName = objectProtoypeMappingEntry.OriginalName;
						if (objectProtoypeMappingEntry.OriginalSourcePosition?.ZeroBasedColumnNumber == mappingEntry.OriginalSourcePosition?.ZeroBasedColumnNumber
							&& objectProtoypeMappingEntry.OriginalSourcePosition?.ZeroBasedLineNumber == mappingEntry.OriginalSourcePosition?.ZeroBasedLineNumber
							&& objectName.EndsWith($".{mappingEntry.OriginalName}"))
						{
							// The object name already contains the method name, so do not append it
							methodName = objectName;
						}
						else
						{
							methodName = $"{objectName}.{mappingEntry.OriginalName}";
						}
					}
					else
					{
						methodName = mappingEntry.OriginalName;
					}
				}
			}
			return methodName;
		}
	}
}
