using System.Text;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class Base64VlqEncoderUnitTests
	{
		[Fact]
		public void Base64VlqEncoder_SmallValue_ListWithOnlyOneValue()
		{
			// Act
			var result = new StringBuilder();
			Base64VlqEncoder.Encode(result, 15);

			// Assert
			Assert.Equal("e", result.ToString());
		}

		[Fact]
		public void Base64VlqEncoder_LargeValue_ListWithOnlyMultipleValues()
		{
			// Act
			var result = new StringBuilder();
			Base64VlqEncoder.Encode(result, 701);

			// Assert
			Assert.Equal("6rB", result.ToString());
		}

		[Fact]
		public void Base64VlqEncoder_NegativeValue_ListWithCorrectValue()
		{
			// Act
			var result = new StringBuilder();
			Base64VlqEncoder.Encode(result, -15);

			// Assert
			Assert.Equal("f", result.ToString());
		}

	}
}
