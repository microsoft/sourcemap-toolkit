using System;
using System.Collections.Generic;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This class is responsible for providing function maps for the JavaScript code corresponding to a URL.
	/// </summary>
	internal class FunctionMapStore : IFunctionMapStore
	{
		private readonly IFunctionMapGenerator _functionMapGenerator;
		private readonly KeyValueCache<string,List<FunctionMapEntry>> _functionMapCache;

		public FunctionMapStore(ISourceCodeProvider sourceCodeProvider, Func<string, SourceMap?> sourceMapGetter)
		{
			_functionMapGenerator = new FunctionMapGenerator();
			_functionMapCache = new KeyValueCache<string, List<FunctionMapEntry>>(sourceCodeUrl => _functionMapGenerator.GenerateFunctionMap(
				sourceCodeProvider.GetSourceCode(sourceCodeUrl),
				sourceMapGetter(sourceCodeUrl)));
		}

		/// <summary>
		/// Returns a function map for the given URL. If the function map is not already available,
		/// it is obtained from the API consumer using the provided ISourceCodeProvider.
		/// Once a function map is generated, the value is cached in memory for future usages.
		/// </summary>
		/// <param name="sourceCodeUrl">The URL of the file for which a function map is required</param>
		public List<FunctionMapEntry>? GetFunctionMapForSourceCode(string? sourceCodeUrl)
		{
			if (sourceCodeUrl == null)
			{
				return null;
			}

			return _functionMapCache.GetValue(sourceCodeUrl);
		}
	}
}
