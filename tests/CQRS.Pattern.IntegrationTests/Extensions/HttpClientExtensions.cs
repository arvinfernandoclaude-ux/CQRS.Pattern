using System.Net.Http.Headers;

using CQRS.Pattern.IntegrationTests.Helpers;

namespace CQRS.Pattern.IntegrationTests.Extensions;

internal static class HttpClientExtensions
{
    internal static HttpClient WithAuth(this HttpClient client, string? token = null)
    {
        token ??= AuthHelper.GenerateToken();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return client;
    }
}
