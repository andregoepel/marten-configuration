namespace AndreGoepel.Marten.Configuration;

/// <summary>
/// Shared base for small, singleton admin-configured settings records. Register subclasses
/// via <see cref="SettingsStoreOptionsExtensions.AddSettingsDocument{T}"/> so every settings
/// type shares one physical table instead of each getting a one-row table of its own.
/// </summary>
public abstract class SettingsDocument
{
    /// <summary>
    /// Set by <see cref="ISettingsStore"/> from the implementing type's
    /// <c>DocumentId</c> on save — callers never need to set it themselves.
    /// </summary>
    public string Id { get; set; } = string.Empty;
}
