namespace AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

public sealed class FirstTestSettings : SettingsDocument, ISettingsDocument<FirstTestSettings>
{
    public static string DocumentId => "first-test-settings";

    public string Value { get; set; } = string.Empty;
}
