using Microsoft.AspNetCore.Diagnostics.HealthChecks;

using Scalar.AspNetCore;
using Serilog;
using Serilog.Events;

using CQRS.Pattern.Application;
using CQRS.Pattern.Infrastructure;
using CQRS.Pattern.WebApi.ExceptionHandlers;
using CQRS.Pattern.WebApi.Middleware;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();
try
{
    Log.Information("Starting CQRS.Pattern API");
    var builder = WebApplication.CreateBuilder(args);

    // Remove default console logger to prevent duplicate output (Serilog owns the console)
    builder.Logging.ClearProviders();

    // Layer service registration (Auth, OpenTelemetry + Serilog registered inside AddInfrastructure)
    builder.Services.AddApplication()
                    .AddInfrastructure(builder.Configuration);

    // Exception handling
    builder.Services.AddExceptionHandler<ApplicationExceptionHandler>();
    builder.Services.AddProblemDetails();

    // API services
    builder.Services.AddControllers();
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // 1. Exception handling (first -- catches all)
    app.UseExceptionHandler();

    app.UseMiddleware<CorrelationIdMiddleware>();

    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi().AllowAnonymous();
        app.MapScalarApiReference().AllowAnonymous();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Health checks — Azure App Service pings /health to determine instance health
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => true
    }).AllowAnonymous();
    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live")
    }).AllowAnonymous();
    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    }).AllowAnonymous();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

// Required for WebApplicationFactory<Program> in integration tests
public partial class Program { }
