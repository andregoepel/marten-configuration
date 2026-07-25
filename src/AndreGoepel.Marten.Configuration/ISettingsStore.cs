namespace AndreGoepel.Marten.Configuration;

/// <summary>
/// Load/save seam for admin-configured settings documents. Reads go straight to Postgres on
/// every call (no caching), so a saved change takes effect on the next request without an
/// application restart.
/// </summary>
public interface ISettingsStore
{
    /// <summary>Returns the persisted record, or <c>null</c> when nothing has been saved yet.</summary>
    Task<T?> LoadAsync<T>(CancellationToken cancellationToken = default)
        where T : SettingsDocument, ISettingsDocument<T>;

    /// <summary>Persists the record, setting <see cref="SettingsDocument.Id"/> to <c>T.DocumentId</c>.</summary>
    Task SaveAsync<T>(T settings, CancellationToken cancellationToken = default)
        where T : SettingsDocument, ISettingsDocument<T>;
}
