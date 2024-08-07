namespace Fanex.DepositAPI.Infrastructure.Utils;

internal static class SpecialPlatform
{
    public const int Normal = 0;

    public const int Jollysports = 2;

    public const int JollysportCV = 3;
}

public static class SabaUtils
{
    public static bool IsHideSaba(int specialPlatform)
    {
        if (specialPlatform == SpecialPlatform.Jollysports || specialPlatform == SpecialPlatform.JollysportCV)
        {
            return true;
        }
        return false;
    }
}