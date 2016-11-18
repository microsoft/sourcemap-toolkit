using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class Base64ConverterUnitTests
	{
		[TestMethod]
		public void FromBase64_ValidBase64InputC_CorrectIntegerOutput2()
		{
			// Act
			int value = Base64Converter.FromBase64('C');

			// Assert
			Assert.AreEqual(2, value);
		}

		[TestMethod]
		public void FromBase64_ValidBase64Input9_CorrectIntegerOutput61()
		{
			// Act
			int value = Base64Converter.FromBase64('9');

			// Assert
			Assert.AreEqual(61, value);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void FromBase64_InvalidBase64Input_ThrowsException()
		{
			// Act
			Base64Converter.FromBase64('@');
		}

		[TestMethod]
		public void ToBase64_ValidIntegerInput61_CorrectBase64Output9()
		{
			// Act
			char value = Base64Converter.ToBase64(61);

			// Assert
			Assert.AreEqual('9', value);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ToBase64_NegativeIntegerInput_ThrowsException()
		{
			// Act
			Base64Converter.ToBase64(-1);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void ToBase64_InvalidIntegerInput_ThrowsException()
		{
			// Act
			Base64Converter.ToBase64(64);
		}
	}
}
