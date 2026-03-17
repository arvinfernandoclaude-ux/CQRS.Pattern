using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CQRS.Pattern.IntegrationTests.Helpers;

internal static class AuthHelper
{
    private static IConfiguration? _configuration;

    internal static void Initialize(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    internal static string Issuer => GetRequired("Jwt:Issuer");
    internal static string Audience => GetRequired("Jwt:Audience");
    internal static string SigningKey => GetRequired("Jwt:SigningKey");

    internal static string GenerateToken(
        string? issuer = null,
        string? audience = null,
        IEnumerable<Claim>? additionalClaims = null,
        DateTime? expires = null)
    {
        EnsureInitialized();

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("sub", GetRequired("Jwt:Claims:Sub")),
            new("oid", GetRequired("Jwt:Claims:Oid")),
            new("tid", GetRequired("Jwt:Claims:Tid")),
            new("appid", GetRequired("Jwt:Claims:AppId")),
            new("appidacr", GetRequired("Jwt:Claims:AppIdAcr")),
            new("ver", GetRequired("Jwt:Claims:Ver")),
        };

        if (additionalClaims is not null)
        {
            claims.AddRange(additionalClaims);
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer ?? Issuer,
            Audience = audience ?? Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires ?? DateTime.UtcNow.AddHours(1),
            SigningCredentials = credentials,
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }

    private static string GetRequired(string key)
    {
        EnsureInitialized();
        return _configuration![key]
            ?? throw new InvalidOperationException($"Missing config key: {key}");
    }

    private static void EnsureInitialized()
    {
        if (_configuration is null)
        {
            throw new InvalidOperationException(
                "AuthHelper.Initialize(IConfiguration) must be called before use. " +
                "This is typically done in CustomWebApplicationFactory.");
        }
    }
}
