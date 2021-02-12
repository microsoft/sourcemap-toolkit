using System.IO;
using System.Text;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	public static class UnitTestUtils
	{
		public static Stream StreamFromString(string streamContents)
		{
			var byteArray = Encoding.UTF8.GetBytes(streamContents);
			return new MemoryStream(byteArray);
		}

		public static MappingEntry GetSimpleEntry(SourcePosition generatedSourcePosition, SourcePosition originalSourcePosition, string originalFileName)
		{
			return new MappingEntry
			{
				GeneratedSourcePosition = generatedSourcePosition,
				OriginalSourcePosition = originalSourcePosition,
				OriginalFileName = originalFileName
			};
		}

		public static SourcePosition GenerateSourcePosition(int lineNumber, int colNumber = 0)
		{
			return new SourcePosition
			{
				ZeroBasedLineNumber = lineNumber,
				ZeroBasedColumnNumber = colNumber
			};
		}
	}
}