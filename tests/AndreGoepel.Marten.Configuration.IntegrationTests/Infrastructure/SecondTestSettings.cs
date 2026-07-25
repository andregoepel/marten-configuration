namespace AndreGoepel.Marten.Configuration.IntegrationTests.Infrastructure;

public sealed class SecondTestSettings : SettingsDocument, ISettingsDocument<SecondTestSettings>
{
    public static string DocumentId => "second-test-settings";

    public int Value { get; set; }
}
