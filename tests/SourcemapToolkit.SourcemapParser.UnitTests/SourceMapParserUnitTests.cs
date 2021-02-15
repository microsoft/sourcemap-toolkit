using System.IO;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourceMapParserUnitTests
	{
		[Fact]
		public void ParseSourceMap_NullInputStream_ReturnsNull()
		{
			// Arrange
			SourceMapParser sourceMapParser = new SourceMapParser();
			StreamReader input = null;

			// Act
			SourceMap output = sourceMapParser.ParseSourceMap(input);

			// Assert
			Assert.Null(output);
		}

		[Fact]
		public void ParseSourceMap_SimpleSourceMap_CorrectlyParsed()
		{
			// Arrange
			SourceMapParser sourceMapParser = new SourceMapParser();
			string input = "{ \"version\":3, \"file\":\"CommonIntl\", \"lineCount\":65, \"mappings\":\"AACAA,aAAA,CAAc\", \"sources\":[\"input/CommonIntl.js\"], \"names\":[\"CommonStrings\",\"afrikaans\"]}";

			// Act
			SourceMap output = sourceMapParser.ParseSourceMap(UnitTestUtils.StreamReaderFromString(input));

			// Assert
			Assert.Equal(3, output.Version);
			Assert.Equal("CommonIntl", output.File);
			Assert.Equal("AACAA,aAAA,CAAc", output.Mappings);
			Assert.Single(output.Sources);
			Assert.Equal("input/CommonIntl.js", output.Sources[0]);
			Assert.Equal(2, output.Names.Count);
			Assert.Equal("CommonStrings", output.Names[0]);
			Assert.Equal("afrikaans", output.Names[1]);
		}
	}
}
