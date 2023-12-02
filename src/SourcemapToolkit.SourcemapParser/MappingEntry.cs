namespace SourcemapToolkit.SourcemapParser;

public struct MappingEntry
{
	/// <summary>
	/// The location of the line of code in the transformed code
	/// </summary>
	public readonly SourcePosition SourceMapPosition;

	/// <summary>
	/// The location of the code in the original source code
	/// </summary>
	public readonly SourcePosition SourcePosition;

	/// <summary>
	/// The original name of the code referenced by this mapping entry
	/// </summary>
	public readonly string OriginalName;

	/// <summary>
	/// The name of the file that originally contained this code
	/// </summary>
	public readonly string OriginalFileName;

	public MappingEntry(
		SourcePosition sourceMapPosition,
		SourcePosition? originalSourcePosition = null,
		string originalName = null,
		string originalFileName = null)
	{
		SourceMapPosition = sourceMapPosition;
		SourcePosition = originalSourcePosition ?? SourcePosition.NotFound;
		OriginalName = originalName;
		OriginalFileName = originalFileName;
	}

	public MappingEntry CloneWithResetColumnNumber()
	{
		return new(
			sourceMapPosition: new(
				line: SourceMapPosition.Line,
				column: 0),
			originalSourcePosition: new SourcePosition(
				line: SourcePosition.Line,
				column: 0),
			originalName: OriginalName,
			originalFileName: OriginalFileName);
	}

	public bool Equals(MappingEntry that)
	{
		return this.OriginalName == that.OriginalName &&
		       this.OriginalFileName == that.OriginalFileName &&
		       this.SourceMapPosition.Equals(that.SourceMapPosition) &&
		       this.SourcePosition.Equals(that.SourcePosition);
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
			var hash = 23;
			hash = hash * 31 + SourceMapPosition.GetHashCode();
			hash = hash * 31 + SourcePosition.GetHashCode();
			hash = hash * 31 + (OriginalName ?? "").GetHashCode();
			hash = hash * 31 + (OriginalFileName ?? "").GetHashCode();
			return hash;
		}
	}
}