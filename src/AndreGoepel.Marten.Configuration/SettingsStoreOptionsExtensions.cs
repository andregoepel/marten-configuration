namespace AndreGoepel.Marten.Configuration;

public static class SettingsStoreOptionsExtensions
{
    /// <summary>
    /// Registers <typeparamref name="T"/> as a subclass of <see cref="SettingsDocument"/>, so
    /// it shares the common settings table instead of getting its own one-row table.
    /// </summary>
    public static void AddSettingsDocument<T>(this global::Marten.StoreOptions options)
        where T : SettingsDocument, ISettingsDocument<T>
    {
        options.Schema.For<SettingsDocument>().AddSubClass<T>();
    }
}
