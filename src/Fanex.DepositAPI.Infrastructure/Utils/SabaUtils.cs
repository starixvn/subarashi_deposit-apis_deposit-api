namespace Fanex.DepositAPI.Infrastructure.Utils;

using Constants;

public static class SabaUtils
{
    public static bool IsHideSaba(int specialPlatform)
    {
        return specialPlatform is SpecialPlatform.Jollysports or SpecialPlatform.JollysportCV;
    }
}