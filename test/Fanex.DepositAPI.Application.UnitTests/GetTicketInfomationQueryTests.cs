using AutoFixture;
using Fanex.DepositAPI.Application.Handlers;
using Fanex.DepositAPI.Application.Queries;
using Fanex.DepositAPI.Domain.Entities;
using Fanex.DepositAPI.Domain.Errors;
using Fanex.DepositAPI.Domain.Repositories;
using Fanex.DepositAPI.Domain.Results;
using Fanex.Logging;
using FluentAssertions;
using NSubstitute;

namespace Fanex.DepositAPI.Application.UnitTests;

[TestFixture]
public class GetTicketInfomationQueryTests
{
    private static readonly GetTicketInfomationQuery Query = new GetTicketInfomationQuery { Refno = 1231313131 };

    private readonly GetTicketInfomationHandler _handler;

    private readonly ILogger _logger;
    private readonly IBalanceUpRepository _balanceUpRepository;
    private readonly ITicketRepository _ticketRepository;

    public GetTicketInfomationQueryTests()
    {
        _logger = Substitute.For<ILogger>();
        _balanceUpRepository = Substitute.For<IBalanceUpRepository>();
        _ticketRepository = Substitute.For<ITicketRepository>();

        _handler = new GetTicketInfomationHandler(_logger, _ticketRepository, _balanceUpRepository);
    }

    [Test]
    public async Task Handle_Should_ReturnNoInfomation_WhenResultReturnNoInfomation()
    {
        // Arrange
        var balanceUp = new BalanceUp { BalanceUpDay = DateTime.Now, MoveBetsDay = DateTime.Now };
        _balanceUpRepository.GetBalanceUp().Returns(balanceUp);

        _ticketRepository.GetTicketInfomation(Query.Refno, balanceUp).Returns(ServiceResult<Ticket>.Failure(TicketError.NoInfomation));

        // Act
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TicketError.NoInfomation);
    }

    [Test]
    public async Task Handle_Should_ReturnError_WhenResultReturnError()
    {
        // Arrange
        var balanceUp = new BalanceUp { BalanceUpDay = DateTime.Now, MoveBetsDay = DateTime.Now };
        var error = new ServiceErrors(100, "Test");
        _balanceUpRepository.GetBalanceUp().Returns(balanceUp);

        _ticketRepository.GetTicketInfomation(Query.Refno, balanceUp).Returns(ServiceResult<Ticket>.Failure(error));

        // Act
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Test]
    public async Task Handle_Should_ReturnData_WhenResultReturnData()
    {
        // Arrange
        Fixture fixture = new Fixture();
        var balanceUp = new BalanceUp { BalanceUpDay = DateTime.Now, MoveBetsDay = DateTime.Now };
        var data = fixture.Create<Ticket>();

        _balanceUpRepository.GetBalanceUp().Returns(balanceUp);

        _ticketRepository.GetTicketInfomation(Query.Refno, balanceUp).Returns(ServiceResult<Ticket>.Success(data));

        // Act
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Result.Should().Be(data);
    }

    [Test]
    public async Task Handle_Should_ReturnErrorAndLoggingError_WhenErrorHasOccurred()
    {
        // Arrange
        var balanceUp = new BalanceUp { BalanceUpDay = DateTime.Now, MoveBetsDay = DateTime.Now };
        var exception = new Exception("Testing");

        _balanceUpRepository.GetBalanceUp().Returns(balanceUp);

        _ticketRepository.When(x => x.GetTicketInfomation(Query.Refno, balanceUp)).Do(x => { throw exception; });

        // Act
        var result = await _handler.Handle(Query, default);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TicketError.SystemError);
        _logger.Received(1).Error(Arg.Any<string>());
    }
}