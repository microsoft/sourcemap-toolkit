using System;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	public class StackTraceDeminfierFactory
	{
		private static void ValidateArguments(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider, IStackTraceParser stackTraceParser)
		{
			if (sourceMapProvider == null)
			{
				throw new ArgumentNullException(nameof(sourceMapProvider));
			}

			if (generatedCodeProvider == null)
			{
				throw new ArgumentNullException(nameof(generatedCodeProvider));
			}

			if (stackTraceParser == null)
			{
				throw new ArgumentNullException(nameof(stackTraceParser));
			}
		}

		/// <summary>
		/// Creates a StackTraceDeminifier with full capabilities. StackTrace deminifiers created with this method will keep source maps cached, and thus use significantly more memory during runtime than the ones generated with GetMethodNameOnlyStackTraceDeminfier.
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetStackTraceDeminfier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider)
		{
			return GetStackTraceDeminfier(sourceMapProvider, generatedCodeProvider, new StackTraceParser());
		}

		/// <summary>
		/// Creates a StackTraceDeminifier with full capabilities. StackTrace deminifiers created with this method will keep source maps cached, and thus use significantly more memory during runtime than the ones generated with GetMethodNameOnlyStackTraceDeminfier.
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="stackTraceParser">Consumers of the API should implement this interface, which provides a parser for the stacktrace. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetStackTraceDeminfier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider, IStackTraceParser stackTraceParser)
		{
			ValidateArguments(sourceMapProvider, generatedCodeProvider, stackTraceParser);

			ISourceMapStore sourceMapStore = new SourceMapStore(sourceMapProvider);
			IStackFrameDeminifier stackFrameDeminifier = new StackFrameDeminifier(sourceMapStore,
				new FunctionMapStore(generatedCodeProvider, sourceMapStore.GetSourceMapForUrl), new FunctionMapConsumer());

			return new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);
		}

		/// <summary>
		/// Creates a StackTraceDeminifier which does not depend on JS files, and is ES2015+ compatible.
		/// StackTrace deminifiers created with this method will keep source maps cached, and thus use significantly more memory during runtime than the ones generated with GetMethodNameOnlyStackTraceDeminfier.
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetMapOnlyStackTraceDeminfier(ISourceMapProvider sourceMapProvider)
		{
			return GetMapOnlyStackTraceDeminfier(sourceMapProvider, new StackTraceParser());
		}

		/// <summary>
		/// Creates a StackTraceDeminifier which does not depend on JS files, and is ES2015+ compatible.
		/// StackTrace deminifiers created with this method will keep source maps cached, and thus use significantly more memory during runtime than the ones generated with GetMethodNameOnlyStackTraceDeminfier.
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="stackTraceParser">Consumers of the API should implement this interface, which provides a parser for the stacktrace. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetMapOnlyStackTraceDeminfier(ISourceMapProvider sourceMapProvider, IStackTraceParser stackTraceParser)
		{
			if (sourceMapProvider == null)
			{
				throw new ArgumentNullException(nameof(sourceMapProvider));
			}

			if (stackTraceParser == null)
			{
				throw new ArgumentNullException(nameof(stackTraceParser));
			}

			ISourceMapStore sourceMapStore = new SourceMapStore(sourceMapProvider);
			IStackFrameDeminifier stackFrameDeminifier = new StackFrameDeminifier(sourceMapStore);

			return new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);
		}

		/// <summary>
		/// Creates a StackTraceDeminifier that only deminifies the method names. StackTrace deminifiers created with this method will use significantly less memory during runtime than the 
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetMethodNameOnlyStackTraceDeminfier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider)
		{
			return GetMethodNameOnlyStackTraceDeminfier(sourceMapProvider, generatedCodeProvider, new StackTraceParser());
		}

		/// <summary>
		/// Creates a StackTraceDeminifier that only deminifies the method names. StackTrace deminifiers created with this method will use significantly less memory during runtime than the 
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file. Throws ArgumentNullException if the parameter is set to null.</param>
		/// <param name="stackTraceParser">Consumers of the API should implement this interface, which provides a parser for the stacktrace. Throws ArgumentNullException if the parameter is set to null.</param>
		public static StackTraceDeminifier GetMethodNameOnlyStackTraceDeminfier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider, IStackTraceParser stackTraceParser)
		{
			ValidateArguments(sourceMapProvider, generatedCodeProvider, stackTraceParser);

			IStackFrameDeminifier stackFrameDeminifier = new MethodNameStackFrameDeminifier(new FunctionMapStore(generatedCodeProvider, (url) => SourceMapParser.ParseSourceMap(sourceMapProvider.GetSourceMapContentsForCallstackUrl(url))), new FunctionMapConsumer());

			return new StackTraceDeminifier(stackFrameDeminifier, stackTraceParser);
		}
	}
}