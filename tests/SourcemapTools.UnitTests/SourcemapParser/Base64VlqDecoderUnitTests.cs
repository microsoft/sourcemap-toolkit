using System.Collections.Generic;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class Base64VlqDecoderUnitTests
	{
		[Fact]
		public void Base64VlqDecoder_SingleEncodedValue_ListWithOnlyOneValue()
		{
			// Arrange
			var input = "6rB";

			// Act
			var result = Base64VlqDecoder.Decode(input);

			// Assert
			Assert.Equal(new List<int> { 701 }, result);
		}

		[Fact]
		public void Base64VlqDecoder_MultipleEncodedValues_ListWithMultipleValues()
		{
			// Arrange
			var input = "AAgBC";

			// Act
			var result = Base64VlqDecoder.Decode(input);

			// Assert
			Assert.Equal(new List<int> { 0, 0, 16, 1 }, result);
		}

		[Fact]
		public void Base64VlqDecoder_InputIncludesNegativeValue_ListContainsNegativeValue()
		{
			// Arrange
			var input = "CACf";

			// Act
			var result = Base64VlqDecoder.Decode(input);

			// Assert
			Assert.Equal(new List<int> { 1, 0, 1, -15 }, result);
		}
	}
}
