namespace SourcemapToolkit.SourcemapParser
{
	internal static class StringExtensions
	{
		/// <summary>
		/// String.Split() allocates an O(input.Length) int array, and
		/// is surprisingly expensive.  For most cases this implementation
		/// is faster and does fewer allocations.
		/// </summary>
		public static string[] SplitFast(this string input, char delimiter)
		{
			int segmentCount = 1;
			for (int i = 0; i < input.Length; i++)
			{
				if (input[i] == delimiter)
				{
					segmentCount++;
				}
			}

			int segmentId = 0;
			int prevDelimiter = 0;
			string[] segments = new string[segmentCount];

			for (int i = 0; i < input.Length; i++)
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
}
