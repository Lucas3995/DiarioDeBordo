---
phase: "02"
plan: "07"
subsystem: Infrastructure
tags: [postgres, secure-storage, bootstrap, dpapi, libsecret, aes-gcm]
dependency_graph:
  requires: [02-01, 02-02]
  provides: [IArmazenamentoSeguro implementations, IPostgresBootstrap implementation, AddInfrastructureBootstrap DI extension]
  affects: [App startup sequence, 02-05 DI wiring, 02-10 integration tests]
tech_stack:
  added:
    - System.Security.Cryptography.ProtectedData 9.* (Windows DPAPI)
    - Microsoft.Extensions.DependencyInjection.Abstractions 9.0.4
    - Microsoft.Extensions.Logging.Abstractions 9.0.4
  patterns:
    - LoggerMessage source-gen delegates for zero-allocation structured logging
    - ConfigureAwait(false) throughout library code
    - AES-256-GCM with nonce+tag+ciphertext layout for Linux fallback storage
    - pg_ctl lifecycle: IsInitialized → initdb → start → TCP probe loop
key_files:
  created:
    - src/DiarioDeBordo.Infrastructure/Seguranca/ArmazenamentoSeguroWindows.cs
    - src/DiarioDeBordo.Infrastructure/Seguranca/ArmazenamentoSeguroLinux.cs
    - src/DiarioDeBordo.Infrastructure/Postgres/PostgresBootstrap.cs
    - src/DiarioDeBordo.Infrastructure/DependencyInjectionBootstrap.cs
  modified:
    - src/DiarioDeBordo.Infrastructure/DiarioDeBordo.Infrastructure.csproj
    - src/DiarioDeBordo.Infrastructure/Persistencia/Migrations/20260403043803_InitialCreate.cs
decisions:
  - "BuildConnectionStringAsync is not part of IPostgresBootstrap — it is an internal helper only; connection string assembly happens inside PostgresBootstrap"
  - "System.Security.Cryptography.ProtectedData added unconditionally (cross-platform package) — DPAPI methods throw PlatformNotSupportedException at runtime on Linux, but ArmazenamentoSeguroWindows is only registered/called on Windows"
  - "DependencyInjectionBootstrap is a separate static class from DependencyInjection.cs (Plan 02-05) to avoid merge conflicts and keep bootstrap concerns isolated"
  - "CA1416 suppressed with #pragma in DependencyInjectionBootstrap because OS-guard via OperatingSystem.IsWindows() satisfies runtime safety but Roslyn analyzer requires explicit annotation"
metrics:
  duration_minutes: 20
  completed_date: "2026-04-03T04:58:13Z"
  tasks_completed: 1
  files_created: 4
  files_modified: 2
---

# Phase 02 Plan 07: PostgreSQL Bootstrap + Secure Storage Summary

**One-liner:** pg_ctl lifecycle (initdb on first run → start → TCP probe) + DPAPI/libsecret+AES-256-GCM secure credential storage, wired via `AddInfrastructureBootstrap` DI extension.

## What Was Built

### Secure Storage — Windows (`ArmazenamentoSeguroWindows`)
- Uses `System.Security.Cryptography.ProtectedData` (DPAPI) with `DataProtectionScope.CurrentUser`
- Per-key encrypted files at `%LOCALAPPDATA%\DiarioDeBordo\secure\{chave}.dat`
- Implements `IArmazenamentoSeguro` exactly as defined in `DiarioDeBordo.Core.Infraestrutura`

### Secure Storage — Linux (`ArmazenamentoSeguroLinux`)
- Primary: `secret-tool` (GNOME Keyring / libsecret) via process invocation
- Fallback: AES-256-GCM encrypted files at `$XDG_DATA_HOME/DiarioDeBordo/secure/{chave}.enc`
- File layout: `[12-byte nonce][16-byte GCM tag][ciphertext]`
- Machine-derived key: `SHA256(machineName + userName + "DiarioDeBordo")`
- File permissions set to 600 (`UnixFileMode.UserRead | UserWrite`)

### PostgreSQL Bootstrap (`PostgresBootstrap`)
- `IsInitializedAsync()`: checks for `PG_VERSION` file in pgdata dir
- `IsRunningAsync()`: TCP probe to `127.0.0.1:15432`
- `EnsureRunningAsync(ct)`: initdb on first run → pg_ctl start → 30-second TCP probe loop
- initdb uses `--auth=scram-sha-256`, `--data-checksums`, password written to temp file then zeroed
- `postgresql.conf` appended with: port=15432, localhost binding, 128MB shared_buffers, max_connections=20
- `pg_hba.conf` overwritten with scram-sha-256 for all auth methods
- Passwords stored via `IArmazenamentoSeguro` before initdb; retrieved by `BuildConnectionStringAsync`
- Logging via `[LoggerMessage]` source-gen (CA1848 compliant, zero-allocation)

### DependencyInjectionBootstrap
- `AddInfrastructureBootstrap(IServiceCollection)` extension method
- OS-branch registration: Windows → DPAPI adapter; else → Linux adapter
- `IPostgresBootstrap` singleton registration

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] IPostgresBootstrap signature mismatch**
- **Found during:** Step 0 (interface reading)
- **Issue:** Plan described `Task EnsureRunningAsync` (void return) and `BuildConnectionStringAsync`. Actual interface has `Task<bool> EnsureRunningAsync`, `Task<bool> IsInitializedAsync()`, and `Task<bool> IsRunningAsync()` (no BuildConnectionString in interface).
- **Fix:** Implemented exact interface; kept `BuildConnectionStringAsync` as a non-interface public helper method
- **Files modified:** `Postgres/PostgresBootstrap.cs`

**2. [Rule 2 - Missing critical functionality] CA2007/CA1848/CA1031/CA1416/CA1822 violations**
- **Found during:** First build attempt (53 errors with TreatWarningsAsErrors=true)
- **Issue:** Strict `AnalysisMode=All` with `TreatWarningsAsErrors=true` enforces CA rules as errors
- **Fix:** Added `.ConfigureAwait(false)` to all awaits; replaced `_logger.Log*()` calls with `[LoggerMessage]` partial methods; made `StartAsync` static; added `#pragma CA1031` where generic catch is intentional (process failures); added `#pragma CA1416` for OS-guarded DI registration; used `int.Parse(..., CultureInfo.InvariantCulture)` for CA1305
- **Files modified:** All 4 new files

**3. [Rule 3 - Blocking] Pre-existing migration CA1062/CA1861 errors**
- **Found during:** First build attempt
- **Issue:** `20260403043803_InitialCreate.cs` (generated EF migration) had CA1062/CA1861 violations not suppressed
- **Fix:** Added `#pragma warning disable CA1062, CA1861` after the existing `#nullable disable` in the generated migration file
- **Files modified:** `Persistencia/Migrations/20260403043803_InitialCreate.cs`
- **Note:** This file is auto-generated; suppressions prevent future codegen from re-enabling errors

**4. [Rule 3 - Blocking] `ProtectedData` unavailable on Linux**
- **Found during:** First build attempt
- **Issue:** `System.Security.Cryptography.ProtectedData` was conditionally added only for `Windows_NT` in csproj, causing CS0103 on Linux builds
- **Fix:** Made package reference unconditional — the NuGet package is cross-platform; DPAPI methods throw `PlatformNotSupportedException` at runtime on non-Windows (but `ArmazenamentoSeguroWindows` is never instantiated on non-Windows)
- **Files modified:** `DiarioDeBordo.Infrastructure.csproj`

## Known Stubs

None — all interface methods are fully implemented. The bootstrap won't succeed at runtime without PostgreSQL binaries (integration tests for that are in Plan 02-10), but all code paths compile and are behaviorally complete.

## Self-Check: PASSED
