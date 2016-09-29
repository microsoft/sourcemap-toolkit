using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class NumericMappingEntryUnitTests
	{
		[TestMethod]
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
			Assert.AreEqual(12, mappingEntry.GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(13, mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.IsNull(mappingEntry.OriginalSourcePosition);
			Assert.IsNull(mappingEntry.OriginalFileName);
			Assert.IsNull(mappingEntry.OriginalName);
		}

		[TestMethod]
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
			Assert.AreEqual(2, mappingEntry.GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(3, mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.AreEqual(16, mappingEntry.OriginalSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(23, mappingEntry.OriginalSourcePosition.ZeroBasedLineNumber);
			Assert.IsNull(mappingEntry.OriginalFileName);
			Assert.IsNull(mappingEntry.OriginalName);
		}

		[TestMethod]
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
			Assert.AreEqual(8, mappingEntry.GeneratedSourcePosition.ZeroBasedColumnNumber);
			Assert.AreEqual(48, mappingEntry.GeneratedSourcePosition.ZeroBasedLineNumber);
			Assert.IsNull(mappingEntry.OriginalSourcePosition);
			Assert.AreEqual("three", mappingEntry.OriginalFileName);
			Assert.AreEqual("bar", mappingEntry.OriginalName);
		}
	}
}