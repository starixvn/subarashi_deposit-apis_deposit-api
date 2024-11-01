namespace Fanex.DepositAPI.Application.UnitTests.Handlers;

using Application.Handlers;
using AutoFixture;
using Domain.Entities;
using Domain.Errors;
using Domain.Repositories;
using Domain.Results;
using FluentAssertions;
using Logging;
using NSubstitute;
using Queries;

public class GetTicketInformationHandlerTest : IDisposable
{
    private static readonly GetTicketInformationQuery Query = new() { RefNo = 1231313131 };
    private static readonly BalanceUp BalanceUp = new() { BalanceUpDay = DateTime.Now, MoveBetsDay = DateTime.Now };

    private readonly IBalanceUpRepository _balanceUpRepository;

    private readonly GetTicketInformationHandler _handler;

    private readonly ILogger _logger;
    private readonly ITicketRepository _ticketRepository;

    public GetTicketInformationHandlerTest()
    {
        _logger = Substitute.For<ILogger>();
        _balanceUpRepository = Substitute.For<IBalanceUpRepository>();
        _ticketRepository = Substitute.For<ITicketRepository>();

        _handler = new GetTicketInformationHandler(_logger, _ticketRepository, _balanceUpRepository);
    }

    public void Dispose()
    {
    }

    [Fact]
    public async Task Handle_Should_ReturnNoInformation_WhenResultReturnNoInformation()
    {
        // Arrange
        var balanceUp = new BalanceUp { BalanceUpDay = DateTime.Now, MoveBetsDay = DateTime.Now };
        _balanceUpRepository.GetBalanceUpAsync().Returns(balanceUp);

        _ticketRepository.GetTicketInformationAsync(Query.RefNo, balanceUp).Returns(ServiceResult<Ticket>.Failure(TicketError.NoInformation));

        // Action
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(TicketError.NoInformation);
    }

    [Fact]
    public async Task Handle_Should_ReturnError_WhenResultReturnError()
    {
        // Arrange
        var error = new ServiceErrors(100, "Test");
        _balanceUpRepository.GetBalanceUpAsync().Returns(BalanceUp);

        _ticketRepository.GetTicketInformationAsync(Query.RefNo, BalanceUp).Returns(ServiceResult<Ticket>.Failure(error));

        // Action
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(error);
    }

    [Fact]
    public async Task Handle_Should_ReturnData_WhenResultReturnData()
    {
        // Arrange
        var fixture = new Fixture();
        var data = fixture.Create<Ticket>();

        _balanceUpRepository.GetBalanceUpAsync().Returns(BalanceUp);

        _ticketRepository.GetTicketInformationAsync(Query.RefNo, BalanceUp).Returns(ServiceResult<Ticket>.Success(data));

        // Action
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().BeEquivalentTo(data);
    }

    [Fact]
    public async Task Handle_Should_ReturnErrorAndLoggingError_WhenErrorHasOccurred()
    {
        // Arrange
        var exception = new Exception("Testing");

        _balanceUpRepository.GetBalanceUpAsync().Returns(BalanceUp);

        _ticketRepository.When(x => x.GetTicketInformationAsync(Query.RefNo, BalanceUp)).Do(x => throw exception);

        // Action
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(TicketError.SystemError);
        await _logger.Received(1).ErrorAsync(Arg.Any<string>());
    }
}