using System.IO;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// This class is to be implemented by the consumer of the source map library.
	/// The implementation should return the correct source map for a given URL that appears in a stack frame.
	/// The callstack deminifier will call this once per file URL and cache the parsed source map.
	/// </summary>
	public interface ISourceMapProvider
	{
		/// <summary>
		/// This method will be invoked for each unique URL that appears in a stack frame from a callstack.
		/// It should return a stream that can be used to obtain the contents of the sourcemap corresponding to the file in the URL.
		/// </summary>
		/// <param name="correspondingCallStackFileUrl">The url from a callstack stackframe that caused us to invoke this method.</param>
		/// <returns>Returns a StreamReader that can be used to obtain the contents of the source map.</returns>
		Stream? GetSourceMapContentsForCallstackUrl(string correspondingCallStackFileUrl);
	}
}