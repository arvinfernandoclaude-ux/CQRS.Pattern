using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Serilog;
using CQRS.Pattern.Application.Common.Interfaces;
using CQRS.Pattern.Infrastructure.Persistence;
using CQRS.Pattern.Infrastructure.HealthChecks;
using CQRS.Pattern.Infrastructure.Logging;

namespace CQRS.Pattern.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddPersistence(services, configuration);
        AddAuth(services, configuration);
        AddHealthChecks(services);
        AddObservability(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddAuth(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication()
            .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

        services.Configure<MicrosoftIdentityOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options => options.AllowWebApiToBeAuthorizedByACL = true);

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    private static void AddHealthChecks(IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<SelfHealthCheck>("self", tags: ["live"])
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
    }

    private static void AddObservability(IServiceCollection services, IConfiguration configuration)
    {
        var azureMonitorConnStr = configuration["AzureMonitor:ConnectionString"];
        if (!string.IsNullOrWhiteSpace(azureMonitorConnStr))
        {
            services.AddOpenTelemetry().UseAzureMonitor();
        }

        services.AddSerilog((services, lc) => lc
            .ReadFrom.Configuration(configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProperty("ApplicationName", "CQRS.Pattern")
            .Enrich.With<ShortCorrelationIdEnricher>(),
            writeToProviders: true);
    }
}
