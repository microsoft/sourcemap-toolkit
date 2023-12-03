using System.Collections.Generic;
using SourcemapToolkit.CallstackDeminifier.StackFrameDeminifiers;

namespace SourcemapToolkit.CallstackDeminifier.StackParsers;

public interface IStackTraceParser
{
	/// <summary>
	/// Generates a list of StackFrame objects based on the input stack trace.
	/// This method normalizes differences between different browsers.
	/// The source positions in the parsed stack frames will be normalized so they
	/// are zero-based instead of one-based to align with the rest of the library.
	/// </summary>
	/// <returns>
	/// Returns a list of StackFrame objects corresponding to the stackTraceString.
	/// Any parts of the stack trace that could not be parsed are excluded from
	/// the result. Does not ever return null.
	/// </returns>
	/// <remarks>
	/// This override drops the Message out param for backward compatibility
	/// </remarks>
	IReadOnlyList<StackFrame> ParseStackTrace(string stackTraceString);

	/// <summary>
	/// Generates a list of StackFrame objects based on the input stack trace.
	/// This method normalizes differences between different browsers.
	/// The source positions in the parsed stack frames will be normalized so they
	/// are zero-based instead of one-based to align with the rest of the library.
	/// </summary>
	/// <returns>
	/// Returns a list of StackFrame objects corresponding to the stackTraceString.
	/// Any parts of the stack trace that could not be parsed are excluded from
	/// the result. Does not ever return null.
	/// </returns>
	IReadOnlyList<StackFrame> ParseStackTrace(string stackTraceString, out string message);
}