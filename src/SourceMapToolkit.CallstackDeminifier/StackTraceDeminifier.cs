using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier;

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
	/// <param name="stackTraceString">stack trace as string, to deobfuscate</param>
	/// <param name="preferSourceMapsSymbols">if true, we will use exact sourcemap names for deobfuscation, without guessing the wrapper function name from source code</param>
	/// <param name="fixOffByOneWithPreferSourceMapSymbols">preferSourceMapsSymbols uses name at call site in deobfuscated frame. Passing this as true fixes that case, to use the caller function name from next frame</param>
	/// <returns></returns>
	public DeminifyStackTraceResult DeminifyStackTrace(string stackTraceString, bool preferSourceMapsSymbols = false, bool fixOffByOneWithPreferSourceMapSymbols = false)
	{
		var minifiedStackFrames = _stackTraceParser.ParseStackTrace(stackTraceString, out var message);
		var deminifiedStackFrameResults = new List<StackFrameDeminificationResult>(minifiedStackFrames.Count);

		// Deminify frames in reverse order so we can pass the symbol name from caller
		// (i.e. the function name) into the next level's deminification.
		string callerSymbolName = null;
		for (var i = minifiedStackFrames.Count - 1; i >= 0; i--)
		{
			var frame = _stackFrameDeminifier.DeminifyStackFrame(minifiedStackFrames[i], callerSymbolName, preferSourceMapsSymbols);
			callerSymbolName = frame?.DeminifiedSymbolName;
			deminifiedStackFrameResults.Add(frame);
		}

		deminifiedStackFrameResults.Reverse();
		if (preferSourceMapsSymbols && fixOffByOneWithPreferSourceMapSymbols)
		{
			// we want to move all method names by one frame, so each frame will contain caller name and not callee name. To make callstacks more familiar to C# and js debug versions.
			// However, for first frame we want to keep calee name (if avaliable) as well since this is interesting info we don't want to lose.
			// However it means that for last frame (N), if have more then 1 frame in callstack, N-1 frame will have the same name.
			// It is confusing, so lets replace last one with null. This will cause toString to use the obfuscated name
			for (var i = 0; i < deminifiedStackFrameResults.Count - 1; i++)
			{
				var updatedMethodName = deminifiedStackFrameResults[i + 1].DeminifiedStackFrame.MethodName;
				if (i == 0 && deminifiedStackFrameResults[i].DeminifiedStackFrame.MethodName != null)
				{
					updatedMethodName = updatedMethodName + "=>" + deminifiedStackFrameResults[i].DeminifiedStackFrame.MethodName;
				}
				deminifiedStackFrameResults[i].DeminifiedStackFrame.MethodName = updatedMethodName;
			}
			if (deminifiedStackFrameResults.Count > 1)
			{
				deminifiedStackFrameResults[deminifiedStackFrameResults.Count - 1].DeminifiedStackFrame.MethodName = null;
			}
		}
		var result = new DeminifyStackTraceResult(message, minifiedStackFrames, deminifiedStackFrameResults);

		return result;
	}
}