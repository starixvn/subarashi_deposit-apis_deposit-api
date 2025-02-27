namespace Fanex.DepositAPI.Infrastructure.UnitTests.Repositories;

using System.Data;
using Domain.Entities;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Repositories;
using NSubstitute;

[Collection("DbFactoryFixture")]
public class BalanceUpRepositoryTest : RepositoryBaseTest
{
    private readonly IBalanceUpRepository _balanceUpRepository;
    private readonly IDataReader _dataReader;

    public BalanceUpRepositoryTest(DbFactoryFixture dbFactoryFixture) : base(dbFactoryFixture)
    {
        _dataReader = Substitute.For<IDataReader>();
        _balanceUpRepository = new BalanceUpRepository(dbFactoryFixture);

        ObjectDb.ExecuteReaderAsync().Returns(_dataReader);
    }

    [Theory]
    [InlineData("01/01/0001", "01/01/0001")]
    [InlineData("02/02/0002", "02/02/0002")]
    public async Task GetBalanceUpAsync_Should_ReturnData_WhenDBReturnData(string balanceUpDay, string moveBetsDay)
    {
        // Arrange
        var data = new BalanceUp
        {
            BalanceUpDay = Convert.ToDateTime(balanceUpDay),
            MoveBetsDay = Convert.ToDateTime(moveBetsDay)
        };

        _dataReader.Read().Returns(true);
        MapReaderData(_dataReader, data);

        // Action
        var result = await _balanceUpRepository.GetBalanceUpAsync();

        // Assert
        result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task GetBalanceUpAsync_Should_ReturnNull_WhenDBDoNotReturnData()
    {
        // Arrange
        _dataReader.Read().Returns(false);

        // Action
        var result = await _balanceUpRepository.GetBalanceUpAsync();

        // Assert
        result.Should().BeNull();
    }
}