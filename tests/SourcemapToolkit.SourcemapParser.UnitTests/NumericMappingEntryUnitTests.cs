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
			NumericMappingEntry numericMappingEntry = new NumericMappingEntry
			{
				GeneratedColumnNumber = 12,
				GeneratedLineNumber = 13
			};
			List<string> names = new List<string>();
			List<string> sources = new List<string>();

			// Act
			MappingEntry mappingEntry = numericMappingEntry.ToMappingEntry(names, sources);

			// Assert
			Assert.Equal(12, mappingEntry.GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(13, mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(SourcePosition.NotFound, mappingEntry.OriginalSourcePosition);
			Assert.Null(mappingEntry.OriginalFileName);
			Assert.Null(mappingEntry.OriginalName);
		}

		[Fact]
		public void ToMappingEntry_ContainsGeneratedAndOriginalSourcePosition_CorrectMappingEntryFieldsPopulated()
		{
			// Arrange
			NumericMappingEntry numericMappingEntry = new NumericMappingEntry
			{
				GeneratedColumnNumber = 2,
				GeneratedLineNumber = 3,
				OriginalColumnNumber = 16,
				OriginalLineNumber = 23
			};
			List<string> names = new List<string>();
			List<string> sources = new List<string>();

			// Act
			MappingEntry mappingEntry = numericMappingEntry.ToMappingEntry(names, sources);

			// Assert
			Assert.Equal(2, mappingEntry.GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(3, mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(16, mappingEntry.OriginalSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(23, mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber);
			Assert.Null(mappingEntry.OriginalFileName);
			Assert.Null(mappingEntry.OriginalName);
		}

		[Fact]
		public void ToMappingEntry_ContainsGeneratedPositionNameIndexAndSourcesIndex_CorrectMappingEntryFieldsPopulated()
		{
			// Arrange
			NumericMappingEntry numericMappingEntry = new NumericMappingEntry
			{
				GeneratedColumnNumber = 8,
				GeneratedLineNumber = 48,
				OriginalNameIndex = 1,
				OriginalSourceFileIndex = 2
			};
			List<string> names = new List<string> {"foo", "bar"};
			List<string> sources = new List<string> { "one", "two", "three"};

			// Act
			MappingEntry mappingEntry = numericMappingEntry.ToMappingEntry(names, sources);

			// Assert
			Assert.Equal(8, mappingEntry.GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.Equal(48, mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.Equal(SourcePosition.NotFound, mappingEntry.OriginalSourcePosition);
			Assert.Equal("three", mappingEntry.OriginalFileName);
			Assert.Equal("bar", mappingEntry.OriginalName);
		}
	}
}