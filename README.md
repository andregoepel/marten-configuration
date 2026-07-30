# AndreGoepel.Marten.Configuration

Small, singleton admin-configured settings records stored as [Marten](https://martendb.io) documents, sharing one physical table instead of each settings type getting its own one-row table.

## Why

Apps often need a handful of admin-editable settings sections (email server, feature flags, cleanup schedules, ...). Each one is logically a single row: load it, show it in a form, save it back. This package gives that shape a common base so every settings type doesn't need its own hand-rolled Marten load/save boilerplate.

## Usage

```csharp
public sealed class EmailSettingsDocument : SettingsDocument, ISettingsDocument<EmailSettingsDocument>
{
    public static string DocumentId => "email-settings";

    public required string SenderEmail { get; init; }
}
```

Register the document type and the store during startup:

```csharp
builder.Services.AddMartenConfiguration();

builder.Services.AddMarten(options =>
{
    options.Connection(connectionString);
    options.AddSettingsDocument<EmailSettingsDocument>();
});
```

Read and write it through `ISettingsStore`:

```csharp
var settings = await settingsStore.LoadAsync<EmailSettingsDocument>(cancellationToken);

await settingsStore.SaveAsync(
    new EmailSettingsDocument { SenderEmail = "noreply@example.com" },
    cancellationToken
);
```

`LoadAsync` reads fresh from Postgres on every call (no caching), so a saved change takes effect on the next request without an application restart. `SaveAsync` sets `Id` to the type's `DocumentId` for you, so callers never need to remember it.

Every settings type registered via `AddSettingsDocument<T>()` shares Marten's document-hierarchy subclass mapping under `SettingsDocument`, so they all live in one physical table (`mt_doc_settingsdocument`) rather than proliferating one-row tables.

## Session helpers

`SessionExtensions` bundles the recurring open-session → load → store → save-changes shape into three thin helpers, for any Marten document (not just settings):

```csharp
// Load-modify-save without the session boilerplate; SaveChangesAsync runs
// automatically when the delegate completes (and not when it throws).
await store.WithSessionAsync(
    async (session, ct) =>
    {
        var counter = await session.LoadOrDefaultAsync<CounterDocument>("visits", ct);
        counter.Count += 1;
        session.Store(counter);
    },
    cancellationToken
);

// One-shot upsert without a load step.
await store.StoreAndSaveAsync(document, cancellationToken);
```

`WithSessionAsync` also has an overload returning the delegate's result, and `LoadOrDefaultAsync<T>` returns a fresh `new T()` instead of `null` when nothing is persisted yet. These are deliberately thin — anything more belongs on a real Marten session, not a generic repository.
