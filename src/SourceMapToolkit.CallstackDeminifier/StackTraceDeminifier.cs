using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This class is responsible for parsing a callstack string into
	/// a list of StackFrame objects and providing the deminified version
	/// of the stack frame.
	/// </summary>
	public class StackTraceDeminifier
	{
		private readonly IStackFrameDeminifier _stackFrameDeminifier;
		private readonly IStackTraceParser _stackTraceParser;

		internal StackTraceDeminifier(IStackFrameDeminifier stackFrameDeminifier, IStackTraceParser stackTraceParser)
		{
			_stackFrameDeminifier = stackFrameDeminifier;
			_stackTraceParser = stackTraceParser;
		}

		/// <summary>
		/// Parses and deminifies a string containing a minified stack trace.
		/// </summary>
		public DeminifyStackTraceResult DeminifyStackTrace(string stackTraceString, bool preferSourceMapsSymbols = false)
		{
			var minifiedStackFrames = _stackTraceParser.ParseStackTrace(stackTraceString, out string message);
			var deminifiedStackFrameResults = new List<StackFrameDeminificationResult>(minifiedStackFrames.Count);

			// Deminify frames in reverse order so we can pass the symbol name from caller
			// (i.e. the function name) into the next level's deminification.
			string callerSymbolName = null;
			for (int i = minifiedStackFrames.Count - 1; i >= 0; i--)
			{
				var frame = _stackFrameDeminifier.DeminifyStackFrame(minifiedStackFrames[i], callerSymbolName, preferSourceMapsSymbols);
				callerSymbolName = frame?.DeminifiedSymbolName;
				deminifiedStackFrameResults.Add(frame);
			}

			deminifiedStackFrameResults.Reverse();

			var result = new DeminifyStackTraceResult(message, minifiedStackFrames, deminifiedStackFrameResults);

			return result;
		}
	}
}
