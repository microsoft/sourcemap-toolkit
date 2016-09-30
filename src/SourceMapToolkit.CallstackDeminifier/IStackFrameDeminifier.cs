namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IStackFrameDeminifier
	{
		/// <summary>
		/// This method will deminify a single stack from from a minified stack trace.
		/// </summary>
		/// <returns>Returns a stack trace that has been translated to the original source code. Returns null if it could not be deminified.</returns>
		StackFrame DeminifyStackFrame(StackFrame stackFrame);
	}
}