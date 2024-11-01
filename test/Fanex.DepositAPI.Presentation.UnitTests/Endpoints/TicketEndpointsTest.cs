namespace Fanex.DepositAPI.Presentation.UnitTests.Endpoints;

using Application.Queries;
using AutoFixture;
using Domain.Entities;
using Domain.Errors;
using Domain.Results;
using DTOs.Response;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using Presentation.Endpoints;

public class TicketEndpointsTest
{
    private readonly IFixture _fixture = new Fixture();
    private readonly IMediator _mediator = Substitute.For<IMediator>();

    [Fact]
    public async Task GetTicketInformation_Should_ReturnSuccess()
    {
        // Arrange
        var refNo = _fixture.Create<long>();
        var ticketInformation = _fixture.Create<Ticket>();
        _mediator
            .Send(Arg.Any<GetTicketInformationQuery>())
            .Returns(ServiceResult<Ticket>.Success(ticketInformation));

        // Action
        var result = await TicketEndpoints.GetTicketInformation(refNo, null, _mediator);
        var okResult = (Ok<GetTicketInformationResponse>)result;

        // Assert
        okResult.StatusCode.Should().Be(200);
        okResult.Value.ErrorCode.Should().Be(0);
        okResult.Value.ErrorMessage.Should().BeEmpty();
        okResult.Value.Result.Should().Be(ticketInformation);
    }

    [Fact]
    public async Task GetTicketInformation_Should_ReturnFailure()
    {
        // Arrange
        var refNo = _fixture.Create<long>();
        _mediator
            .Send(Arg.Any<GetTicketInformationQuery>())
            .Returns(ServiceResult<Ticket>.Failure(ServiceErrors.SystemError));

        // Action
        var result = await TicketEndpoints.GetTicketInformation(refNo, null, _mediator);
        var okResult = (Ok<GetTicketInformationResponse>)result;

        // Assert
        okResult.StatusCode.Should().Be(200);
        okResult.Value.ErrorCode.Should().Be(ServiceErrors.SystemError.Code);
        okResult.Value.ErrorMessage.Should().Be(ServiceErrors.SystemError.Message);
        okResult.Value.Result.Should().BeNull();
    }
}