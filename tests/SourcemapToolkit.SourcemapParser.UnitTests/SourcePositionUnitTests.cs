using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourcePositionUnitTests
	{
		[Fact]
		public void CompareTo_SameLineAndColumn_Equal()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 6);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 6);

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public void CompareTo_LargerZeroBasedLineNumber_ReturnLarger()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 4);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 8);

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.True(result > 0);
		}

		[Fact]
		public void CompareTo_SmallerZeroBasedLineNumber_ReturnSmaller()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 4);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 3,
				zeroBasedColumnNumber: 8);

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.True(result < 0);
		}

		[Fact]
		public void CompareTo_SameLineLargerColumn_ReturnLarger()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 8);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 6);

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.True(result > 0);
		}

		[Fact]
		public void CompareTo_SameLineSmallerColumn_ReturnSmaller()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 4);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 8);

			// Act
			int result = x.CompareTo(y);

			// Assert
			Assert.True(result < 0);
		}

		[Fact]
		public void LessThanOverload_SameZeroBasedLineNumberXColumnSmaller_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 4);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 8);

			// Act
			bool result = x < y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void LessThanOverload_XZeroBasedLineNumberSmallerX_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 10);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 8);

			// Act
			bool result = x < y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void LessThanOverload_Equal_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 0,
				zeroBasedColumnNumber: 0);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 0,
				zeroBasedColumnNumber: 0);

			// Act
			bool result = x < y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void LessThanOverload_XColumnLarger_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 0,
				zeroBasedColumnNumber: 10);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 0,
				zeroBasedColumnNumber: 0);

			// Act
			bool result = x < y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void GreaterThanOverload_SameLineXColumnLarger_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 10);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 3);

			// Act
			bool result = x > y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void GreaterThanOverload_XZeroBasedLineNumberLarger_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 3,
				zeroBasedColumnNumber: 10);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 30);

			// Act
			bool result = x > y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void GreaterThanOverload_Equal_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 3,
				zeroBasedColumnNumber: 10);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 3,
				zeroBasedColumnNumber: 10);

			// Act
			bool result = x > y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void GreaterThanOverload_XSmaller_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 3,
				zeroBasedColumnNumber: 9);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 3,
				zeroBasedColumnNumber: 10);

			// Act
			bool result = x > y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void IsEqualish_XEqualsY_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 13,
				zeroBasedColumnNumber: 5);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 13,
				zeroBasedColumnNumber: 5);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_XColumnNumberBiggerByOne_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 8);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 2,
				zeroBasedColumnNumber: 7);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberBiggerByOne_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 10);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 1,
				zeroBasedColumnNumber: 11);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberBiggerByTwo_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 155,
				zeroBasedColumnNumber: 100);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 155,
				zeroBasedColumnNumber: 102);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void IsEqualish_XColumnNumberZeroLineNumbersDifferByOne_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 235,
				zeroBasedColumnNumber: 0);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 234,
				zeroBasedColumnNumber: 102);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberZeroLineNumbersDifferByOne_ReturnTrue()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 458,
				zeroBasedColumnNumber: 13);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 459,
				zeroBasedColumnNumber: 0);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberZeroLineNumbersDifferByTwo_ReturnFalse()
		{
			// Arrange
			SourcePosition x = new SourcePosition(
				zeroBasedLineNumber: 5456,
				zeroBasedColumnNumber: 13);
			SourcePosition y = new SourcePosition(
				zeroBasedLineNumber: 5458,
				zeroBasedColumnNumber: 0);

			// Act
			bool result = x.IsEqualish(y);

			// Assert
			Assert.False(result);
		}
	}
}