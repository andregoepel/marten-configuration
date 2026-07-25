# Project Instructions

## Project Overview

Small NuGet library: a shared `SettingsDocument` base and Marten-backed `ISettingsStore` for small, singleton admin-configured settings records (e.g. email server settings, feature flags, cleanup schedules). Every settings type registered via `AddSettingsDocument<T>()` shares one physical Marten table instead of each type getting its own one-row table.

**Solution projects:**
- `AndreGoepel.Marten.Configuration` — the packable NuGet library
- `AndreGoepel.Marten.Configuration.IntegrationTests` — Testcontainers/Postgres-backed tests

Consumed by `AndreGoepel.Marten.Identity` and `AndreGoepel.AppFoundation`, among others.

## Tech Stack
- .NET 10, class library (no ASP.NET Core dependency)
- Marten + PostgreSQL
- xUnit v3, Testcontainers.PostgreSql

## Commands
- Build: `dotnet build`
- Test: `dotnet test` (integration tests need Docker for the Postgres container)
- Format: `csharpier format .` (run after every change)

## Git Workflow
- Branches: `feature/`, `bugfix/`, `hotfix/`
- Commits: `type: description` (feat, fix, refactor, test, docs, chore)
- **Always create a branch before making any file edits.** Never edit files on `main`.
- **Never commit without explicit user confirmation.** Ask before every commit, no exceptions.
- **Never push to `main` or `master`.** All pushes go to a feature/bugfix/hotfix branch only.
- **Never add a `Co-Authored-By` trailer to commits.** Commit messages contain only the description.
- Run tests before committing
- Releases are tag-driven: pushing a `vX.Y.Z` tag packs and publishes to nuget.org via trusted publishing (`.github/workflows/ci.yml`)

## Code Conventions

### Naming
- Settings documents: `[Name]SettingsDocument` or `[Name]Settings`, implementing `ISettingsDocument<T>` with a `DocumentId` constant/property
- Stores: `[Name]SettingsStore` when a settings type needs its own store beyond the generic `ISettingsStore`

### Quality
- Use async/await for all I/O; always pass `CancellationToken`
- Classes are `internal sealed` by default; only the public API surface (`SettingsDocument`, `ISettingsDocument<T>`, `ISettingsStore`, `SettingsStoreOptionsExtensions`, `Initialization`) is `public`
- Use bare `default` instead of `default(T)` when type is inferrable
- File-scoped namespaces

### Patterns
- No caching in `ISettingsStore` — every `LoadAsync` reads fresh from Postgres so a saved change takes effect without an application restart. Keep it that way; don't add caching without discussing it first
- `SaveAsync` sets `SettingsDocument.Id` from `T.DocumentId` itself — callers never set it manually

## Testing
- Scope: the Marten store and schema-hierarchy behavior (round-trip, shared-table mapping)
- Naming: `[Method]_[Scenario]_[ExpectedResult]`
- Files: `[Subject]Tests.cs`; class name inside stays `[Subject]Tests`
- Uses a collection fixture (`MartenFixture` + `IntegrationCollection`) that spins up one Postgres Testcontainer per test collection; reset documents between tests via `fixture.ResetAsync()`, don't rebuild the schema per test
- `InternalsVisibleTo`: use `<InternalsVisibleTo Include="AssemblyName" />` shorthand in csproj
