using System.Net;

using CQRS.Pattern.IntegrationTests.Fixtures;

using Xunit;

namespace CQRS.Pattern.IntegrationTests.Controllers;

[Collection("Integration")]
public sealed class HealthCheckTests(CustomWebApplicationFactory factory)
{
    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task LiveEndpoint_ReturnsOk_WithHealthyStatus()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/live");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Healthy", body);
    }

    [Fact]
    public async Task ReadyEndpoint_ReturnsOk()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health/ready");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task HealthEndpoints_ReturnPlainTextContentType()
    {
        // Arrange
        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.StartsWith("text/plain", response.Content.Headers.ContentType?.MediaType);
    }
}
