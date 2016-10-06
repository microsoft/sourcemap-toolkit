using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests
{
	[TestClass]
	public class KeyValueCacheUnitTests
	{
		[TestMethod]
		public void GetValue_KeyNotInCache_CallValueGetter()
		{
			// Arrange
			Func<string, string> valueGetter = MockRepository.GenerateStrictMock<Func<string, string>>();
			valueGetter.Stub(x => x("bar")).Return("foo");
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>(valueGetter);

			// Act
			string result = keyValueCache.GetValue("bar");

			// Assert
			Assert.AreEqual("foo", result);

		}

		[TestMethod]
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
			Assert.AreEqual("foo", result);
			valueGetter.VerifyAllExpectations();
		}

		[TestMethod]
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
			Assert.AreEqual(null, result);
			valueGetter.VerifyAllExpectations();
		}

		[TestMethod]
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
			Assert.AreEqual("foo", result);
			valueGetter.VerifyAllExpectations();
		}
	}
}
