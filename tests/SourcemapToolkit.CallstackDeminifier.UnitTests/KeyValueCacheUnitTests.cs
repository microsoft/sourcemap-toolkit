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
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>();
			Func<string> valueGetter = MockRepository.GenerateStrictMock<Func<string>>();
			valueGetter.Stub(x => x()).Return("foo");

			// Act
			string result = keyValueCache.GetValue("bar", valueGetter);

			// Assert
			Assert.AreEqual("foo", result);

		}

		[TestMethod]
		public void GetValue_CallGetTwice_OnlyCallValueGetterOnce()
		{
			// Arrange
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>();
			Func<string> valueGetter = MockRepository.GenerateStrictMock<Func<string>>();
			valueGetter.Stub(x => x()).Return("foo").Repeat.Once();
			keyValueCache.GetValue("bar", valueGetter); // Place the value in the cache

			// Act
			string result = keyValueCache.GetValue("bar", valueGetter);

			// Assert
			Assert.AreEqual("foo", result);
			valueGetter.VerifyAllExpectations();
		}

		[TestMethod]
		public void GetValue_CallGetTwiceValueGetterReturnsNull_CallGetterTwice()
		{
			// Arrange
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>();
			Func<string> valueGetter = MockRepository.GenerateStrictMock<Func<string>>();
			valueGetter.Stub(x => x()).Return(null).Repeat.Twice();
			keyValueCache.GetValue("bar", valueGetter); // Place null in the cache

			// Act
			string result = keyValueCache.GetValue("bar", valueGetter);

			// Assert
			Assert.AreEqual(null, result);
			valueGetter.VerifyAllExpectations();
		}

		[TestMethod]
		public void GetValue_CallGetMultipleTimesFirstGetterReturnsNull_CacheFirstNonNullValue()
		{
			// Arrange
			KeyValueCache<string, string> keyValueCache = new KeyValueCache<string, string>();
			Func<string> valueGetter = MockRepository.GenerateStrictMock<Func<string>>();
			valueGetter.Stub(x => x()).Return(null).Repeat.Once();
			keyValueCache.GetValue("bar", valueGetter); // Place null in the cache
			valueGetter.Stub(x => x()).Return("foo").Repeat.Once();
			keyValueCache.GetValue("bar", valueGetter); // Place a non null value in the cahce

			// Act
			string result = keyValueCache.GetValue("bar", valueGetter);

			// Assert
			Assert.AreEqual("foo", result);
			valueGetter.VerifyAllExpectations();
		}
	}
}
