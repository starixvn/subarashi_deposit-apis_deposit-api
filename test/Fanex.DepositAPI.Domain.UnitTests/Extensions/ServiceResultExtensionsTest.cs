namespace Fanex.DepositAPI.Domain.UnitTests.Extensions;

using Domain.Extensions;
using Errors;
using NSubstitute;
using Results;

public class ServiceResultExtensionsTest
{
    [Fact]
    public void Match_WithSuccessResult_ShouldCallOnSuccess()
    {
        // Arrange
        var serviceResult = new ServiceResult<string>(true, ServiceErrors.None, null, "Success Info");
        var onSuccess = Substitute.For<Func<string, string>>();
        var onFailure = Substitute.For<Func<ServiceErrors, string, string>>();

        // Act
        var result = serviceResult.Match(onSuccess, onFailure);

        // Assert
        onSuccess.Received(1).Invoke("Success Info");
        onFailure.DidNotReceive().Invoke(Arg.Any<ServiceErrors>(), Arg.Any<string>());
    }

    [Fact]
    public void Match_WithFailureResult_ShouldCallOnFailure()
    {
        // Arrange
        var serviceErrors = ServiceErrors.SystemError;
        var serviceResult = new ServiceResult<string>(false, serviceErrors, null, "Failure Info");
        var onSuccess = Substitute.For<Func<string, string>>();
        var onFailure = Substitute.For<Func<ServiceErrors, string, string>>();

        // Act
        var result = serviceResult.Match(onSuccess, onFailure);

        // Assert
        onFailure.Received(1).Invoke(serviceErrors, "Failure Info");
        onSuccess.DidNotReceive().Invoke(Arg.Any<string>());
    }

    [Fact]
    public void Match_Generic_WithSuccessResult_ShouldCallOnSuccess()
    {
        // Arrange
        var serviceResult = new ServiceResult<string>(true, ServiceErrors.None, null, "Success Info");
        var onSuccess = Substitute.For<Func<string, string>>();
        var onFailure = Substitute.For<Func<ServiceErrors, string, string>>();

        // Act
        var result = serviceResult.Match(onSuccess, onFailure);

        // Assert
        onSuccess.Received(1).Invoke("Success Info");
        onFailure.DidNotReceive().Invoke(Arg.Any<ServiceErrors>(), Arg.Any<string>());
    }

    [Fact]
    public void Match_Generic_WithFailureResult_ShouldCallOnFailure()
    {
        // Arrange
        var serviceErrors = ServiceErrors.SystemError;
        var serviceResult = new ServiceResult<string>(false, serviceErrors, null, "Failure Info");
        var onSuccess = Substitute.For<Func<string, string>>();
        var onFailure = Substitute.For<Func<ServiceErrors, string, string>>();

        // Act
        var result = serviceResult.Match(onSuccess, onFailure);

        // Assert
        onFailure.Received(1).Invoke(serviceErrors, "Failure Info");
        onSuccess.DidNotReceive().Invoke(Arg.Any<string>());
    }
}