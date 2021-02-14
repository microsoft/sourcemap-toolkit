using System;

namespace SourcemapToolkit.SourcemapParser
{
	public struct MappingEntry
	{
		/// <summary>
		/// The location of the line of code in the transformed code
		/// </summary>
		public SourcePosition GeneratedSourcePosition { get; }

		/// <summary>
		/// The location of the code in the original source code
		/// </summary>
		public SourcePosition OriginalSourcePosition { get; }

		/// <summary>
		/// The original name of the code referenced by this mapping entry
		/// </summary>
		public string OriginalName { get; }

		/// <summary>
		/// The name of the file that originally contained this code
		/// </summary>
		public string OriginalFileName { get; }

		public MappingEntry(
			SourcePosition generatedSourcePosition,
			SourcePosition? originalSourcePosition = null,
			string originalName = null,
			string originalFileName = null)
		{
			GeneratedSourcePosition = generatedSourcePosition;
			OriginalSourcePosition = originalSourcePosition ?? SourcePosition.NotFound;
			OriginalName = originalName;
			OriginalFileName = originalFileName;
		}

		public MappingEntry CloneWithResetColumnNumber()
		{
			return new MappingEntry(
				generatedSourcePosition: new SourcePosition(
					zeroBasedLineNumber: GeneratedSourcePosition.ZeroBasedLineNumber,
					zeroBasedColumnNumber: 0),
				originalSourcePosition: new SourcePosition(
					zeroBasedLineNumber: OriginalSourcePosition.ZeroBasedLineNumber,
					zeroBasedColumnNumber: 0),
				originalName: OriginalName,
				originalFileName: OriginalFileName);
		}

		public bool Equals(MappingEntry that)
		{
			return this.OriginalName == that.OriginalName &&
				this.OriginalFileName == that.OriginalFileName &&
				this.GeneratedSourcePosition.Equals(that.GeneratedSourcePosition) &&
				this.OriginalSourcePosition.Equals(that.OriginalSourcePosition);
		}

		public override bool Equals(object obj)
		{
			return (obj is MappingEntry mappingEntry) ? Equals(mappingEntry) : false;
		}

		/// <summary>
		/// An implementation of Josh Bloch's hashing algorithm from Effective Java.
		/// It is fast, offers a good distribution (with primes 23 and 31), and allocation free.
		/// </summary>
		public override int GetHashCode()
		{
			unchecked // Wrap to protect overflow
			{
				int hash = 23;
				hash = hash * 31 + GeneratedSourcePosition.GetHashCode();
				hash = hash * 31 + OriginalSourcePosition.GetHashCode();
				hash = hash * 31 + (OriginalName ?? "").GetHashCode();
				hash = hash * 31 + (OriginalFileName ?? "").GetHashCode();
				return hash;
			}
		}
	}
}