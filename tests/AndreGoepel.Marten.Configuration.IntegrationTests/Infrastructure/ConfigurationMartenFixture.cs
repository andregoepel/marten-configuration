using AndreGoepel.Marten.Configuration;
using AndreGoepel.Marten.Testing;
using Marten;

namespace AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

/// <summary>
/// Registers this repo's test settings documents as <see cref="SettingsDocument"/> subclasses
/// before the shared <see cref="MartenFixture"/> creates the schema.
/// </summary>
public sealed class ConfigurationMartenFixture : MartenFixture
{
    protected override void ConfigureStore(StoreOptions options)
    {
        options.AddSettingsDocument<FirstTestSettings>();
        options.AddSettingsDocument<SecondTestSettings>();
    }
}
