using System;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	public class StackTraceDeminfierFactory
	{
		private static void ValidateArguments(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider)
		{
			if (sourceMapProvider == null)
			{
				throw new ArgumentNullException(nameof(sourceMapProvider));
			}

			if (generatedCodeProvider == null)
			{
				throw new ArgumentNullException(nameof(generatedCodeProvider));
			}
		}

		/// <summary>
		/// Creates a StackTraceDeminifier with full capabilities. StackTrace deminifiers created with this method will keep source maps cached, and thus use significantly more memory during runtime than the MethodNameStackTraceDeminifier.
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetStackTraceDeminfier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider)
		{
			ValidateArguments(sourceMapProvider, generatedCodeProvider);

			ISourceMapStore sourceMapStore = new SourceMapStore(sourceMapProvider);
			IStackFrameDeminifier stackFrameDeminifier = new StackFrameDeminifier(sourceMapStore,
				new FunctionMapStore(generatedCodeProvider, sourceMapStore.GetSourceMapForUrl), new FunctionMapConsumer());

			IStackTraceParser stackTraceParser = new StackTraceParser();

			return new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);
		}

		/// <summary>
		/// Creates a StackTraceDeminifier that only deminifies the method names. StackTrace deminifiers created with this method will use significantly less memory during runtime than the 
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetMethodNameOnlyStackTraceDeminfier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider)
		{
			ValidateArguments(sourceMapProvider, generatedCodeProvider);

			SourceMapParser sourceMapParser = new SourceMapParser();
			IStackFrameDeminifier stackFrameDeminifier = new SimpleStackFrameDeminifier(new FunctionMapStore(generatedCodeProvider, (url) => sourceMapParser.ParseSourceMap(sourceMapProvider.GetSourceMapContentsForCallstackUrl(url))), new FunctionMapConsumer());

			IStackTraceParser stackTraceParser = new StackTraceParser();

			return new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);
		}
	}
}