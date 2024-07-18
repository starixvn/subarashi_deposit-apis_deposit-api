using System.Net.Mime;
using Fanex.DepositAPI.Infrastructure.Settings;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;

namespace Fanex.DepositAPI.WebApi.Startup;

public static class WebApplicationSetup
{
    public static WebApplication ConfigureExceptionHandler(this WebApplication app, IConfiguration configuration)
    {
        var appsetting = configuration.GetSection(AppSetting.SettingName).Get<AppSetting>();

        if (appsetting.UseErrorHandler)
        {
            app.UseExceptionHandler(opts => { });
        }
        else
        {
            app.UseDeveloperExceptionPage();
        }

        return app;
    }

    public static WebApplication ConfigureSwagger(this WebApplication app, IConfiguration configuration)
    {
        var appsetting = configuration.GetSection(AppSetting.SettingName).Get<AppSetting>();

        if (appsetting.UseSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.EnableTryItOutByDefault();
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"Deposit API v1");
            });
        }

        return app;
    }

    public static IApplicationBuilder ConfigureHealthCheck(this IApplicationBuilder app)
    {
        Func<HttpContext, HealthReport, Task> customResponseWriter = async (context, report) =>
        {
            var result = JsonConvert.SerializeObject(new
            {
                status = report.Status.ToString(),
                monitors = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = Enum.GetName(typeof(HealthStatus), e.Value.Status),
                    description = e.Value.Description,
                    values = e.Value.Data
                })
            }, Formatting.Indented);

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(result);
        };

        app.UseHealthChecks("/whoami", options: new HealthCheckOptions
        {
            AllowCachingResponses = false,
            Predicate = _ => _.Name.Contains("whoami"),
            ResponseWriter = customResponseWriter
        });

        return app;
    }
}