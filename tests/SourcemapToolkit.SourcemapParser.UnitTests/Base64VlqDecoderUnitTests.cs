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
			string input = "6rB";

			// Act
			List<int> result = Base64VlqDecoder.Decode(input);

			// Assert
			Assert.Equal(new List<int> { 701 }, result);
		}

		[Fact]
		public void Base64VlqDecoder_MultipleEncodedValues_ListWithMultipleValues()
		{
			// Arrange
			string input = "AAgBC";

			// Act
			List<int> result = Base64VlqDecoder.Decode(input);

			// Assert
			Assert.Equal(new List<int> { 0, 0, 16, 1 }, result);
		}

		[Fact]
		public void Base64VlqDecoder_InputIncludesNegativeValue_ListContainsNegativeValue()
		{
			// Arrange
			string input = "CACf";

			// Act
			List<int> result = Base64VlqDecoder.Decode(input);

			// Assert
			Assert.Equal(new List<int> { 1, 0, 1, -15 }, result);
		}
	}
}
