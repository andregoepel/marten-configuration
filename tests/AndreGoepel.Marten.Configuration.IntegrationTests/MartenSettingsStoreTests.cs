using AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

namespace AndreGoepel.Marten.Configuration.IntegrationTests;

[Collection(IntegrationCollection.Name)]
public sealed class MartenSettingsStoreTests(MartenFixture fixture) : IAsyncLifetime
{
    private CancellationToken Ct => TestContext.Current.CancellationToken;

    private ISettingsStore Store => new MartenSettingsStore(fixture.Store);

    public async ValueTask InitializeAsync() => await fixture.ResetAsync(Ct);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task LoadAsync_NothingSaved_ReturnsNull()
    {
        // Act
        var settings = await Store.LoadAsync<FirstTestSettings>(Ct);

        // Assert
        Assert.Null(settings);
    }

    [Fact]
    public async Task SaveAsync_ThenLoadAsync_RoundTrips()
    {
        // Act
        await Store.SaveAsync(new FirstTestSettings { Value = "hello" }, Ct);
        var settings = await Store.LoadAsync<FirstTestSettings>(Ct);

        // Assert
        Assert.NotNull(settings);
        Assert.Equal("hello", settings.Value);
    }

    [Fact]
    public async Task SaveAsync_SetsIdToDocumentId_EvenIfCallerLeftItUnset()
    {
        // Arrange
        var settings = new FirstTestSettings { Value = "irrelevant" };
        Assert.Equal(string.Empty, settings.Id);

        // Act
        await Store.SaveAsync(settings, Ct);

        // Assert
        Assert.Equal(FirstTestSettings.DocumentId, settings.Id);
    }

    [Fact]
    public async Task SaveAsync_TwoSettingsTypes_ShareOnePhysicalTable()
    {
        // Act
        await Store.SaveAsync(new FirstTestSettings { Value = "first" }, Ct);
        await Store.SaveAsync(new SecondTestSettings { Value = 42 }, Ct);

        await using var session = fixture.Store.QuerySession();
        var rowCount = await session.QueryAsync<int>(
            "select count(*) from mt_doc_settingsdocument",
            Ct
        );

        // Assert
        Assert.Equal(2, rowCount.Single());

        // Act
        var first = await Store.LoadAsync<FirstTestSettings>(Ct);
        var second = await Store.LoadAsync<SecondTestSettings>(Ct);

        // Assert
        Assert.Equal("first", first?.Value);
        Assert.Equal(42, second?.Value);
    }
}
