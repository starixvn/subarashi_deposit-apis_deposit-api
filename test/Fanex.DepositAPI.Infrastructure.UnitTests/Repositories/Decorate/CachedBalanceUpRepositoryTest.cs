namespace Fanex.DepositAPI.Infrastructure.UnitTests.Repositories.Decorate;

using AutoFixture;
using Constants;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Repositories.Decorate;
using NSubstitute;

public class CachedBalanceUpRepositoryTest : CachedRepositoryBaseTest
{
    private readonly IBalanceUpRepository _balanceUpRepository;
    private readonly IBalanceUpRepository _cachedBalanceUpRepository;

    public CachedBalanceUpRepositoryTest()
    {
        _balanceUpRepository = Substitute.For<IBalanceUpRepository>();
        _cachedBalanceUpRepository = new CachedBalanceUpRepository(_balanceUpRepository, MemoryCache);
    }

    public override void Dispose()
    {
        _balanceUpRepository.ClearReceivedCalls();
        base.Dispose();
    }

    [Fact]
    public async Task GetBalanceUpAsync_Should_ReturnCachedBalanceUpDataWhenAvailable()
    {
        // Arrange
        var balanceUp = Fixture.Create<BalanceUp>();
        _balanceUpRepository.GetBalanceUpAsync().Returns(balanceUp);
        SetupCacheData(CacheKeys.BalanceUp, balanceUp);

        // Action
        var result = await _cachedBalanceUpRepository.GetBalanceUpAsync();

        // Assert
        result.Should().BeEquivalentTo(balanceUp);
        await _balanceUpRepository.Received(0).GetBalanceUpAsync();
    }

    [Fact]
    public async Task GetBalanceUpAsync_Should_FetchBalanceUpDataFromRepositoryWhenCacheIsEmpty()
    {
        // Arrange
        var balanceUp = Fixture.Create<BalanceUp>();
        _balanceUpRepository.GetBalanceUpAsync().Returns(balanceUp);
        SetupNoCacheData(CacheKeys.BalanceUp);

        // Action
        var result = await _cachedBalanceUpRepository.GetBalanceUpAsync();

        // Assert
        result.Should().BeEquivalentTo(balanceUp);
        await _balanceUpRepository.Received(1).GetBalanceUpAsync();
    }

    [Fact]
    public async Task GetBalanceUpAsync_Should_ReturnNullWhenBalanceUpDataIsNullFromRepository()
    {
        // Arrange
        _balanceUpRepository.GetBalanceUpAsync().Returns((BalanceUp)null!);

        // Action
        var result = await _cachedBalanceUpRepository.GetBalanceUpAsync();

        // Assert
        result.Should().BeNull();
        await _balanceUpRepository.Received(1).GetBalanceUpAsync();
    }

    //[Fact]
    //public async Task GetBalanceUpAsync_Should_HandleConcurrentRequestsByReturningCachedDataUntilNewDataIsFetched()
    //{
    //    // Arrange
    //    var balanceUp = Fixture.Create<BalanceUp>();
    //    _balanceUpRepository.GetBalanceUpAsync().Returns(balanceUp);

    //    var tasks = new List<Task<BalanceUp>>();
    //    for (var i = 0; i < 10; i++)
    //    {
    //        tasks.Add(_cachedBalanceUpRepository.GetBalanceUpAsync());
    //    }

    //    // Action
    //    var results = await Task.WhenAll(tasks);

    //    // Assert
    //    results.Should().AllBeEquivalentTo(balanceUp);
    //    await _balanceUpRepository.Received(1).GetBalanceUpAsync();
    //}
}