using Fanex.DepositAPI.Domain.Errors;
using Fanex.DepositAPI.Domain.Results;

namespace Fanex.DepositAPI.Domain.Extensions;

public static class ServiceResultExtensions
{
    public static T1 Match<T1, T2>(
        this ServiceResult<T2> result,
        Func<T1> onSuccess,
        Func<ServiceErrors, T1> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }

    public static T1 Match<T1>(
        this ServiceResult result,
        Func<T1> onSuccess,
        Func<ServiceErrors, T1> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result.Error);
    }
}