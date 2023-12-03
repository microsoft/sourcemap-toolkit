using System;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier;

public class StackTraceDeminifierFactory
{

	/// <summary>
	/// Creates a StackTraceDeminifier with full capabilities. StackTrace deminifiers created with
	/// this method will keep source maps cached, and thus use significantly more memory during
	/// runtime than the ones generated with GetMethodNameOnlyStackTraceDeminfier.
	/// </summary>
	/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which
	/// provides the source map for a given JavaScript file. Throws ArgumentNullException if the
	/// parameter is set to null.</param>
	/// <param name="generatedCodeProvider">Consumers of the API should implement this interface,
	/// which provides the contents of a JavaScript file. Throws ArgumentNullException if the
	/// parameter is set to null.</param>
	/// <param name="stackTraceParser">Consumers of the API should implement this interface,
	/// which provides a parser for the stacktrace. If null, default is used.</param>
	/// <param name="removeSourcesContent">Optional parameter that will remove "SourcesContent"
	/// data from the loaded source maps, which will use less memory for the cached map files</param>
	public static StackTraceDeminifier GetDeminifier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider, IStackTraceParser stackTraceParser = null, bool removeSourcesContent = false)
	{
		ArgumentNullException.ThrowIfNull(sourceMapProvider);
		ArgumentNullException.ThrowIfNull(generatedCodeProvider);

		var sourceMapStore = new SourceMapStore(sourceMapProvider, removeSourcesContent);
		var stackFrameDeminifier = new StackFrameDeminifier(sourceMapStore,
			new FunctionMapStore(generatedCodeProvider, sourceMapStore.GetSourceMapForUrl), new FunctionMapConsumer());

		return new(stackFrameDeminifier, stackTraceParser ?? new StackTraceParser());
	}

	/// <summary>
	/// Creates a StackTraceDeminifier which does not depend on JS files, and is ES2015+ compatible.
	/// StackTrace deminifiers created with this method will keep source maps cached, and thus use
	/// significantly more memory during runtime than the ones generated with GetMethodNameOnlyStackTraceDeminfier.
	/// </summary>
	/// <param name="sourceMapProvider">Consumers of the API should implement this interface, whichc
	/// provides the source map for a given JavaScript file. Throws ArgumentNullException if the
	/// parameter is set to null.</param>
	/// <param name="stackTraceParser">Consumers of the API should implement this interface, which provides
	/// a parser for the stacktrace. If null the default is used.</param>
	/// <param name="removeSourcesContent">Optional parameter that will remove "SourcesContent" data from
	/// the loaded source maps, which will use less memory for the cached map files</param>
	public static StackTraceDeminifier GetMapOnlyDeminifier(ISourceMapProvider sourceMapProvider, IStackTraceParser stackTraceParser = null, bool removeSourcesContent = false)
	{
        ArgumentNullException.ThrowIfNull(sourceMapProvider);
        ArgumentNullException.ThrowIfNull(sourceMapProvider);

        var sourceMapStore = new SourceMapStore(sourceMapProvider, removeSourcesContent);
		var stackFrameDeminifier = new StackFrameDeminifier(sourceMapStore);

		return new(stackFrameDeminifier, stackTraceParser ?? new StackTraceParser());
	}

	/// <summary>
	/// Creates a StackTraceDeminifier that only deminifies the method names. StackTrace deminifiers
	/// created with this method will use significantly less memory during runtime than the 
	/// </summary>
	/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which
	/// provides the source map for a given JavaScript file. Throws ArgumentNullException if the
	/// parameter is set to null.</param>
	/// <param name="generatedCodeProvider">Consumers of the API should implement this interface,
	/// which provides the contents of a JavaScript file. Throws ArgumentNullException if the
	/// parameter is set to null.</param>
	/// <param name="stackTraceParser">Consumers of the API should implement this interface, which
	/// provides a parser for the stacktrace. If null the default is used.
	/// </param>
	public static StackTraceDeminifier GetMethodNameDeminifier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider, IStackTraceParser stackTraceParser = null)
	{
		ArgumentNullException.ThrowIfNull(sourceMapProvider);
		ArgumentNullException.ThrowIfNull(generatedCodeProvider);

		var sourceMapParser = new SourceMapParser();
		var stackFrameDeminifier = new MethodNameStackFrameDeminifier(new FunctionMapStore(generatedCodeProvider, (url) => sourceMapParser.ParseSourceMap(sourceMapProvider.GetSourceMapContentsForCallstackUrl(url))), new FunctionMapConsumer());

		return new(stackFrameDeminifier, stackTraceParser ?? new StackTraceParser());
	}
}