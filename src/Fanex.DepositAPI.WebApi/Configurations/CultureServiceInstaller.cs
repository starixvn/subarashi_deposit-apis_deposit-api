using Microsoft.AspNetCore.Localization;

namespace Fanex.DepositAPI.WebApi.Configurations;

public class CultureServiceInstaller : IServiceInstaller
{
    private static readonly string[] supporteLanguageCode = ["en-US", "zh-CN"];

    public static readonly IReadOnlyDictionary<string, string> LanguageCodeMappers = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "en-US", "en-US" },
            { "en", "en-US" },
            { "zh-CN", "zh-CN" },
            { "cs", "zh-CN" },
        };

    public void Install(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = supporteLanguageCode;

            options
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            options.RequestCultureProviders = [
                new CustomRequestCultureProvider(context=>{
                    var query = context.Request.Query;
                    var langCode = supporteLanguageCode[0];

                    if (query != null && query.ContainsKey("lang")){
                        langCode = LanguageCodeMappers.GetValueOrDefault(query["lang"], supporteLanguageCode[0]);
                    }

                    return Task.FromResult(new ProviderCultureResult(culture: supporteLanguageCode[0], uiCulture: langCode));
                }),
            ];
        });
    }
}