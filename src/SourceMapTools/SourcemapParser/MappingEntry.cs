namespace SourcemapToolkit.SourcemapParser
{
	public class MappingEntry
	{
		/// <summary>
		/// The location of the line of code in the transformed code
		/// </summary>
		public SourcePosition GeneratedSourcePosition = null!;

		/// <summary>
		/// The location of the code in the original source code
		/// </summary>
		public SourcePosition OriginalSourcePosition = null!;

		/// <summary>
		/// The original name of the code referenced by this mapping entry
		/// </summary>
		public string? OriginalName;

		/// <summary>
		/// The name of the file that originally contained this code
		/// </summary>
		public string? OriginalFileName;

		public MappingEntry Clone()
		{
			return new MappingEntry
			{
				GeneratedSourcePosition = GeneratedSourcePosition.Clone(),
				OriginalSourcePosition = OriginalSourcePosition.Clone(),
				OriginalFileName = OriginalFileName,
				OriginalName = OriginalName
			};
		}

		public bool IsValueEqual(MappingEntry anEntry)
		{
			return
				OriginalName == anEntry.OriginalName &&
				OriginalFileName == anEntry.OriginalFileName &&
				GeneratedSourcePosition.CompareTo(anEntry.GeneratedSourcePosition) == 0 &&
				OriginalSourcePosition.CompareTo(anEntry.OriginalSourcePosition) == 0;
		}
	}
}