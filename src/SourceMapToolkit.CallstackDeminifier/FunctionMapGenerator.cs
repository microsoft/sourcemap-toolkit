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

            List<FunctionMapEntry> result = ParseSourceCode(sourceCodeStreamReader);

            foreach (FunctionMapEntry functionMapEntry in result)
            {
                functionMapEntry.DeminfifiedMethodName = GetDeminifiedMethodNameFromSourceMap(functionMapEntry, sourceMap);
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

	    internal static string GetDeminifiedMethodNameFromSourceMap(FunctionMapEntry wrappingFunction, SourceMap sourceMap)
	    {
	        string methodName = null;

	        if (wrappingFunction?.Bindings != null && wrappingFunction.Bindings.Count > 0)
	        {
	            if (wrappingFunction.Bindings.Count == 2)
	            {
	                MappingEntry objectProtoypeMappingEntry =
	                    sourceMap?.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings[0].SourcePosition);

	                methodName = objectProtoypeMappingEntry?.OriginalName;
	            }

	            MappingEntry mappingEntry =
	                sourceMap?.GetMappingEntryForGeneratedSourcePosition(wrappingFunction.Bindings.Last().SourcePosition);

	            if (mappingEntry != null)
	            {
	                if (mappingEntry.OriginalName != null)
	                {
	                    if (methodName != null)
	                    {
	                        methodName = methodName + "." + mappingEntry.OriginalName;
	                    }
	                    else
	                    {
	                        methodName = mappingEntry.OriginalName;
	                    }
	                }
	            }
	        }
	        return methodName;
	    }
	}
}
