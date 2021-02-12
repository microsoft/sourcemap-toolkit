namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Enum indicating if there were any errors encountered when attempting to deminify the StakFrame.
	/// </summary>
	public enum DeminificationError
	{
		/// <summary>
		/// No error was encountered durring deminification of the StackFrame.
		/// </summary>
		None,

		/// <summary>
		/// There was no source code provided by the ISourceCodeProvider.
		/// </summary>
		NoSourceCodeProvided,

		/// <summary>
		/// The function that wraps the minified stack frame could not be determined.
		/// </summary>
		NoWrapingFunctionFound,

		/// <summary>
		/// There was not a valid source map returned by ISourceMapProvider.GetSourceMapForUrl.
		/// </summary>
		NoSourceMap,

		/// <summary>
		/// A mapping entry was not found for the source position of the minified stack frame.
		/// </summary>
		NoMatchingMapingInSourceMap,

		/// <summary>
		/// There was an error when attempting to parse the source map returned by ISourceMapProvider.GetSourceMapForUrl.
		/// </summary>
		SourceMapFailedToParse
	}

	/// <summary>
	/// Represents the result of attmpting to deminify a single entry in a JavaScript stack frame. 
	/// </summary>
	public class StackFrameDeminificationResult
	{
		internal StackFrameDeminificationResult(
			DeminificationError deminificationError,
			StackFrame deminifiedStackFrame)
		{
			DeminificationError = deminificationError;
			DeminifiedStackFrame = deminifiedStackFrame;
		}

		/// <summary>
		/// The deminified StackFrame.
		/// </summary>
		public StackFrame DeminifiedStackFrame { get; }

		/// <summary>
		/// The original name of the symbol at this frame's position
		/// </summary>
		public string? DeminifiedSymbolName { get; internal set; }

		/// <summary>
		/// An enum indicating if any errors occured when deminifying the stack frame.
		/// </summary>
		public DeminificationError DeminificationError { get; internal set; }
	}
}
