using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	public class DeminifyStackTraceResult
	{
		public List<StackFrame> MinifiedStackFrames;

		public List<StackFrame> DeminifiedStackFrames;
	}
}
