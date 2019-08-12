using System;
using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	public class DeminifyStackTraceResult
	{
		public string Message;

		public List<StackFrame> MinifiedStackFrames;

		public List<StackFrameDeminificationResult> DeminifiedStackFrameResults;

		public override string ToString()
		{
			string output = Message ?? string.Empty;
			for (int i = 0; i < DeminifiedStackFrameResults.Count; i++)
			{
				StackFrame deminFrame = DeminifiedStackFrameResults[i].DeminifiedStackFrame;
				StackFrame frame = string.IsNullOrEmpty(deminFrame.MethodName) ? MinifiedStackFrames[i] : deminFrame;

				output += $"{Environment.NewLine}  {frame}";
			}

			return output;
		}
	}
}
