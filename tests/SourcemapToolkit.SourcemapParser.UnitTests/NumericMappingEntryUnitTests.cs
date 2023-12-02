using System.Collections.Generic;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class NumericMappingEntryUnitTests
	{
		[Fact]
		public void ToMappingEntry_ContainsGeneratedSourcePosition_CorrectMappingEntryFieldsPopulated()
		{
			// Arrange
			var numericMappingEntry = new NumericMappingEntry
			{
				MappedColumn = 12,
				MappedLine = 13
			};
			var names = new List<string>();
			var sources = new List<string>();

			// Act
			var mappingEntry = numericMappingEntry.ToMappingEntry(names, sources);

			// Assert
			Assert.Equal(12, mappingEntry.SourceMapPosition.Column);
			Assert.Equal(13, mappingEntry.SourceMapPosition.Line);
			Assert.Equal(SourcePosition.NotFound, mappingEntry.SourcePosition);
			Assert.Null(mappingEntry.OriginalFileName);
			Assert.Null(mappingEntry.OriginalName);
		}

		[Fact]
		public void ToMappingEntry_ContainsGeneratedAndOriginalSourcePosition_CorrectMappingEntryFieldsPopulated()
		{
			// Arrange
			var numericMappingEntry = new NumericMappingEntry
			{
				MappedColumn = 2,
				MappedLine = 3,
				SourceColumn = 16,
				SourceLine = 23
			};
			var names = new List<string>();
			var sources = new List<string>();

			// Act
			var mappingEntry = numericMappingEntry.ToMappingEntry(names, sources);

			// Assert
			Assert.Equal(2, mappingEntry.SourceMapPosition.Column);
			Assert.Equal(3, mappingEntry.SourceMapPosition.Line);
			Assert.Equal(16, mappingEntry.SourcePosition.Column);
			Assert.Equal(23, mappingEntry.SourcePosition.Line);
			Assert.Null(mappingEntry.OriginalFileName);
			Assert.Null(mappingEntry.OriginalName);
		}

		[Fact]
		public void ToMappingEntry_ContainsGeneratedPositionNameIndexAndSourcesIndex_CorrectMappingEntryFieldsPopulated()
		{
			// Arrange
			var numericMappingEntry = new NumericMappingEntry
			{
				MappedColumn = 8,
				MappedLine = 48,
				SourceNameIndex = 1,
				SourceFileIndex = 2
			};
			var names = new List<string> {"foo", "bar"};
			var sources = new List<string> { "one", "two", "three"};

			// Act
			var mappingEntry = numericMappingEntry.ToMappingEntry(names, sources);

			// Assert
			Assert.Equal(8, mappingEntry.SourceMapPosition.Column);
			Assert.Equal(48, mappingEntry.SourceMapPosition.Line);
			Assert.Equal(SourcePosition.NotFound, mappingEntry.SourcePosition);
			Assert.Equal("three", mappingEntry.OriginalFileName);
			Assert.Equal("bar", mappingEntry.OriginalName);
		}
	}
}