using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourcePositionUnitTests
	{
		[Fact]
		public void CompareTo_SameLineAndColumn_Equal()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 6);
			var y = new SourcePosition(
				line: 1,
				column: 6);

			// Act
			var result = x.CompareTo(y);

			// Assert
			Assert.Equal(0, result);
		}

		[Fact]
		public void CompareTo_LargerZeroBasedLineNumber_ReturnLarger()
		{
			// Arrange
			var x = new SourcePosition(
				line: 2,
				column: 4);
			var y = new SourcePosition(
				line: 1,
				column: 8);

			// Act
			var result = x.CompareTo(y);

			// Assert
			Assert.True(result > 0);
		}

		[Fact]
		public void CompareTo_SmallerZeroBasedLineNumber_ReturnSmaller()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 4);
			var y = new SourcePosition(
				line: 3,
				column: 8);

			// Act
			var result = x.CompareTo(y);

			// Assert
			Assert.True(result < 0);
		}

		[Fact]
		public void CompareTo_SameLineLargerColumn_ReturnLarger()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 8);
			var y = new SourcePosition(
				line: 1,
				column: 6);

			// Act
			var result = x.CompareTo(y);

			// Assert
			Assert.True(result > 0);
		}

		[Fact]
		public void CompareTo_SameLineSmallerColumn_ReturnSmaller()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 4);
			var y = new SourcePosition(
				line: 1,
				column: 8);

			// Act
			var result = x.CompareTo(y);

			// Assert
			Assert.True(result < 0);
		}

		[Fact]
		public void LessThanOverload_SameZeroBasedLineNumberXColumnSmaller_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 4);
			var y = new SourcePosition(
				line: 1,
				column: 8);

			// Act
			var result = x < y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void LessThanOverload_XZeroBasedLineNumberSmallerX_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 10);
			var y = new SourcePosition(
				line: 2,
				column: 8);

			// Act
			var result = x < y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void LessThanOverload_Equal_ReturnFalse()
		{
			// Arrange
			var x = new SourcePosition(
				line: 0,
				column: 0);
			var y = new SourcePosition(
				line: 0,
				column: 0);

			// Act
			var result = x < y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void LessThanOverload_XColumnLarger_ReturnFalse()
		{
			// Arrange
			var x = new SourcePosition(
				line: 0,
				column: 10);
			var y = new SourcePosition(
				line: 0,
				column: 0);

			// Act
			var result = x < y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void GreaterThanOverload_SameLineXColumnLarger_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 2,
				column: 10);
			var y = new SourcePosition(
				line: 2,
				column: 3);

			// Act
			var result = x > y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void GreaterThanOverload_XZeroBasedLineNumberLarger_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 3,
				column: 10);
			var y = new SourcePosition(
				line: 2,
				column: 30);

			// Act
			var result = x > y;

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void GreaterThanOverload_Equal_ReturnFalse()
		{
			// Arrange
			var x = new SourcePosition(
				line: 3,
				column: 10);
			var y = new SourcePosition(
				line: 3,
				column: 10);

			// Act
			var result = x > y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void GreaterThanOverload_XSmaller_ReturnFalse()
		{
			// Arrange
			var x = new SourcePosition(
				line: 3,
				column: 9);
			var y = new SourcePosition(
				line: 3,
				column: 10);

			// Act
			var result = x > y;

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void IsEqualish_XEqualsY_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 13,
				column: 5);
			var y = new SourcePosition(
				line: 13,
				column: 5);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_XColumnNumberBiggerByOne_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 2,
				column: 8);
			var y = new SourcePosition(
				line: 2,
				column: 7);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberBiggerByOne_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 1,
				column: 10);
			var y = new SourcePosition(
				line: 1,
				column: 11);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberBiggerByTwo_ReturnFalse()
		{
			// Arrange
			var x = new SourcePosition(
				line: 155,
				column: 100);
			var y = new SourcePosition(
				line: 155,
				column: 102);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.False(result);
		}

		[Fact]
		public void IsEqualish_XColumnNumberZeroLineNumbersDifferByOne_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 235,
				column: 0);
			var y = new SourcePosition(
				line: 234,
				column: 102);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberZeroLineNumbersDifferByOne_ReturnTrue()
		{
			// Arrange
			var x = new SourcePosition(
				line: 458,
				column: 13);
			var y = new SourcePosition(
				line: 459,
				column: 0);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.True(result);
		}

		[Fact]
		public void IsEqualish_YColumnNumberZeroLineNumbersDifferByTwo_ReturnFalse()
		{
			// Arrange
			var x = new SourcePosition(
				line: 5456,
				column: 13);
			var y = new SourcePosition(
				line: 5458,
				column: 0);

			// Act
			var result = x.IsEqualish(y);

			// Assert
			Assert.False(result);
		}
	}
}