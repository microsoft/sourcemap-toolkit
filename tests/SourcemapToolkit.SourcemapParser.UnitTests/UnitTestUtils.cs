using System.IO;
using System.Text;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	public static class UnitTestUtils
	{
		public static StreamReader StreamReaderFromString(string streamContents)
		{
			byte[] byteArray = Encoding.UTF8.GetBytes(streamContents);
			return new StreamReader(new MemoryStream(byteArray));
		}

		public static MappingEntry getSimpleEntry(SourcePosition generatedSourcePosition, SourcePosition originalSourcePosition, string originalFileName)
		{
			return new MappingEntry(
				generatedSourcePosition: generatedSourcePosition,
				originalSourcePosition: originalSourcePosition,
				originalFileName: originalFileName);
		}

		public static SourcePosition generateSourcePosition(int lineNumber, int colNumber = 0)
		{
			return new SourcePosition(
				zeroBasedLineNumber: lineNumber,
				zeroBasedColumnNumber: colNumber);
		}
	}
}