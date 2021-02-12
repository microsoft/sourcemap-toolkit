using System;
using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class Base64ConverterUnitTests
	{
		[Fact]
		public void FromBase64_ValidBase64InputC_CorrectIntegerOutput2()
		{
			// Act
			var value = Base64Converter.FromBase64('C');

			// Assert
			Assert.Equal(2, value);
		}

		[Fact]
		public void FromBase64_ValidBase64Input9_CorrectIntegerOutput61()
		{
			// Act
			var value = Base64Converter.FromBase64('9');

			// Assert
			Assert.Equal(61, value);
		}

		[Fact]
		public void FromBase64_InvalidBase64Input_ThrowsException()
		{
			// Act
			Assert.Throws<ArgumentOutOfRangeException>(() => Base64Converter.FromBase64('@'));
		}

		[Fact]
		public void ToBase64_ValidIntegerInput61_CorrectBase64Output9()
		{
			// Act
			var value = Base64Converter.ToBase64(61);

			// Assert
			Assert.Equal('9', value);
		}

		[Fact]
		public void ToBase64_NegativeIntegerInput_ThrowsException()
		{
			// Act
			Assert.Throws<ArgumentOutOfRangeException>(() => Base64Converter.ToBase64(-1));
		}

		[Fact]
		public void ToBase64_InvalidIntegerInput_ThrowsException()
		{
			// Act
			Assert.Throws<ArgumentOutOfRangeException>(() => Base64Converter.ToBase64(64));
		}
	}
}
