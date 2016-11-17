using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{
	[TestClass]
	public class Base64VlqEncoderUnitTests
	{
		[TestMethod]
		public void Base64VlqEncoder_SmallValue_ListWithOnlyOneValue()
		{
			// Act
			List<char> result = new List<char>();
			Base64VlqEncoder.Encode( result, 15 );

			// Assert
			Assert.AreEqual( "e", new string( result.ToArray() ) );
		}

		[TestMethod]
		public void Base64VlqEncoder_LargeValue_ListWithOnlyMultipleValues()
		{
			// Act
			List<char> result = new List<char>();
			Base64VlqEncoder.Encode( result, 701 );

			// Assert
			Assert.AreEqual( "6rB", new string( result.ToArray() ) );
		}

		[TestMethod]
		public void Base64VlqEncoder_NegativeValue_ListWithCorrectValue()
		{
			// Act
			List<char> result = new List<char>();
			Base64VlqEncoder.Encode( result, -15 );

			// Assert
			Assert.AreEqual( "f", new string( result.ToArray() ) );
		}

	}
}
