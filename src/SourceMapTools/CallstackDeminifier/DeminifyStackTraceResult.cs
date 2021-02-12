using System;
using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	public class DeminifyStackTraceResult
	{
		internal DeminifyStackTraceResult(
			List<StackFrame> minifiedStackFrames,
			List<StackFrameDeminificationResult> deminifiedStackFrameResults,
			string? message)
		{
			MinifiedStackFrames = minifiedStackFrames;
			DeminifiedStackFrameResults = deminifiedStackFrameResults;
			Message = message;
		}

		public string? Message { get; }

		public IReadOnlyList<StackFrame> MinifiedStackFrames { get; }

		public IReadOnlyList<StackFrameDeminificationResult> DeminifiedStackFrameResults { get; }

		public override string ToString()
		{
			var output = Message ?? string.Empty;
			for (var i = 0; i < DeminifiedStackFrameResults.Count; i++)
			{
				var deminFrame = DeminifiedStackFrameResults[i].DeminifiedStackFrame;

				// Use deminified info wherever possible, merging if necessary so we always print a full frame
				var frame = new StackFrame()
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
