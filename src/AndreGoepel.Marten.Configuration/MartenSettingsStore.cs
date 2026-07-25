using Marten;

namespace AndreGoepel.Marten.Configuration;

internal sealed class MartenSettingsStore(IDocumentStore store) : ISettingsStore
{
    public async Task<T?> LoadAsync<T>(CancellationToken cancellationToken = default)
        where T : SettingsDocument, ISettingsDocument<T>
    {
        await using var session = store.QuerySession();
        return await session.LoadAsync<T>(T.DocumentId, cancellationToken);
    }

    public async Task SaveAsync<T>(T settings, CancellationToken cancellationToken = default)
        where T : SettingsDocument, ISettingsDocument<T>
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Id = T.DocumentId;

        await using var session = store.LightweightSession();
        session.Store(settings);
        await session.SaveChangesAsync(cancellationToken);
    }
}
