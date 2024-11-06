namespace Fanex.DepositAPI.Infrastructure.UnitTests.Repositories;

using System.Collections;
using System.Data;
using Application.Common.Interfaces;
using AutoFixture;
using BetList.Core.Builder;
using BetList.Core.Builder.ChoiceBuilder;
using BetList.Core.Builder.StatusBuilder;
using BetList.Core.Entities;
using BetListNPOI.HSSF.UserModel;
using Constants;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using FluentAssertions;
using Infrastructure.Repositories;
using NSubstitute;
using Utils;
using BetListTicket = BetList.Core.Entities.Ticket;

[Collection("DbFactoryFixture")]
public class TicketRepositoryTest : RepositoryBaseTest
{
    private readonly IBetListExcelBuilder _betListExcelBuilder;
    private readonly IBetListHTMLBuilder _betListHTMLBuilder;
    private readonly IDataReader _dataReader;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITicketHelper _ticketHelper;
    private readonly ITicketRepository _ticketRepository;

    public TicketRepositoryTest(DbFactoryFixture dbFactoryFixture) : base(dbFactoryFixture)
    {
        _ticketHelper = Substitute.For<ITicketHelper>();
        _betListExcelBuilder = Substitute.For<IBetListExcelBuilder>();
        _betListHTMLBuilder = Substitute.For<IBetListHTMLBuilder>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _dataReader = Substitute.For<IDataReader>();
        _ticketRepository = new TicketRepository(
            dbFactoryFixture,
            _betListExcelBuilder,
            _betListHTMLBuilder,
            _ticketHelper,
            _dateTimeProvider);

        ObjectDb.ExecuteReaderAsync().Returns(_dataReader);
    }

    [Theory]
    [InlineData(101, "You have no permission to check the ticket")]
    [InlineData(404, "No information is available")]
    [InlineData(999, "Your request cannot be processed now, please try again later")]
    public async Task GetTicketInformationAsync_Should_ReturnError_WhenDBReturnError(int errorCode, string errorMessage)
    {
        // Arrange
        var refNo = Fixture.Create<long>();
        var balanceUp = Fixture.Create<BalanceUp>();
        _dataReader.Read().Returns(true);

        ObjectDb.GetOutputParams().Returns(new Hashtable
        {
            { DbParam.ErrCode, errorCode },
            { DbParam.ErrMsg, errorMessage }
        });

        // Action
        var result = await _ticketRepository.GetTicketInformationAsync(refNo, balanceUp);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(ServiceErrors.GetFromDbError(errorCode, errorMessage));
        result.ExtraInfo.Should().BeNull();
    }

    [Fact]
    public async Task GetTicketInformationAsync_Should_ReturnSuccess()
    {
        // Arrange
        var refNo = Fixture.Create<long>();
        var balanceUp = Fixture.Create<BalanceUp>();
        var data = Fixture.Create<BetListTicket>();
        _dataReader.Read().Returns(true);
        _dataReader.NextResult().Returns(false);
        DbTestUtils.MapReaderData(_dataReader, data);
        SetupBuilderMock(string.Empty);

        ObjectDb.GetOutputParams().Returns(new Hashtable
        {
            { DbParam.ErrCode, 0 },
            { DbParam.ErrMsg, "" }
        });

        // Action
        var result = await _ticketRepository.GetTicketInformationAsync(refNo, balanceUp);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(ServiceErrors.None);
        result.ExtraInfo.Should().BeNull();
    }

    [Fact]
    public async Task GetTicketInformationAsync_Should_ReturnNoInformation_WhenDBReturnNoInformation()
    {
        // Arrange
        var refNo = Fixture.Create<long>();
        var balanceUp = Fixture.Create<BalanceUp>();
        _dataReader.Read().Returns(false);

        // Action
        var result = await _ticketRepository.GetTicketInformationAsync(refNo, balanceUp);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(TicketError.NoInformation);
        result.ExtraInfo.Should().BeNull();
    }

    [Fact]
    public async Task GetTicketInformationAsync_Should_SetParameterValueCorrect()
    {
        // Arrange
        var refNo = Fixture.Create<long>();
        var balanceUp = Fixture.Create<BalanceUp>();
        _dataReader.Read().Returns(false);

        // Action
        await _ticketRepository.GetTicketInformationAsync(refNo, balanceUp);

        // Assert
        ObjectDb.Received(1).SetParameterValue(DbParam.RefNo, refNo);
        ObjectDb.Received(1).SetParameterValue(DbParam.BalanceUpDay, balanceUp.BalanceUpDay);
        ObjectDb.Received(1).SetParameterValue(DbParam.MoveBetsDay, balanceUp.MoveBetsDay);
    }

    [Theory]
    [InlineData("Won")]
    [InlineData("Lose")]
    [InlineData("Draw")]
    public async Task GetTicketInformationAsync_Should_ReturnTicketInfo_WhenDBReturnData(string statusText)
    {
        // Arrange
        var refNo = Fixture.Create<long>();
        var agentId = Fixture.Create<int>();
        var siteId = Fixture.Create<int>();
        var balanceUp = Fixture.Create<BalanceUp>();
        var data = Fixture.Create<BetListTicket>();
        _dataReader.Read().Returns(true);
        _dataReader.NextResult().Returns(false);
        DbTestUtils.MapReaderData(_dataReader, data);
        SetupBuilderMock(statusText);
        _dataReader[DbParam.PlayerSiteId].Returns(siteId);
        _dataReader[DbParam.Recommend].Returns(agentId);

        // Action
        var result = await _ticketRepository.GetTicketInformationAsync(refNo, balanceUp);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(ServiceErrors.None);
        result.Result.RefNo.Should().Be(_betListHTMLBuilder.BuildRefNo(data));
        result.Result.TransTime.Should().Be(_betListHTMLBuilder.BuildTransTime(data));
        result.Result.LicTxID.Should().Be(_betListHTMLBuilder.BuildLicenseeTxId(data));
        result.Result.SystemId.Should().Be(data.UserName);
        result.Result.MatchId.Should().Be(data.MatchId);
        result.Result.Choice.Should().BeEmpty();
        result.Result.Status.Should().Be(statusText);
        result.Result.SiteId.Should().Be(siteId);
        result.Result.AgentId.Should().Be(agentId);
        result.ExtraInfo.Should().BeNull();
    }

    private void SetupBuilderMock(string statusText)
    {
        var mockChoiceBuilder = Substitute.For<IChoice>();
        var mockStatusBuilder = Substitute.For<IStatus>();
        _betListExcelBuilder.CreateChoiceBuilder(Arg.Any<int>()).Returns(mockChoiceBuilder);
        _betListExcelBuilder.CreateStatusBuilder(Arg.Any<int>()).Returns(mockStatusBuilder);
        mockChoiceBuilder
            .RenderRTF(Arg.Any<ITicket>(), _ticketHelper, Arg.Any<List<ITicketData>>(), false, Arg.Any<RTFHelper>())
            .Returns([]);
        mockStatusBuilder
            .RenderRTF(Arg.Any<ITicket>(), _ticketHelper, Arg.Any<List<ITicketData>>(), false, Arg.Any<RTFHelper>())
            .Returns(new HSSFRichTextString($"{statusText}\nLine2\nLine3"));
    }
}