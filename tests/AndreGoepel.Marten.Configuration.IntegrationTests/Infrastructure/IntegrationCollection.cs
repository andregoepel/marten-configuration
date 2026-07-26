namespace AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class IntegrationCollection : ICollectionFixture<ConfigurationMartenFixture>
{
    public const string Name = "Integration";
}
