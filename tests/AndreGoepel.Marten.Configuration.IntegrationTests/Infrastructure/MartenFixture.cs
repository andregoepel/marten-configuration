using AndreGoepel.Marten.Configuration;
using JasperFx;
using Marten;
using Testcontainers.PostgreSql;

namespace AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

/// <summary>
/// Lives for the whole test collection so the Postgres container start-up cost is paid once.
/// </summary>
public sealed class MartenFixture : IAsyncLifetime
{
    private PostgreSqlContainer _container = null!;

    public IDocumentStore Store { get; private set; } = null!;

    // Pin the Postgres image by digest (not a mutable tag) so the test/CI
    // environment can't be fed a different image behind the same tag.
    private const string PostgresImage =
        "postgres:16-alpine@sha256:e013e867e712fec275706a6c51c966f0bb0c93cfa8f51000f85a15f9865a28cb";

    public async ValueTask InitializeAsync()
    {
        _container = new PostgreSqlBuilder(PostgresImage).Build();
        await _container.StartAsync(TestContext.Current.CancellationToken);

        Store = DocumentStore.For(opts =>
        {
            opts.Connection(_container.GetConnectionString());
            opts.AddSettingsDocument<FirstTestSettings>();
            opts.AddSettingsDocument<SecondTestSettings>();
            opts.AutoCreateSchemaObjects = AutoCreate.All;
        });
    }

    public async ValueTask DisposeAsync()
    {
        await Store.DisposeAsync();
        await _container.DisposeAsync();
    }

    // Wipes documents between tests without dropping the schema.
    public async Task ResetAsync(CancellationToken cancellationToken = default)
    {
        await Store.Advanced.Clean.DeleteAllDocumentsAsync(cancellationToken);
    }
}
