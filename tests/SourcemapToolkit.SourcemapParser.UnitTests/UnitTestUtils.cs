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
	}
}