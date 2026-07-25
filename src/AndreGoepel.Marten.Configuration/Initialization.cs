using AndreGoepel.Marten.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class Initialization
{
    /// <summary>
    /// Registers <see cref="ISettingsStore"/>. Safe to call more than once (e.g. from several
    /// packages that each depend on settings persistence) — the first registration wins.
    /// Singleton because <see cref="MartenSettingsStore"/> holds no per-request state, only a
    /// reference to the also-Singleton <c>IDocumentStore</c> Marten registers — every call
    /// opens its own short-lived session regardless of the store's own lifetime.
    /// </summary>
    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.TryAddSingleton<ISettingsStore, MartenSettingsStore>();
        return services;
    }
}
