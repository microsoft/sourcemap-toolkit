using Xunit;

namespace SourcemapToolkit.SourcemapParser.UnitTests
{

	public class SourcePositionUnitTests
	{
		[Fact]
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
			Assert.Equal(0, result);
		}

		[Fact]
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
			Assert.True(result > 0);
		}

		[Fact]
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
			Assert.True(result < 0);
		}

		[Fact]
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
			Assert.True(result > 0);
		}

		[Fact]
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
			Assert.True(result < 0);
		}

		[Fact]
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
			Assert.True(result);
		}

		[Fact]
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
			Assert.True(result);
		}

		[Fact]
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
			Assert.False(result);
		}

		[Fact]
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
			Assert.False(result);
		}

		[Fact]
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
			Assert.True(result);
		}

		[Fact]
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
			Assert.True(result);
		}

		[Fact]
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
			Assert.False(result);
		}

		[Fact]
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
			Assert.False(result);
		}

        [Fact]
        public void IsEqualish_XEqualsY_ReturnTrue()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 13,
                ZeroBasedColumnNumber = 5
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 13,
                ZeroBasedColumnNumber = 5
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEqualish_XColumnNumberBiggerByOne_ReturnTrue()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 2,
                ZeroBasedColumnNumber = 8
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 2,
                ZeroBasedColumnNumber = 7
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEqualish_YColumnNumberBiggerByOne_ReturnTrue()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 1,
                ZeroBasedColumnNumber = 10
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 1,
                ZeroBasedColumnNumber = 11
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEqualish_YColumnNumberBiggerByTwo_ReturnFalse()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 155,
                ZeroBasedColumnNumber = 100
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 155,
                ZeroBasedColumnNumber = 102
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsEqualish_XColumnNumberZeroLineNumbersDifferByOne_ReturnTrue()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 235,
                ZeroBasedColumnNumber = 0
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 234,
                ZeroBasedColumnNumber = 102
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEqualish_YColumnNumberZeroLineNumbersDifferByOne_ReturnTrue()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 458,
                ZeroBasedColumnNumber = 13
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 459,
                ZeroBasedColumnNumber = 0
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEqualish_YColumnNumberZeroLineNumbersDifferByTwo_ReturnFalse()
        {
            // Arrange
            SourcePosition x = new SourcePosition
            {
                ZeroBasedLineNumber = 5456,
                ZeroBasedColumnNumber = 13
            };
            SourcePosition y = new SourcePosition
            {
                ZeroBasedLineNumber = 5458,
                ZeroBasedColumnNumber = 0
            };

            // Act
            bool result = x.IsEqualish(y);

            // Assert
            Assert.False(result);
        }
    }
}