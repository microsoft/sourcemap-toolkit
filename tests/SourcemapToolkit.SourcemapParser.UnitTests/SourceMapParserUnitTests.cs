using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class SourceMapParserUnitTests
	{
		[TestMethod]
		public void ParseSourceMap_SimpleSourceMap_CorrectlyParsed()
		{
			// Arrange
			SourceMapParser sourceMapParser = new SourceMapParser();
			string input = "{ \"version\":3, \"file\":\"CommonIntl\", \"lineCount\":65, \"mappings\":\"AACAA,aAAA,CAAc\", \"sources\":[\"input/CommonIntl.js\"], \"names\":[\"CommonStrings\",\"afrikaans\"]}";

			// Act
			SourceMap output = sourceMapParser.ParseSourceMap(input);

			// Assert
			Assert.AreEqual(3, output.Version);
			Assert.AreEqual("CommonIntl", output.File);
			Assert.AreEqual("AACAA,aAAA,CAAc", output.Mappings);
			Assert.AreEqual(1, output.Sources.Count);
			Assert.AreEqual("input/CommonIntl.js", output.Sources[0]);
			Assert.AreEqual(2, output.Names.Count);
			Assert.AreEqual("CommonStrings", output.Names[0]);
			Assert.AreEqual("afrikaans", output.Names[1]);
		}
	}
}
