using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser;

internal static class IReadOnlyListExtensions
{
	public static int IndexOf<T>(this IReadOnlyList<T> input, T value)
	{
		var equalityComparer = EqualityComparer<T>.Default;
		for (var i = 0; i < input.Count; i++)
		{
			if (equalityComparer.Equals(input[i], value))
			{
				return i;
			}
		}

		return -1;
	}

	/// <summary>
	/// Copied from: https://referencesource.microsoft.com/#mscorlib/system/collections/generic/arraysorthelper.cs,63a9955a91f2b37b
	/// </summary>
	public static int BinarySearch<T>(this IReadOnlyList<T> input, T item, IComparer<T> comparer)
	{
		var lo = 0;
		var hi = input.Count - 1;

		while (lo <= hi)
		{
			var i = lo + ((hi - lo) >> 1);
			var order = comparer.Compare(input[i], item);

			if (order == 0)
			{
				return i;
			}

			if (order < 0)
			{
				lo = i + 1;
			}
			else
			{
				hi = i - 1;
			}
		}

		return ~lo;
	}
}