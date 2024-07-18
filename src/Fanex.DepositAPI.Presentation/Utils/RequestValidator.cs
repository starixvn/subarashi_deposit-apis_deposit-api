using System.ComponentModel.DataAnnotations;

namespace Fanex.DepositAPI.Presentation.Utils;

public static class RequestValidator
{
    public static bool IsValidRequest(object request, out List<ValidationResult> results)
    {
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(request, validationContext, validationResults, true);
        results = validationResults;
        return isValid;
    }

    public static string GetInvalidParamList(List<ValidationResult> results)
    {
        return string.Join(", ", results.Select(x => string.Join(", ", x.MemberNames)));
    }
}