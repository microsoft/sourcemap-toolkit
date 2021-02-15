using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Represents a single entry in a JavaScript stack frame. 
	/// </summary>
	public class StackFrame
	{
		/// <summary>
		/// The name of the method
		/// </summary>
		public string MethodName { get; set; }

		/// <summary>
		/// The path of the file where this code is defined
		/// </summary>
		public string FilePath { get; set; }

		/// <summary>
		/// The zero-based position of this stack entry.
		/// </summary>
		public SourcePosition SourcePosition { get; set; } = SourcePosition.NotFound;

		public override string ToString()
		{
			string output = $"at {(string.IsNullOrWhiteSpace(MethodName) ? "?" : MethodName)}";
			if (!string.IsNullOrWhiteSpace(FilePath))
			{
				output += $" in {FilePath}";
				if (SourcePosition != SourcePosition.NotFound)
				{
					output += $":{SourcePosition.ZeroBasedLineNumber + 1}:{SourcePosition.ZeroBasedColumnNumber + 1}";
				}
			}
			return output;
		}
	}
}
