using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class SourceMapGeneratorUnitTests
	{
		[TestMethod]
		public void SerializeMappingEntry_DifferentLineNumber_SemicolonAdded()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState( new List<string>() { "Name" }, new List<string>() { "Source" } );
			state.LastGeneratedPosition.ZeroBasedColumnNumber = 1;

			MappingEntry entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
				OriginalFileName = state.Sources[ 0 ],
				OriginalName = state.Names[ 0 ],
				OriginalSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
			};

			// Act
			List<char> output = new List<char>();
			sourceMapGenerator.SerializeMappingEntry( entry, state, output );

			// Assert
			Assert.IsTrue( output.IndexOf( ';' ) >= 0 );
		}

		[TestMethod]
		public void SerializeMappingEntry_NoOriginalFileName_OneSegment()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState( new List<string>() { "Name" }, new List<string>() { "Source" } );

			MappingEntry entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 10 },
				OriginalSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 1 },
			};

			// Act
			List<char> output = new List<char>();
			sourceMapGenerator.SerializeMappingEntry( entry, state, output );

			// Assert
			Assert.AreEqual( "U", new string( output.ToArray() ) );
		}

		[TestMethod]
		public void SerializeMappingEntry_WithOriginalFileNameNoOriginalName_FourSegment()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState( new List<string>() { "Name" }, new List<string>() { "Source" } );
			state.IsFirstSegment = false;

			MappingEntry entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedColumnNumber = 10 },
				OriginalFileName = state.Sources[0],
				OriginalSourcePosition = new SourcePosition() { ZeroBasedColumnNumber = 5 },
			};

			// Act
			List<char> output = new List<char>();
			sourceMapGenerator.SerializeMappingEntry( entry, state, output );

			// Assert
			Assert.AreEqual( ",UAAK", new string( output.ToArray() ) );
		}

		[TestMethod]
		public void SerializeMappingEntry_WithOriginalFileNameAndOriginalName_FiveSegment()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();

			MappingGenerateState state = new MappingGenerateState( new List<string>() { "Name" }, new List<string>() { "Source" } );
			state.LastGeneratedPosition.ZeroBasedLineNumber = 1;

			MappingEntry entry = new MappingEntry()
			{
				GeneratedSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 5 },
				OriginalSourcePosition = new SourcePosition() { ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 6 },
				OriginalFileName = state.Sources[ 0 ],
				OriginalName = state.Names[ 0 ],
			};

			// Act
			List<char> output = new List<char>();
			sourceMapGenerator.SerializeMappingEntry( entry, state, output );

			// Assert
			Assert.AreEqual( "KACMA", new string( output.ToArray() ) );
		}

		[TestMethod]
		public void SerializeMapping_NullInput_ReturnsNull()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();
			SourceMap input = null;

			// Act
			string output = sourceMapGenerator.SerializeMapping( input );

			// Assert
			Assert.IsNull( output );
		}

		[TestMethod]
		public void SerializeMapping_SimpleSourceMap_CorrectlySerialized()
		{
			// Arrange
			SourceMapGenerator sourceMapGenerator = new SourceMapGenerator();
			SourceMap input = new SourceMap()
			{
				File = "CommonIntl",
				Names = new List<string>() { "CommonStrings", "afrikaans" },
				Sources = new List<string>() { "input/CommonIntl.js" },
				Version = 3,
			};
			input.ParsedMappings = new List<MappingEntry>()
			{
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 0 },
					OriginalFileName = input.Sources[0],
					OriginalName = input.Names[0],
					OriginalSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
				},
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 13 },
					OriginalFileName = input.Sources[0],
					OriginalSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 0 },
				},
				new MappingEntry
				{
					GeneratedSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 0, ZeroBasedColumnNumber = 14 },
					OriginalFileName = input.Sources[0],
					OriginalSourcePosition = new SourcePosition() {ZeroBasedLineNumber = 1, ZeroBasedColumnNumber = 14 },
				},
			};

			// Act
			string output = sourceMapGenerator.SerializeMapping( input );

			// Assert
			Assert.AreEqual( "{\"version\":3,\"file\":\"CommonIntl\",\"mappings\":\"AACAA,aAAA,CAAc;\",\"sources\":[\"input/CommonIntl.js\"],\"names\":[\"CommonStrings\",\"afrikaans\"]}", output );
		}
	}
}
