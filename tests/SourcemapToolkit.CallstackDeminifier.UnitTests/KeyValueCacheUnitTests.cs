using System;
using Xunit;
using Rhino.Mocks;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{

	public class KeyValueCacheUnitTests
	{
		[Fact]
		public void GetValue_KeyNotInCache_CallValueGetter()
		{
			// Arrange
			Func<string, string> valueGetter = MockRepository.GenerateStrictMock<Func<string, string>>();
			valueGetter.Stub(x => x("bar")).Return("foo");
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>(valueGetter);

			// Act
			string result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Equal("foo", result);

		}

		[Fact]
		public void GetValue_CallGetTwice_OnlyCallValueGetterOnce()
		{
			// Arrange
			Func<string, string> valueGetter = MockRepository.GenerateStrictMock<Func<string, string>>();
			valueGetter.Stub(x => x("bar")).Return("foo").Repeat.Once();
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>(valueGetter);
			keyValueCache.GetValue("bar"); // Place the value in the cache

			// Act
			string result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Equal("foo", result);
			valueGetter.VerifyAllExpectations();
		}

		[Fact]
		public void GetValue_CallGetTwiceValueGetterReturnsNull_CallGetterTwice()
		{
			// Arrange
			Func<string, string> valueGetter = MockRepository.GenerateStrictMock<Func<string, string>>();
			valueGetter.Stub(x => x("bar")).Return(null).Repeat.Twice();
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>(valueGetter);
			keyValueCache.GetValue("bar"); // Place null in the cache

			// Act
			string result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Null(result);
			valueGetter.VerifyAllExpectations();
		}

		[Fact]
		public void GetValue_CallGetMultipleTimesFirstGetterReturnsNull_CacheFirstNonNullValue()
		{
			// Arrange
			Func<string, string> valueGetter = MockRepository.GenerateStrictMock<Func<string, string>>();
			valueGetter.Stub(x => x("bar")).Return(null).Repeat.Once();
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>(valueGetter);
			keyValueCache.GetValue("bar"); // Place null in the cache
			valueGetter.Stub(x => x("bar")).Return("foo").Repeat.Once();
			keyValueCache.GetValue("bar"); // Place a non null value in the cahce

			// Act
			string result = keyValueCache.GetValue("bar");

			// Assert
			Assert.Equal("foo", result);
			valueGetter.VerifyAllExpectations();
		}
	}
}
