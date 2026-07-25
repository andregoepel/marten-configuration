using AndreGoepel.Marten.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class Initialization
{
    /// <summary>
    /// Registers <see cref="ISettingsStore"/>. Safe to call more than once (e.g. from several
    /// packages that each depend on settings persistence) — the first registration wins.
    /// </summary>
    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.TryAddScoped<ISettingsStore, MartenSettingsStore>();
        return services;
    }
}
