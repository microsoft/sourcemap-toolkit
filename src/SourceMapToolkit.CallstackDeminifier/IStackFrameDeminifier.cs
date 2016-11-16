namespace SourcemapToolkit.CallstackDeminifier
{
	internal interface IStackFrameDeminifier
	{
		/// <summary>
		/// This method will deminify a single stack from from a minified stack trace.
		/// </summary>
		/// <returns>Returns a StackFrameDeminification result that contains a stack trace that has been translated to the original source code. The DeminificationError indicates if the StackFrame could not be deminified. DeminifiedStackFrame will not be null, but any properties off of DeminifiedStackFrame could be if the value could not be extracted. </returns>
		StackFrameDeminificationResult DeminifyStackFrame(StackFrame stackFrame);
	}
}