using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class Base64VlqDecoderUnitTests
	{
		[TestMethod]
		public void Base64VlqDecoder_SingleEncodedValue_ListWithOnlyOneValue()
		{
			// Arrange
			Base64VlqDecoder decoder = new Base64VlqDecoder();
			string input = "6rB";

			// Act
			List<int> result = decoder.Decode(input);

			// Assert
			CollectionAssert.AreEqual(new List<int> { 701 }, result);
		}

		[TestMethod]
		public void Base64VlqDecoder_MultipleEncodedValues_ListWithMultipleValues()
		{
			// Arrange
			Base64VlqDecoder decoder = new Base64VlqDecoder();
			string input = "AAgBC";

			// Act
			List<int> result = decoder.Decode(input);

			// Assert
			CollectionAssert.AreEqual(new List<int> { 0, 0, 16, 1 }, result);
		}

		[TestMethod]
		public void Base64VlqDecoder_InputIncludesNegativeValue_ListContainsNegativeValue()
		{
			// Arrange
			Base64VlqDecoder decoder = new Base64VlqDecoder();
			string input = "CACf";

			// Act
			List<int> result = decoder.Decode(input);

			// Assert
			CollectionAssert.AreEqual(new List<int> { 1, 0, 1, -15 }, result);
		}
	}
}
