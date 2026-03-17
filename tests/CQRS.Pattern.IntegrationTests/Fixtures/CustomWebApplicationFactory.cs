using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Serilog;
using Serilog.Events;

using CQRS.Pattern.IntegrationTests.Helpers;

namespace CQRS.Pattern.IntegrationTests.Fixtures;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Reset Serilog's static logger to prevent "logger is already frozen" error
        // when WebApplicationFactory creates subsequent host instances
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        builder.UseEnvironment("IntegrationTest");

        // Override Serilog and AzureAd settings from test-specific config
        var testAssemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddJsonFile(
                Path.Combine(testAssemblyDir, "appsettings.IntegrationTest.json"),
                optional: false);
        });

        builder.ConfigureTestServices(services =>
        {
            // Build a temporary service provider to resolve IConfiguration,
            // then initialize AuthHelper so it reads from appsettings.IntegrationTest.json
            using var sp = services.BuildServiceProvider();
            var configuration = sp.GetRequiredService<IConfiguration>();
            AuthHelper.Initialize(configuration);

            // Override Azure AD's Bearer scheme token validation with symmetric test key.
            // The Bearer scheme is already registered by AddMicrosoftIdentityWebApi in production,
            // so we reconfigure it instead of adding a duplicate scheme.
            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(AuthHelper.SigningKey));

            services.PostConfigureAll<JwtBearerOptions>(options =>
            {
                options.Authority = null;
                options.MetadataAddress = string.Empty;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = AuthHelper.Issuer,
                    ValidateAudience = true,
                    ValidAudience = AuthHelper.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                };
            });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddAuthorization();
        });
    }
}
