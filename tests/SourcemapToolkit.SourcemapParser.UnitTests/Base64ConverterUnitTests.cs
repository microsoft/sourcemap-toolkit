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
	}
}
