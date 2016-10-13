using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Class used to parse a JavaScript stack trace 
	/// string into a list of StackFrame objects
	/// </summary>
	public class StackTraceParser : IStackTraceParser
	{
		private readonly Regex _lineNumberRegex = new Regex(@"([^@(\s]*\.js)[^/]*:([0-9]+):([0-9]+)[^/]*$", RegexOptions.Compiled);

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
		public List<StackFrame> ParseStackTrace(string stackTraceString)
		{
			if (stackTraceString == null)
			{
				throw new ArgumentNullException(nameof(stackTraceString));
			}

			List<StackFrame> stackTrace = new List<StackFrame>();
			List<string> stackFrameStrings = stackTraceString.Split('\n').ToList();

			foreach (string frame in stackFrameStrings)
			{
				StackFrame parsedStackFrame = TryParseSingleStackFrame(frame);

				if (parsedStackFrame != null)
				{
					stackTrace.Add(parsedStackFrame);
				}
			}

			return stackTrace;
		}

		/// <summary>
		/// Given a single stack frame, extract the method name.
		/// </summary>
		private string TryExtractMethodNameFromFrame(string frame)
		{
			// Firefox has stackframes in the form: "c@http://localhost:19220/crashcauser.min.js:1:34"
			int atSymbolIndex = frame.IndexOf("@http", StringComparison.Ordinal);
			if (atSymbolIndex != -1)
			{
				return frame.Substring(0, atSymbolIndex).TrimStart();
			}

			// Chrome and IE11 have stackframes in the form: " at d (http://chrisgocallstack.azurewebsites.net/crashcauser.min.js:1:75)"
			int atStringIndex = frame.IndexOf("at ", StringComparison.Ordinal);
			if (atStringIndex != -1)
			{
				int httpIndex = frame.IndexOf(" (http", atStringIndex, StringComparison.Ordinal);
				if (httpIndex != -1)
				{
					return frame.Substring(atStringIndex, httpIndex - atStringIndex).Replace("at ", "").Trim();
				}
			}

			return null;
		}

		/// <summary>
		/// Parses a string representing a single stack frame into a StackFrame object. 
		/// </summary>
		internal virtual StackFrame TryParseSingleStackFrame(string frame)
		{
			if (frame == null)
			{
				throw new ArgumentNullException(nameof(frame));
			}

			Match lineNumberMatch = _lineNumberRegex.Match(frame);

			if (!lineNumberMatch.Success)
			{
				return null;
			}

			StackFrame result = new StackFrame {MethodName = TryExtractMethodNameFromFrame(frame)};

			if (lineNumberMatch.Success)
			{
				result.FilePath = lineNumberMatch.Groups[1].Value;
				result.SourcePosition = new SourcePosition
				{
					// The browser provides one-based line and column numbers, but the
					// rest of this library uses zero-based values. Normalize to make
					// the stack frames zero based.
					ZeroBasedLineNumber = int.Parse(lineNumberMatch.Groups[2].Value, CultureInfo.InvariantCulture) - 1,
					ZeroBasedColumnNumber = int.Parse(lineNumberMatch.Groups[3].Value, CultureInfo.InvariantCulture) -1 
				};
			}

			return result;
		}
	}
}