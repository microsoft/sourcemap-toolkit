using System;

namespace SourcemapToolkit.SourcemapParser
{
	/// <summary>
	/// Identifies the location of a piece of code in a JavaScript file
	/// </summary>
	public struct SourcePosition : IComparable<SourcePosition>, IEquatable<SourcePosition>
	{
		public static readonly SourcePosition NotFound = new SourcePosition(-1, -1);

		public int ZeroBasedLineNumber { get; }
		public int ZeroBasedColumnNumber { get; }

		public SourcePosition(int zeroBasedLineNumber, int zeroBasedColumnNumber)
		{
			ZeroBasedLineNumber = zeroBasedLineNumber;
			ZeroBasedColumnNumber = zeroBasedColumnNumber;
		}

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

		public static bool operator ==(SourcePosition x, SourcePosition y)
		{
			return x.Equals(y);
		}

		public static bool operator !=(SourcePosition x, SourcePosition y)
		{
			return !x.Equals(y);
		}

		public bool Equals(SourcePosition that)
		{
			return this.ZeroBasedLineNumber == that.ZeroBasedLineNumber
				&& this.ZeroBasedColumnNumber == that.ZeroBasedColumnNumber;
		}

		public override bool Equals(object obj)
		{
			return (obj is SourcePosition otherSourcePosition) ? Equals(otherSourcePosition) : false;
		}

		public override int GetHashCode()
		{
			return ZeroBasedColumnNumber.GetHashCode() ^ ZeroBasedColumnNumber.GetHashCode();
		}

		/// <summary>
		/// Returns true if we think that the two source positions are close enough together that they may in fact be the referring to the same piece of code.
		/// </summary>
		public bool IsEqualish(SourcePosition other)
		{
			// If the column numbers differ by 1, we can say that the source code is approximately equal
			if (this.ZeroBasedLineNumber == other.ZeroBasedLineNumber && Math.Abs(this.ZeroBasedColumnNumber - other.ZeroBasedColumnNumber) <= 1)
			{
				return true;
			}

			// This handles the case where we are comparing code at the end of one line and the beginning of the next line.
			// If the column number on one of the entries is zero, it is ok for the line numbers to differ by 1, so long as the one that has a column number of zero is the one with the larger line number.
			// Since we don't have the number of columns in each line, we can't know for sure if these two pieces of code are actually near each other. This is an optimistic guess.
			if (Math.Abs(this.ZeroBasedLineNumber - other.ZeroBasedLineNumber) == 1)
			{
				SourcePosition largerLineNumber = this.ZeroBasedLineNumber > other.ZeroBasedLineNumber ? this : other;

				if (largerLineNumber.ZeroBasedColumnNumber == 0)
				{
					return true;
				}
			}

			return false;
		}
	}
}
