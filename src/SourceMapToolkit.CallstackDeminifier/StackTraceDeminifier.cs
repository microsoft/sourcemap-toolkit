using System;
using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Public API for parsing and deminifying browser stack traces.
	/// </summary>
	public class StackTraceDeminifier
	{
		private readonly IStackFrameDeminifier _stackFrameDeminifier;
		private readonly IStackTraceParser _stackTraceParser;
		
		/// <summary>
		/// This class is responsible for parsing a callstack string into
		/// a list of StackFrame objects and providing the deminified version
		/// of the stack frame.
		/// </summary>
		/// <param name="sourceMapProvider">Consumers of the API should implement this interface, which provides the source map for a given JavaScript file</param>
		/// <param name="generatedCodeProvider">Consumers of the API should implement this interface, which provides the contents of a JavaScript file</param>
		public StackTraceDeminifier(ISourceMapProvider sourceMapProvider, ISourceCodeProvider generatedCodeProvider)
			: this(new StackFrameDeminifier(new SourceMapStore(sourceMapProvider),
				new FunctionMapStore(generatedCodeProvider), new FunctionMapConsumer()), new StackTraceParser())
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

		internal StackTraceDeminifier(IStackFrameDeminifier stackFrameDeminifier, IStackTraceParser stackTraceParser)
		{
			_stackFrameDeminifier = stackFrameDeminifier;
			_stackTraceParser = stackTraceParser;
		}

		public DeminifyStackTraceResult DeminifyStackTrace(string stackTraceString)
		{
			DeminifyStackTraceResult result = new DeminifyStackTraceResult();
			result.MinifiedStackFrames = _stackTraceParser.ParseStackTrace(stackTraceString);
			result.DeminifiedStackFrames = new List<StackFrame>();

			foreach (StackFrame minifiedStackFrame in result.MinifiedStackFrames)
			{
				result.DeminifiedStackFrames.Add(_stackFrameDeminifier.DeminifyStackFrame(minifiedStackFrame));
			}

			return result;
		}
	}
}
