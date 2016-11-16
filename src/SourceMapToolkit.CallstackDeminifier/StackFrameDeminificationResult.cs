using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Enum indicating if there were any errors encountered when attempting to deminify the StakFrame.
	/// </summary>
	[Flags]
	public enum  DeminificationError
	{
		/// <summary>
		/// No error was encountered durring deminification of the StackFrame.
		/// </summary>
		None,

		/// <summary>
		/// The function that wraps the minified stack frame could not be determined.
		/// </summary>
		NoWrapingFunction,

		/// <summary>
		/// There was an error when there was not a valid source map returned by ISourceMapProvider.GetSourceMapForUrl.
		/// </summary>
		NoSourceMap,

		/// <summary>
		/// There was not a mapping entry found for the source position of the minified stack frame.
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
		/// <summary>
		/// The deminified StackFrame.
		/// </summary>
		public StackFrame DeminifiedStackFrame { get; set; }

		/// <summary>
		/// An enum indicating if any errors occured when deminifying the stack frame.
		/// </summary>
		public DeminificationError DeminificationError { get; set; }

	}
}
