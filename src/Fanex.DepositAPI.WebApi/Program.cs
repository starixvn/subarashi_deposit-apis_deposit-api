using Fanex.DepositAPI.Presentation.Extensions;
using Fanex.DepositAPI.WebApi.Startup;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddDependencyInjection(builder.Configuration);

var app = builder.Build();

app.ConfigureHealthCheck();
app.UseForwardedHeaders();

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/api"),
    builder => builder.UseHttpLogging());

app.ConfigureExceptionHandler(builder.Configuration);

app.ConfigureSwagger(builder.Configuration);

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.MapGet("/", () => "Deposit API is running ...").ExcludeFromDescription();

app.MapEndpoints();

app.Run();