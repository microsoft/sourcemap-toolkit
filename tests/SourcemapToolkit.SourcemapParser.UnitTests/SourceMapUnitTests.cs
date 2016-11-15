using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class SourceMapUnitTests
	{
		[TestMethod]
		public void GetMappingEntryForGeneratedSourcePosition_NullMappingList_ReturnNull()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			SourcePosition sourcePosition = new SourcePosition {ZeroBasedColumnNumber = 3, ZeroBasedLineNumber = 4}; 

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.IsNull(result);
		}

		[TestMethod]
		public void GetMappingEntryForGeneratedSourcePosition_NoMatchingEntryInMappingList_ReturnNull()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
				}
			};
			SourcePosition sourcePosition = new SourcePosition { ZeroBasedColumnNumber = 3, ZeroBasedLineNumber = 4 };

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.IsNull(result);
		}

		[TestMethod]
		public void GetMappingEntryForGeneratedSourcePosition_MatchingEntryInMappingList_ReturnMatchingEntry()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			MappingEntry matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 8, ZeroBasedColumnNumber = 13}
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
				},
				matchingMappingEntry
			};
			SourcePosition sourcePosition = new SourcePosition { ZeroBasedLineNumber = 8, ZeroBasedColumnNumber = 13 };

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.AreEqual(matchingMappingEntry, result);
		}

		[TestMethod]
		public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnSameLine_ReturnSimilarEntry()
		{
			// Arrange
			SourceMap sourceMap = new SourceMap();
			MappingEntry matchingMappingEntry = new MappingEntry
			{
				GeneratedSourcePosition = new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 13 }
			};
			sourceMap.ParsedMappings = new List<MappingEntry>
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
				},
				matchingMappingEntry
			};
			SourcePosition sourcePosition = new SourcePosition { ZeroBasedLineNumber = 10, ZeroBasedColumnNumber = 14 };

			// Act
			MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

			// Asset
			Assert.AreEqual(matchingMappingEntry, result);
		}

        [TestMethod]
        public void GetMappingEntryForGeneratedSourcePosition_NoExactMatchHasSimilarOnDifferentLinesLine_ReturnSimilarEntry()
        {
            // Arrange
            SourceMap sourceMap = new SourceMap();
            MappingEntry matchingMappingEntry = new MappingEntry
            {
                GeneratedSourcePosition = new SourcePosition { ZeroBasedLineNumber = 23, ZeroBasedColumnNumber = 15 }
            };
            sourceMap.ParsedMappings = new List<MappingEntry>
            {
                new MappingEntry
                {
                    GeneratedSourcePosition = new SourcePosition {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0}
                },
                matchingMappingEntry
            };
            SourcePosition sourcePosition = new SourcePosition { ZeroBasedLineNumber = 24, ZeroBasedColumnNumber = 0 };

            // Act
            MappingEntry result = sourceMap.GetMappingEntryForGeneratedSourcePosition(sourcePosition);

            // Asset
            Assert.AreEqual(matchingMappingEntry, result);
        }
    }
}
