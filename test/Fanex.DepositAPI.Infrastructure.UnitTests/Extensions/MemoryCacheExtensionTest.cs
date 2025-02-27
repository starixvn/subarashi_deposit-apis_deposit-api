namespace Fanex.DepositAPI.Infrastructure.UnitTests.Extensions;

using FluentAssertions;
using Infrastructure.Extensions;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

public class MemoryCacheExtensionTest
{
    [Fact]
    public async Task GetOrCreateFromCache_Should_ReturnCachedValue_WhenValueExists()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var cachedValue = "cachedValue";
        cache.TryGetValue(key, out Arg.Any<object>()).Returns(x =>
        {
            x[1] = cachedValue;
            return true;
        });

        // Act
        var result = await cache.GetOrCreateFromCache(key, () => Task.FromResult("newValue"), TimeSpan.FromMinutes(5));

        // Assert
        result.Should().Be(cachedValue);
    }

    [Fact]
    public async Task GetOrCreateFromCache_Should_ReturnNewValue_WhenValueDoesNotExist()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var newValue = "newValue";
        cache.TryGetValue(key, out Arg.Any<object>()).Returns(false);

        // Act
        var result = await cache.GetOrCreateFromCache(key, () => Task.FromResult(newValue), TimeSpan.FromMinutes(5));

        // Assert
        result.Should().Be(newValue);
        cache.Received(1).SetValueToCache(key, newValue, TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task GetOrCreateFromCacheWithoutExpired_Should_ReturnCachedValue_WhenValueExists()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var cachedValue = "cachedValue";
        cache.TryGetValue(key, out Arg.Any<object>()).Returns(x =>
        {
            x[1] = cachedValue;
            return true;
        });

        // Act
        var result = await cache.GetOrCreateFromCacheWithoutExpired(key, () => Task.FromResult("newValue"));

        // Assert
        result.Should().Be(cachedValue);
    }

    [Fact]
    public async Task GetOrCreateFromCacheWithoutExpired_Should_ReturnNewValue_WhenValueDoesNotExist()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var newValue = "newValue";
        cache.TryGetValue(key, out Arg.Any<object>()).Returns(false);

        // Act
        var result = await cache.GetOrCreateFromCacheWithoutExpired(key, () => Task.FromResult(newValue));

        // Assert
        result.Should().Be(newValue);
        cache.Received(1).SetValueToCacheWithoutExpired(key, newValue);
    }

    [Fact]
    public void GetValueFromCache_Should_ReturnCachedValue_WhenValueExists()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var cachedValue = "cachedValue";
        cache.TryGetValue(key, out Arg.Any<object>()).Returns(x =>
        {
            x[1] = cachedValue;
            return true;
        });

        // Act
        var result = cache.GetValueFromCache(key, "defaultValue");

        // Assert
        result.Should().Be(cachedValue);
    }

    [Fact]
    public void GetValueFromCache_Should_ReturnDefaultValue_WhenValueDoesNotExist()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        cache.TryGetValue(key, out Arg.Any<object>()).Returns(false);

        // Act
        var result = cache.GetValueFromCache(key, "defaultValue");

        // Assert
        result.Should().Be("defaultValue");
    }

    [Fact]
    public void RemoveFromCache_Should_RemoveValueFromCache()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";

        // Act
        cache.RemoveFromCache(key);

        // Assert
        cache.Received(1).Remove(key);
    }

    [Fact]
    public void SetValueToCache_Should_SetValueInCache()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var value = "value";
        var cacheTime = TimeSpan.FromMinutes(5);

        // Act
        cache.SetValueToCache(key, value, cacheTime);

        // Assert
        cache.Received(1).Set(key, value, cacheTime);
    }

    [Fact]
    public void SetValueToCacheWithoutExpired_Should_SetValueInCacheWithoutExpiration()
    {
        // Arrange
        var cache = Substitute.For<IMemoryCache>();
        var key = "testKey";
        var value = "value";
        var cacheEntry = Substitute.For<ICacheEntry>();

        cache.CreateEntry(key).Returns(cacheEntry);

        // Act
        cache.SetValueToCacheWithoutExpired(key, value);

        // Assert
        cache.Received(1).CreateEntry(key);
        cacheEntry.Value.Should().Be(value);
        cacheEntry.Priority.Should().Be(CacheItemPriority.NeverRemove);
    }
}