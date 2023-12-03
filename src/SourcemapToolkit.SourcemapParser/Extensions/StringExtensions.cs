namespace SourcemapToolkit.SourcemapParser.Extensions;

internal static class StringExtensions
{
	/// <summary>
	/// String.Split() allocates an O(input.Length) int array, and
	/// is surprisingly expensive.  For most cases this implementation
	/// is faster and does fewer allocations.
	/// </summary>
	public static string[] SplitFast(this string input, char delimiter)
	{
		var segmentCount = 1;
		for (var i = 0; i < input.Length; i++)
		{
			if (input[i] == delimiter)
			{
				segmentCount++;
			}
		}

		var segmentId = 0;
		var prevDelimiter = 0;
		var segments = new string[segmentCount];

		for (var i = 0; i < input.Length; i++)
		{
			if (input[i] == delimiter)
			{
				segments[segmentId] = input.Substring(prevDelimiter, i - prevDelimiter);
				segmentId++;
				prevDelimiter = i + 1;
			}
		}

		segments[segmentId] = input.Substring(prevDelimiter);

		return segments;
	}
}