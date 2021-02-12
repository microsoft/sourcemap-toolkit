using System;
using Moq;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class KeyValueCacheUnitTests
	{
		[Fact]
		public void GetValue_KeyNotInCache_CallValueGetter()
		{
			// Arrange
			var valueGetter = new Mock<Func<string, string>>();
			valueGetter.Setup(x => x("bar")).Returns("foo");
			var keyValueCache = new KeyValueCache<string, string>(valueGetter.Object);

			// Act
			var result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Equal("foo", result);

		}

		[Fact]
		public void GetValue_CallGetTwice_OnlyCallValueGetterOnce()
		{
			// Arrange
			var valueGetter = new Mock<Func<string, string>>();
			valueGetter.Setup(x => x("bar")).Returns("foo");
			var keyValueCache = new KeyValueCache<string, string>(valueGetter.Object);
			keyValueCache.GetValue("bar"); // Place the value in the cache

			// Act
			var result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Equal("foo", result);
			valueGetter.Verify(x => x("bar"), Times.Once());
		}

		[Fact]
		public void GetValue_CallGetTwiceValueGetterReturnsNull_CallGetterTwice()
		{
			// Arrange
			var valueGetter = new Mock<Func<string, string>>();
			valueGetter.Setup(x => x("bar")).Returns<string>(null!);
			var keyValueCache = new KeyValueCache<string, string>(valueGetter.Object);
			keyValueCache.GetValue("bar"); // Place null in the cache

			// Act
			var result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Null(result);
			valueGetter.Verify(x => x("bar"), Times.Exactly(2));
		}

		[Fact]
		public void GetValue_CallGetMultipleTimesFirstGetterReturnsNull_CacheFirstNonNullValue()
		{
			// Arrange
			var valueGetter = new Mock<Func<string, string>>();
			valueGetter.Setup(x => x("bar")).Returns<string>(null!);
			var keyValueCache = new KeyValueCache<string, string>(valueGetter.Object);
			keyValueCache.GetValue("bar"); // Place null in the cache
			valueGetter.Verify(x => x("bar"), Times.Once());

			valueGetter.Setup(x => x("bar")).Returns("foo");
			keyValueCache.GetValue("bar"); // Place a non null value in the cahce

			// Act
			var result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Equal("foo", result);
			valueGetter.Verify(x => x("bar"), Times.Exactly(2));
		}
	}
}
