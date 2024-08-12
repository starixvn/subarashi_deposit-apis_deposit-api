using System.Text.Json.Serialization;
using Fanex.DepositAPI.Domain.Errors;

namespace Fanex.DepositAPI.Domain.Results;

public class ServiceResult<T>
{
    private ServiceResult(bool isSuccess, ServiceErrors error, T result)
    {
        if (isSuccess && error != ServiceErrors.None ||
            !isSuccess && error == ServiceErrors.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = isSuccess;
        Result = result;
        Error = error;
    }

    [JsonIgnore]
    public bool IsSuccess { get; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    public ServiceErrors Error { get; }

    public T Result { get; }

    public static ServiceResult<T> Success(T result) => new(true, ServiceErrors.None, result);

    public static ServiceResult<T> Failure(ServiceErrors error) => new(false, error, default);
}

public class ServiceResult
{
    private ServiceResult(bool isSuccess, ServiceErrors error)
    {
        if (isSuccess && error != ServiceErrors.None ||
            !isSuccess && error == ServiceErrors.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }
        IsSuccess = isSuccess;
        Error = error;
    }

    [JsonIgnore]
    public bool IsSuccess { get; }

    [JsonIgnore]
    public bool IsFailure => !IsSuccess;

    public ServiceErrors Error { get; }

    public static ServiceResult Success() => new(true, ServiceErrors.None);

    public static ServiceResult Failure(ServiceErrors error) => new(false, error);
}