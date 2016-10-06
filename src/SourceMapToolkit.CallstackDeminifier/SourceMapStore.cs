using SourcemapToolkit.SourcemapParser;

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
		private readonly KeyValueCache<string, SourceMap> _sourceMapCache;

		public SourceMapStore(ISourceMapProvider sourceMapProvider)
		{
			_sourceMapProvider = sourceMapProvider;
			_sourceMapParser = new SourceMapParser();
			_sourceMapCache = new KeyValueCache<string, SourceMap>(sourceCodeUrl => _sourceMapParser.ParseSourceMap(_sourceMapProvider.GetSourceMapContentsForCallstackUrl(sourceCodeUrl)));
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
