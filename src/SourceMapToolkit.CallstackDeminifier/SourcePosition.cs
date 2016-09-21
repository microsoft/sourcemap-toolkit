using System;

namespace SourcemapToolkit.CallstackDeminifier
{
	/// <summary>
	/// Identifies the location of a piece of code in a JavaScript file
	/// </summary>
	public class SourcePosition : IComparable<SourcePosition>
	{
		public int ZeroBasedLineNumber;

		public int ZeroBasedColumnNumber;

		public int CompareTo(SourcePosition other)
		{
			if (this.ZeroBasedLineNumber == other.ZeroBasedLineNumber)
			{
				return this.ZeroBasedColumnNumber.CompareTo(other.ZeroBasedColumnNumber);
			}

			return this.ZeroBasedLineNumber.CompareTo(other.ZeroBasedLineNumber);
		}

		public static bool operator <(SourcePosition x, SourcePosition y)
		{
			return x.CompareTo(y) < 0;
		}

		public static bool operator >(SourcePosition x, SourcePosition y)
		{
			return x.CompareTo(y) > 0;
		}
	}
}
