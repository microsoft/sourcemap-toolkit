namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This class is to be implemented by the consumer of the source map library.
	/// The implementation should be able to return the contents of a file referenced in the stack frame URL.
	/// </summary>
	public interface IGeneratedSourceCodeProvider
	{
		/// <summary>
		/// This method will be invoked for each unique genered code URL that appears in a call stack in order to retreieve 
		/// the contents of that file.
		/// </summary>
		/// <param name="correspondingCallStackFileUrl">The url from a callstack stackframe that caused us to invoke this method</param>
		string GetSourceCodeForStackFrame(string correspondingCallStackFileUrl);
	}
}
