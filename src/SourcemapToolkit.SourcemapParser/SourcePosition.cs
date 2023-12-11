using System;

namespace SourcemapToolkit.SourcemapParser;

/// <summary>
/// Identifies the location of a piece of code in a JavaScript file
/// </summary>
public struct SourcePosition : IComparable<SourcePosition>, IEquatable<SourcePosition>
{
	public static readonly SourcePosition NotFound = new(-1, -1);

	public readonly int Line;
	public readonly int Column;

	public SourcePosition(int line, int column)
	{
		Line = line;
		Column = column;
	}

	public int CompareTo(SourcePosition other)
	{
		if (Line == other.Line)
		{
			return Column.CompareTo(other.Column);
		}

		return Line.CompareTo(other.Line);
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
		return Line == that.Line
		       && Column == that.Column;
	}

	public override bool Equals(object obj)
	{
		return (obj is SourcePosition otherSourcePosition) ? Equals(otherSourcePosition) : false;
	}

	public override int GetHashCode()
	{
		return Column.GetHashCode() ^ Column.GetHashCode();
	}

	/// <summary>
	/// Returns true if we think that the two source positions are close enough together that they may in
	/// fact be the referring to the same piece of code.
	/// </summary>
	public bool IsEqualish(SourcePosition other)
	{
		// If the column numbers differ by 1, we can say that the source code is approximately equal
		if (Line == other.Line && Math.Abs(Column - other.Column) <= 1) return true;

		// This handles the case where we are comparing code at the end of one line and the beginning of the next line.
		// If the column number on one of the entries is zero, it is ok for the line numbers to differ by 1, so
		// long as the one that has a column number of zero is the one with the larger line number.
		// Since we don't have the number of columns in each line, we can't know for sure if these two pieces of
		// code are actually near each other. This is an optimistic guess.
		if (Math.Abs(Line - other.Line) == 1)
		{
			var largerLineNumber = Line > other.Line ? this : other;
			return largerLineNumber.Column == 0;
		}

		return false;
	}
}