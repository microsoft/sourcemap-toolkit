using System;
using System.Collections.Generic;

using Xunit;

using Rhino.Mocks;

using SourcemapToolkit.SourcemapParser;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	public class SourceMapExtensionsUnitTests
	{
		[Fact]
		public void GetDeminifiedMethodName_NullBindings_ReturnsNull()
		{
			// Arrange
			IReadOnlyList<BindingInformation> bindings = null;
			SourceMap sourceMap = CreateSourceMapMock();

			// Act
			string deminifiedMethodName = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Null(deminifiedMethodName);
		}

		[Fact]
		public void GetDeminifiedMethodName_NullSourceMap_ThrowsException()
		{
			// Arrange
			IReadOnlyList<BindingInformation> bindings = null;
			SourceMap sourceMap = null;

			// Act
			Assert.Throws<ArgumentNullException>(() => SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings));
		}

		[Fact]
		public void GetDeminifiedMethodName_EmptyBinding_ReturnNullMethodName()
		{
			// Arrange
			IReadOnlyList<BindingInformation> bindings = new List<BindingInformation>();
			SourceMap sourceMap = CreateSourceMapMock();

			// Act
			string result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Null(result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>()
				{
					new BindingInformation(
						name: default,
						sourcePosition: new SourcePosition(zeroBasedLineNumber: 20, zeroBasedColumnNumber: 15))
				};

			SourceMap sourceMap = CreateSourceMapMock();
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(Arg<SourcePosition>.Is.Anything)).Return(null);

			// Act
			string result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Null(result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_HasSingleBindingMatchingMapping_ReturnsMethodName()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>()
				{
					new BindingInformation(
						name: default,
						sourcePosition: new SourcePosition(zeroBasedLineNumber: 5, zeroBasedColumnNumber: 8))
				};

			SourceMap sourceMap = CreateSourceMapMock();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 8)))
				.Return(new MappingEntry(generatedSourcePosition: default, originalName: "foo"));

			// Act
			string result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Equal("foo", result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_MatchingMappingMultipleBindingsMissingPrototypeMapping_ReturnsMethodName()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>
				{
					new BindingInformation(
						name: default,
						sourcePosition: new SourcePosition(zeroBasedLineNumber: 86, zeroBasedColumnNumber: 52)),
					new BindingInformation(
						name: default,
						sourcePosition: new SourcePosition(zeroBasedLineNumber: 88, zeroBasedColumnNumber: 78))
				};

			SourceMap sourceMap = CreateSourceMapMock();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 86 && y.ZeroBasedColumnNumber == 52)))
				.Return(null);

			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 88 && y.ZeroBasedColumnNumber == 78)))
				.Return(new MappingEntry(generatedSourcePosition: default, originalName: "baz"));

			// Act
			string result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Equal("baz", result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_MatchingMappingMultipleBindings_ReturnsMethodNameWithFullBinding()
		{
			// Arrange
			List<BindingInformation> bindings = new List<BindingInformation>
				{
					new BindingInformation(
						name: default,
						sourcePosition: new SourcePosition(zeroBasedLineNumber: 5, zeroBasedColumnNumber: 5)),
					new BindingInformation(
						name: default,
						sourcePosition: new SourcePosition(zeroBasedLineNumber: 20, zeroBasedColumnNumber: 10))
				};

			SourceMap sourceMap = CreateSourceMapMock();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 5 && y.ZeroBasedColumnNumber == 5)))
				.Return(new MappingEntry(generatedSourcePosition: default, originalName: "bar"));

			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.ZeroBasedLineNumber == 20 && y.ZeroBasedColumnNumber == 10)))
				.Return(new MappingEntry(generatedSourcePosition: default, originalName: "baz"));

			// Act
			string result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Equal("bar.baz", result);
			sourceMap.VerifyAllExpectations();
		}

		private static SourceMap CreateSourceMapMock()
		{
			return MockRepository.GenerateStub<SourceMap>(
				0 /* version */,
				default(string) /* file */,
				default(string) /* mappings */,
				default(IReadOnlyList<string>) /* sources */,
				default(IReadOnlyList<string>) /* names */,
				default(IReadOnlyList<MappingEntry>) /* parsedMappings */,
				default(IReadOnlyList<string>) /* sourcesContent */);
		}
	}
}
