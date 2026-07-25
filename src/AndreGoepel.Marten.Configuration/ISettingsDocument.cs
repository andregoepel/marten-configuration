namespace AndreGoepel.Marten.Configuration;

/// <summary>
/// Gives a <see cref="SettingsDocument"/> subclass a well-known, type-safe document id, so
/// <see cref="ISettingsStore"/> can load and save it without callers passing a string around.
/// </summary>
public interface ISettingsDocument<TSelf>
    where TSelf : SettingsDocument, ISettingsDocument<TSelf>
{
    static abstract string DocumentId { get; }
}
