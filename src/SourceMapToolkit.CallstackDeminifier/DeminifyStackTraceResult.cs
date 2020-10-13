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

				// Use deminified info wherever possible, merging if necessary so we always print a full frame
				StackFrame frame = new StackFrame()
				{
					MethodName = deminFrame.MethodName ?? MinifiedStackFrames[i].MethodName,
					SourcePosition = deminFrame.SourcePosition ?? MinifiedStackFrames[i].SourcePosition,
					FilePath = deminFrame.SourcePosition != null ? deminFrame.FilePath : MinifiedStackFrames[i].FilePath
				};

				output += $"{Environment.NewLine}  {frame}";
			}

			return output;
		}
	}
}
