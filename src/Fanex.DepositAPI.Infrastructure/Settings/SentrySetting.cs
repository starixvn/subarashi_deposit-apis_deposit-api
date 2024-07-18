namespace Fanex.DepositAPI.Infrastructure.Settings;

public class SentrySetting
{
    public const string SettingName = "Sentry";

    public string Enviroment { get; set; }

    public string Dsn { get; set; }

    public int MaxBreadcrumbs { get; set; }

    public bool SendDefaultPii { get; set; }
}