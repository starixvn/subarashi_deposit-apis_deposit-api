namespace Fanex.DepositAPI.Presentation.UnitTests.Utils;

using AutoFixture;
using FluentAssertions;
using Presentation.Utils;

public class ObjectMapperTest
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void MapNewPlayerCommand_Should_MapPropertiesCorrectly()
    {
        // Arrange
        var refNo = _fixture.Create<long>();

        // Action
        var getTicketInformationQuery = ObjectMapper.MapGetTicketInfomationQuery(refNo);

        // Assert
        getTicketInformationQuery.RefNo.Should().Be(refNo);
    }
}