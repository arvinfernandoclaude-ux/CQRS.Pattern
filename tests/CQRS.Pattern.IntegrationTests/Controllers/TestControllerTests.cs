using System.Net;

using CQRS.Pattern.IntegrationTests.Extensions;
using CQRS.Pattern.IntegrationTests.Fixtures;

using Xunit;

namespace CQRS.Pattern.IntegrationTests.Controllers;

[Collection("Integration")]
public sealed class TestControllerTests(CustomWebApplicationFactory factory)
{
    [Fact]
    public async Task PublicEndpoint_ReturnsOk_WithoutAuthentication()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/test/public");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("OK", body);
    }

    [Fact]
    public async Task PrivateEndpoint_ReturnsUnauthorized_WithoutAuthentication()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/test/private");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PrivateEndpoint_ReturnsOk_WithAuthentication()
    {
        // Arrange
        using var client = factory.CreateClient().WithAuth();

        // Act
        var response = await client.GetAsync("/api/test/private");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("OK", body);
    }
}
