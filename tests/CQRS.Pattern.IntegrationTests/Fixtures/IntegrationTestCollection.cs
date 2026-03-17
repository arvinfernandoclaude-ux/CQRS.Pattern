using Xunit;

namespace CQRS.Pattern.IntegrationTests.Fixtures;

[CollectionDefinition("Integration")]
public class IntegrationTestFixtureDefinition : ICollectionFixture<CustomWebApplicationFactory>;
