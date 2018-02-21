using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class Base64VlqEncoderUnitTests
	{
		[TestMethod]
		public void Base64VlqEncoder_SmallValue_ListWithOnlyOneValue()
		{
			// Act
			var result = new StringBuilder();
			Base64VlqEncoder.Encode(result, 15);

			// Assert
			Assert.AreEqual("e", result.ToString());
		}

		[TestMethod]
		public void Base64VlqEncoder_LargeValue_ListWithOnlyMultipleValues()
		{
			// Act
			var result = new StringBuilder();
			Base64VlqEncoder.Encode(result, 701);

			// Assert
			Assert.AreEqual("6rB", result.ToString());
		}

		[TestMethod]
		public void Base64VlqEncoder_NegativeValue_ListWithCorrectValue()
		{
			// Act
			var result = new StringBuilder();
			Base64VlqEncoder.Encode(result, -15);

			// Assert
			Assert.AreEqual("f", result.ToString());
		}

	}
}
