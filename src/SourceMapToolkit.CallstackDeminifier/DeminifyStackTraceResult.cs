using System.Collections.Generic;
using System.Text;

namespace SourcemapToolkit.CallstackDeminifier
{
	public class DeminifyStackTraceResult
	{
		public string Message { get; }
		public IReadOnlyList<StackFrame> MinifiedStackFrames { get; }
		public IReadOnlyList<StackFrameDeminificationResult> DeminifiedStackFrameResults { get; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Message))
			{
				sb.Append(Message);
			}

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

				sb.AppendLine();
				sb.Append("  ");
				sb.Append(frame);
			}

			return sb.ToString();
		}

		public DeminifyStackTraceResult(
			string message,
			IReadOnlyList<StackFrame> minifiedStackFrames,
			IReadOnlyList<StackFrameDeminificationResult> deminifiedStackFrameResults)
		{
			Message = message;
			MinifiedStackFrames = minifiedStackFrames;
			DeminifiedStackFrameResults = deminifiedStackFrameResults;
		}
	}
}
