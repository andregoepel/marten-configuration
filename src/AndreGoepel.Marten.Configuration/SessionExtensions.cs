using Marten;

namespace AndreGoepel.Marten.Configuration;

/// <summary>
/// Thin helpers for the recurring open-session → load → store → save-changes shape, so call
/// sites don't repeat the session lifetime boilerplate. Deliberately not a generic repository —
/// anything beyond these three moves belongs on a real Marten session.
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    /// Runs <paramref name="work"/> in a lightweight session and saves the pending changes
    /// before disposing it. When the delegate throws, nothing is persisted.
    /// </summary>
    public static async Task WithSessionAsync(
        this IDocumentStore store,
        Func<IDocumentSession, CancellationToken, Task> work,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(work);

        await using var session = store.LightweightSession();
        await work(session, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Runs <paramref name="work"/> in a lightweight session, saves the pending changes before
    /// disposing it, and returns the delegate's result. When the delegate throws, nothing is
    /// persisted.
    /// </summary>
    public static async Task<TResult> WithSessionAsync<TResult>(
        this IDocumentStore store,
        Func<IDocumentSession, CancellationToken, Task<TResult>> work,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(work);

        await using var session = store.LightweightSession();
        var result = await work(session, cancellationToken);
        await session.SaveChangesAsync(cancellationToken);
        return result;
    }

    /// <summary>
    /// Returns the persisted document, or a fresh <c>new T()</c> when nothing has been saved
    /// under <paramref name="id"/> yet.
    /// </summary>
    public static async Task<T> LoadOrDefaultAsync<T>(
        this IQuerySession session,
        object id,
        CancellationToken cancellationToken = default
    )
        where T : class, new()
    {
        return await session.LoadAsync<T>(id, cancellationToken) ?? new T();
    }

    /// <summary>
    /// Stores <paramref name="document"/> in a lightweight session and saves the change — the
    /// one-shot upsert without a load step.
    /// </summary>
    public static async Task StoreAndSaveAsync<T>(
        this IDocumentStore store,
        T document,
        CancellationToken cancellationToken = default
    )
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(document);

        await using var session = store.LightweightSession();
        session.Store(document);
        await session.SaveChangesAsync(cancellationToken);
    }
}
