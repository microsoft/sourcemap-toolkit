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
			var sourceMap = CreateSourceMapMock();

			// Act
			var deminifiedMethodName = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

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
			var sourceMap = CreateSourceMapMock();

			// Act
			var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Null(result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_HasSingleBindingNoMatchingMapping_ReturnNullMethodName()
		{
			// Arrange
			var bindings = new List<BindingInformation>()
				{
					new BindingInformation(
						name: default(string),
						sourcePosition: new SourcePosition(line: 20, column: 15))
				};

			var sourceMap = CreateSourceMapMock();
			sourceMap.Stub(x => x.GetMappingEntryForGeneratedSourcePosition(Arg<SourcePosition>.Is.Anything)).Return(null);

			// Act
			var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Null(result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_HasSingleBindingMatchingMapping_ReturnsMethodName()
		{
			// Arrange
			var bindings = new List<BindingInformation>()
				{
					new BindingInformation(
						name: default(string),
						sourcePosition: new SourcePosition(line: 5, column: 8))
				};

			var sourceMap = CreateSourceMapMock();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.Line == 5 && y.Column == 8)))
				.Return(new MappingEntry(sourceMapPosition: default(SourcePosition), originalName: "foo"));

			// Act
			var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Equal("foo", result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_MatchingMappingMultipleBindingsMissingPrototypeMapping_ReturnsMethodName()
		{
			// Arrange
			var bindings = new List<BindingInformation>
				{
					new BindingInformation(
						name: default(string),
						sourcePosition: new SourcePosition(line: 86, column: 52)),
					new BindingInformation(
						name: default(string),
						sourcePosition: new SourcePosition(line: 88, column: 78))
				};

			var sourceMap = CreateSourceMapMock();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.Line == 86 && y.Column == 52)))
				.Return(null);

			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.Line == 88 && y.Column == 78)))
				.Return(new MappingEntry(sourceMapPosition: default(SourcePosition), originalName: "baz"));

			// Act
			var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

			// Assert
			Assert.Equal("baz", result);
			sourceMap.VerifyAllExpectations();
		}

		[Fact]
		public void GetDeminifiedMethodName_MatchingMappingMultipleBindings_ReturnsMethodNameWithFullBinding()
		{
			// Arrange
			var bindings = new List<BindingInformation>
				{
					new BindingInformation(
						name: default(string),
						sourcePosition: new SourcePosition(line: 5, column: 5)),
					new BindingInformation(
						name: default(string),
						sourcePosition: new SourcePosition(line: 20, column: 10))
				};

			var sourceMap = CreateSourceMapMock();
			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.Line == 5 && y.Column == 5)))
				.Return(new MappingEntry(sourceMapPosition: default(SourcePosition), originalName: "bar"));

			sourceMap.Stub(
				x =>
					x.GetMappingEntryForGeneratedSourcePosition(
						Arg<SourcePosition>.Matches(y => y.Line == 20 && y.Column == 10)))
				.Return(new MappingEntry(sourceMapPosition: default(SourcePosition), originalName: "baz"));

			// Act
			var result = SourceMapExtensions.GetDeminifiedMethodName(sourceMap, bindings);

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
