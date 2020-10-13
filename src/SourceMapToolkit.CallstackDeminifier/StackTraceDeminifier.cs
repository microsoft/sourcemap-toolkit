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
		public DeminifyStackTraceResult DeminifyStackTrace(string stackTraceString)
		{
			DeminifyStackTraceResult result = new DeminifyStackTraceResult();
			result.MinifiedStackFrames = _stackTraceParser.ParseStackTrace(stackTraceString, out string message);
			result.Message = message;
			result.DeminifiedStackFrameResults = new List<StackFrameDeminificationResult>();

			// Deminify frames in reverse order so we can pass the symbol name from caller
			// (i.e. the function name) into the next level's deminification.
			string callerSymbolName = null;
			for (int i = result.MinifiedStackFrames.Count - 1; i >= 0; i--)
			{
				var frame = _stackFrameDeminifier.DeminifyStackFrame(result.MinifiedStackFrames[i], callerSymbolName);
				callerSymbolName = frame?.DeminifiedSymbolName;
				result.DeminifiedStackFrameResults.Insert(0, frame);
			}

			return result;
		}
	}
}
