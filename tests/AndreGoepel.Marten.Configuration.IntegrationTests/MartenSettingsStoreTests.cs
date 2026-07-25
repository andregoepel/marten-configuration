using AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

namespace AndreGoepel.Marten.Configuration.IntegrationTests;

[Collection(IntegrationCollection.Name)]
public class MartenSettingsStoreTests(MartenFixture fixture) : IAsyncLifetime
{
    private CancellationToken Ct => TestContext.Current.CancellationToken;

    private ISettingsStore Store => new MartenSettingsStore(fixture.Store);

    public async ValueTask InitializeAsync() => await fixture.ResetAsync(Ct);

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task LoadAsync_NothingSaved_ReturnsNull()
    {
        var settings = await Store.LoadAsync<FirstTestSettings>(Ct);

        Assert.Null(settings);
    }

    [Fact]
    public async Task SaveAsync_ThenLoadAsync_RoundTrips()
    {
        await Store.SaveAsync(new FirstTestSettings { Value = "hello" }, Ct);

        var settings = await Store.LoadAsync<FirstTestSettings>(Ct);

        Assert.NotNull(settings);
        Assert.Equal("hello", settings.Value);
    }

    [Fact]
    public async Task SaveAsync_SetsIdToDocumentId_EvenIfCallerLeftItUnset()
    {
        var settings = new FirstTestSettings { Value = "irrelevant" };
        Assert.Equal(string.Empty, settings.Id);

        await Store.SaveAsync(settings, Ct);

        Assert.Equal(FirstTestSettings.DocumentId, settings.Id);
    }

    [Fact]
    public async Task SaveAsync_TwoSettingsTypes_ShareOnePhysicalTable()
    {
        await Store.SaveAsync(new FirstTestSettings { Value = "first" }, Ct);
        await Store.SaveAsync(new SecondTestSettings { Value = 42 }, Ct);

        await using var session = fixture.Store.QuerySession();
        var rowCount = await session.QueryAsync<int>(
            "select count(*) from mt_doc_settingsdocument",
            Ct
        );

        Assert.Equal(2, rowCount.Single());

        var first = await Store.LoadAsync<FirstTestSettings>(Ct);
        var second = await Store.LoadAsync<SecondTestSettings>(Ct);
        Assert.Equal("first", first?.Value);
        Assert.Equal(42, second?.Value);
    }
}
