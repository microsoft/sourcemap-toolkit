using System;
using Moq;
using Xunit;

namespace SourcemapToolkit.CallstackDeminifier.UnitTests;

public class KeyValueCacheUnitTests
{
    [Fact]
    public void GetValue_KeyNotInCache_CallValueGetter()
    {
        // Arrange
        Func<string, string> valueGetter = s => s == "bar" ? "foo" : null;
        var keyValueCache = new KeyValueCache<string, string>(valueGetter);

        // Act
        var result = keyValueCache.GetValue("bar");

        // Assert
        Assert.Equal("foo", result);
    }

    [Fact]
    public void GetValue_CallGetTwice_OnlyCallValueGetterOnce()
    {
        // Arrange
        var call = 0;
        Func<string, string> valueGetter = s => call++ == 0 && s == "bar" ? "foo" : null;
        var keyValueCache = new KeyValueCache<string, string>(valueGetter);
        keyValueCache.GetValue("bar"); // Place the value in the cache

        // Act
        var result = keyValueCache.GetValue("bar");

        // Assert
        Assert.Equal("foo", result);
    }

    [Fact]
    public void GetValue_CallGetTwiceValueGetterReturnsNull_CallGetterTwice()
    {
        // Arrange
        Func<string, string> valueGetter = s => s == "bar" ? null : throw new AggregateException();
        var keyValueCache = new KeyValueCache<string, string>(valueGetter);
        keyValueCache.GetValue("bar"); // Place null in the cache

        // Act
        var result = keyValueCache.GetValue("bar");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void GetValue_CallGetMultipleTimesFirstGetterReturnsNull_CacheFirstNonNullValue()
    {
        // Arrange
        string valueGetterReturn = null; 
        Func<string, string> valueGetter = s => s == "bar" ? valueGetterReturn : throw new AggregateException();
        var keyValueCache = new KeyValueCache<string, string>(valueGetter);
        keyValueCache.GetValue("bar"); // Place null in the cache
        valueGetterReturn = "foo";
        keyValueCache.GetValue("bar"); // Place a non null value in the cache

        // Act
        var result = keyValueCache.GetValue("bar");

        // Assert
        Assert.Equal("foo", result);
    }
}