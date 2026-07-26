# marten-configuration

Small NuGet library: a shared `SettingsDocument` base and Marten-backed
`ISettingsStore` for small, singleton admin-configured settings records
(e.g. email server settings, feature flags, cleanup schedules). Every
settings type registered via `AddSettingsDocument<T>()` shares one physical
Marten table instead of each type getting its own one-row table.

## Solution Projects
- `AndreGoepel.Marten.Configuration` — the packable NuGet library
- `AndreGoepel.Marten.Configuration.IntegrationTests` —
  Testcontainers/Postgres-backed tests

Consumed by `AndreGoepel.Marten.Identity` and `AndreGoepel.AppFoundation`,
among others. Plain class library — no ASP.NET Core dependency.

## Naming
- Settings documents: `[Name]SettingsDocument` or `[Name]Settings`,
  implementing `ISettingsDocument<T>` with a `DocumentId` constant/property
- Stores: `[Name]SettingsStore` when a settings type needs its own store
  beyond the generic `ISettingsStore`

## Library Rules
- No caching in `ISettingsStore` — every `LoadAsync` reads fresh from
  Postgres so a saved change takes effect without an application restart.
  Keep it that way; don't add caching without discussing it first.
- `SaveAsync` sets `SettingsDocument.Id` from `T.DocumentId` itself —
  callers never set it manually.
- Public API surface is only `SettingsDocument`, `ISettingsDocument<T>`,
  `ISettingsStore`, `SettingsStoreOptionsExtensions`, `Initialization`,
  `DataProtectorExtensions` — everything else stays `internal`.
- `DataProtectorExtensions.ProtectOrKeepExisting` is the shared
  "protect a new value, or keep the existing ciphertext when the caller
  left the field blank" round trip for a secret field inside a settings
  document (SMTP password, API token, provider credential, ...). It takes
  the caller's own `IDataProtector` — this library never constructs one
  itself, so it only depends on `Microsoft.AspNetCore.DataProtection.Abstractions`
  (interfaces only), not the full ASP.NET Core shared framework.
- Exception: `MartenSettingsStore` is `public` (not `internal`) despite the
  rule above — Wolverine's `NotAllowed` service-location policy constructs
  it directly in generated handler code and requires public visibility.

## Testing
- `AndreGoepel.Marten.Configuration.Tests` — pure unit tests, no I/O
  (e.g. `DataProtectorExtensions`)
- `AndreGoepel.Marten.Configuration.IntegrationTests` — the Marten store
  and schema-hierarchy behavior (round-trip, shared-table mapping); needs
  Docker for the Postgres container

## Conventions Audit
- `.editorconfig` is intentionally the 26-line variant (no Razor/CSS/JS
  sections), not the 45-line variant shared by `marten-identity`,
  `app-foundation`, `finance-app`, `customer-portal`, and
  `andregoepel-dev`. This repo is a plain C# class library with no
  `.razor`/`.cshtml`/`.css`/`.scss`/`.js` files, so those sections would
  have nothing to apply to. Re-evaluate only if this repo ever gains
  web-facing content (it shouldn't, per "no ASP.NET Core dependency"
  above) — don't extend it just to match the other repos byte-for-byte.
