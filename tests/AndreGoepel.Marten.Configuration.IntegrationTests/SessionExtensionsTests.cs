using AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;
using Marten;

namespace AndreGoepel.Marten.Configuration.IntegrationTests;

[Collection(IntegrationCollection.Name)]
public sealed class SessionExtensionsTests(ConfigurationMartenFixture fixture) : IAsyncLifetime
{
    private CancellationToken Ct => TestContext.Current.CancellationToken;

    private IDocumentStore Store => fixture.Store;

    public async ValueTask InitializeAsync() => await fixture.ResetAsync(Ct);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task WithSessionAsync_StoresDocument_PersistsAfterDelegateCompletes()
    {
        // Act
        await Store.WithSessionAsync(
            (session, ct) =>
            {
                session.Store(new PlainTestDocument { Id = "with-session", Count = 1 });
                return Task.CompletedTask;
            },
            Ct
        );

        // Assert
        await using var session = Store.QuerySession();
        var document = await session.LoadAsync<PlainTestDocument>("with-session", Ct);
        Assert.NotNull(document);
        Assert.Equal(1, document.Count);
    }

    [Fact]
    public async Task WithSessionAsync_DelegateThrows_PersistsNothing()
    {
        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Store.WithSessionAsync(
                (session, ct) =>
                {
                    session.Store(new PlainTestDocument { Id = "aborted", Count = 1 });
                    throw new InvalidOperationException("boom");
                },
                Ct
            )
        );

        // Assert
        await using var session = Store.QuerySession();
        var document = await session.LoadAsync<PlainTestDocument>("aborted", Ct);
        Assert.Null(document);
    }

    [Fact]
    public async Task WithSessionAsync_WithResult_ReturnsDelegateResultAndSaves()
    {
        // Arrange
        await Store.StoreAndSaveAsync(new PlainTestDocument { Id = "counter", Count = 1 }, Ct);

        // Act
        var newCount = await Store.WithSessionAsync(
            async (session, ct) =>
            {
                var document = await session.LoadOrDefaultAsync<PlainTestDocument>("counter", ct);
                document.Count += 1;
                session.Store(document);
                return document.Count;
            },
            Ct
        );

        // Assert
        Assert.Equal(2, newCount);
        await using var session = Store.QuerySession();
        var persisted = await session.LoadAsync<PlainTestDocument>("counter", Ct);
        Assert.Equal(2, persisted?.Count);
    }

    [Fact]
    public async Task LoadOrDefaultAsync_NothingSaved_ReturnsFreshDefault()
    {
        // Act
        await using var session = Store.QuerySession();
        var document = await session.LoadOrDefaultAsync<PlainTestDocument>("missing", Ct);

        // Assert
        Assert.NotNull(document);
        Assert.Equal(string.Empty, document.Id);
        Assert.Equal(0, document.Count);
    }

    [Fact]
    public async Task LoadOrDefaultAsync_DocumentExists_ReturnsPersistedDocument()
    {
        // Arrange
        await Store.StoreAndSaveAsync(new PlainTestDocument { Id = "existing", Count = 7 }, Ct);

        // Act
        await using var session = Store.QuerySession();
        var document = await session.LoadOrDefaultAsync<PlainTestDocument>("existing", Ct);

        // Assert
        Assert.Equal("existing", document.Id);
        Assert.Equal(7, document.Count);
    }

    [Fact]
    public async Task StoreAndSaveAsync_Document_RoundTrips()
    {
        // Act
        await Store.StoreAndSaveAsync(new PlainTestDocument { Id = "one-shot", Count = 3 }, Ct);

        // Assert
        await using var session = Store.QuerySession();
        var document = await session.LoadAsync<PlainTestDocument>("one-shot", Ct);
        Assert.NotNull(document);
        Assert.Equal(3, document.Count);
    }

    [Fact]
    public async Task StoreAndSaveAsync_ExistingDocument_Upserts()
    {
        // Arrange
        await Store.StoreAndSaveAsync(new PlainTestDocument { Id = "upsert", Count = 1 }, Ct);

        // Act
        await Store.StoreAndSaveAsync(new PlainTestDocument { Id = "upsert", Count = 2 }, Ct);

        // Assert
        await using var session = Store.QuerySession();
        var document = await session.LoadAsync<PlainTestDocument>("upsert", Ct);
        Assert.Equal(2, document?.Count);
    }
}
