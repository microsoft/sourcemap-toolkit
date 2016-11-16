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
