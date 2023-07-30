using SourcemapToolkit.SourcemapParser;
using System;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface ISourceMapStore
	{
		SourceMap GetSourceMapForUrl(string sourceCodeUrl);
	}

	/// <summary>
	/// This class is responsible for providing the source map that corresponds to a given JavaScript file.
	/// </summary>
	internal class SourceMapStore : ISourceMapStore
	{
		private readonly SourceMapParser _sourceMapParser;
		private readonly ISourceMapProvider _sourceMapProvider;
		private readonly IKeyValueCache<string, SourceMap> _sourceMapCache;

		public SourceMapStore(ISourceMapProvider sourceMapProvider, IKeyValueCache<string, SourceMap> keyValueCache, bool removeSourcesContent)
		{
			_sourceMapProvider = sourceMapProvider;
			_sourceMapParser = new SourceMapParser(removeSourcesContent);
			Func<string, SourceMap> valueGetter = sourceCodeUrl => _sourceMapParser.ParseSourceMap(_sourceMapProvider.GetSourceMapContentsForCallstackUrl(sourceCodeUrl));

			if (keyValueCache == null)
			{
				_sourceMapCache = new KeyValueCache<string, SourceMap>(valueGetter);
			}
			else
			{
				keyValueCache.SetValueGetter(valueGetter);
				_sourceMapCache = keyValueCache;
			}
		}

		/// <summary>
		/// Returns a source map for the given URL. If the source map is not already available,
		/// it is obtained from the API consumer using the provided ISourceMapProvider.
		/// Once a source map is generated, the value is cached in memory for future usages.
		/// </summary>
		/// <param name="sourceCodeUrl">The URL of the file for which a function map is required</param>
		public SourceMap GetSourceMapForUrl(string sourceCodeUrl)
		{
			return _sourceMapCache.GetValue(sourceCodeUrl);
		}
	}
}
