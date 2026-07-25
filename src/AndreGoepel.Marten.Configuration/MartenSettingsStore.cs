using Marten;

namespace AndreGoepel.Marten.Configuration;

// Public (not internal): Wolverine's default ServiceLocationPolicy.NotAllowed has its
// generated handler code construct scoped/transient dependencies directly, which
// requires every concrete type in the chain — including this one — to be visible
// outside this assembly. An internal type here silently breaks message handling for
// any consumer using Wolverine (e.g. AndreGoepel.AppFoundation.MailService).
public sealed class MartenSettingsStore(IDocumentStore store) : ISettingsStore
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
