namespace AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

/// <summary>
/// An ordinary Marten document (not a <see cref="SettingsDocument"/>) so the generic session
/// helpers are exercised against the shape they target ecosystem-wide.
/// </summary>
public sealed class PlainTestDocument
{
    public string Id { get; set; } = string.Empty;

    public int Count { get; set; }
}
