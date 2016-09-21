using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class SourcePositionUnitTests
	{
		[TestMethod]
		public void CompareTo_SameLineAndColumn_Equal()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 6
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 6
			};

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.AreEqual(0, result);
		}

		[TestMethod]
		public void CompareTo_LargerZeroBasedLineNumber_ReturnLarger()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 4
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 8
			};

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.IsTrue(result > 0);
		}

		[TestMethod]
		public void CompareTo_SmallerZeroBasedLineNumber_ReturnSmaller()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 4
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 3,
				ZeroBasedColumnNumber = 8
			};

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.IsTrue(result < 0);
		}

		[TestMethod]
		public void CompareTo_SameLineLargerColumn_ReturnLarger()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 8
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 6
			};

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.IsTrue(result > 0);
		}

		[TestMethod]
		public void CompareTo_SameLineSmallerColumn_ReturnSmaller()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 4
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 8
			};

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.IsTrue(result < 0);
		}

		[TestMethod]
		public void LessThanOverload_SameZeroBasedLineNumberXColumnSmaller_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 4
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 8
			};

			// Act
			bool result = x < y;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void LessThanOverload_XZeroBasedLineNumberSmallerX_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 1,
				ZeroBasedColumnNumber = 10
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 8
			};

			// Act
			bool result = x < y;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void LessThanOverload_Equal_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 0,
				ZeroBasedColumnNumber = 0
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 0,
				ZeroBasedColumnNumber = 0
			};

			// Act
			bool result = x < y;

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void LessThanOverload_XColumnLarger_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 0,
				ZeroBasedColumnNumber = 10
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 0,
				ZeroBasedColumnNumber = 0
			};

			// Act
			bool result = x < y;

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void GreaterThanOverload_SameLineXColumnLarger_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 10
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 3
			};

			// Act
			bool result = x > y;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void GreaterThanOverload_XZeroBasedLineNumberLarger_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 3,
				ZeroBasedColumnNumber = 10
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 2,
				ZeroBasedColumnNumber = 30
			};

			// Act
			bool result = x > y;

			// Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void GreaterThanOverload_Equal_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 3,
				ZeroBasedColumnNumber = 10
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 3,
				ZeroBasedColumnNumber = 10
			};

			// Act
			bool result = x > y;

			// Assert
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void GreaterThanOverload_XSmaller_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition
			{
				ZeroBasedLineNumber = 3,
				ZeroBasedColumnNumber = 9
			};
			SourcePosition y = new SourcePosition
			{
				ZeroBasedLineNumber = 3,
				ZeroBasedColumnNumber = 10
			};

			// Act
			bool result = x > y;

			// Assert
			Assert.IsFalse(result);
		}
	}
}