using System;

namespace SourcemapToolkit.SourcemapParser
{
	public class MappingEntry
	{
		/// <summary>
		/// The location of the line of code in the transformed code
		/// </summary>
		public SourcePosition GeneratedSourcePosition;

		/// <summary>
		/// The location of the code in the original source code
		/// </summary>
		public SourcePosition OriginalSourcePosition;

		/// <summary>
		/// The original name of the code referenced by this mapping entry
		/// </summary>
		public string OriginalName;

		/// <summary>
		/// The name of the file that originally contained this code
		/// </summary>
		public string OriginalFileName;

		public MappingEntry Clone()
		{
			return new MappingEntry
			{
				GeneratedSourcePosition = this.GeneratedSourcePosition.Clone(),
				OriginalSourcePosition = this.OriginalSourcePosition.Clone(),
				OriginalFileName = this.OriginalFileName,
				OriginalName = this.OriginalName
			};
		}

		public Boolean IsValueEqual(MappingEntry anEntry)
		{
			return (
				this.OriginalName == anEntry.OriginalName &&
				this.OriginalFileName == anEntry.OriginalFileName &&
				this.GeneratedSourcePosition.CompareTo(anEntry.GeneratedSourcePosition) == 0 &&
				this.OriginalSourcePosition.CompareTo(anEntry.OriginalSourcePosition) == 0);
		}
	}
}