---
phase: "02"
plan: "01"
subsystem: "solution-structure"
tags: ["dotnet", "msbuild", "solution", "csproj", "build-config"]
dependency-graph:
  requires: []
  provides: ["solution-skeleton", "build-config", "banned-symbols", "test-infrastructure"]
  affects: ["all-plans-in-phase-02"]
tech-stack:
  added:
    - ".NET SDK 10.0 (adapted from net9 — SDK 10 only available on dev machine)"
    - "TargetFramework: net10.0 (existing Directory.Build.props)"
    - "Avalonia 11.2.4"
    - "SukiUI 2.1.0"
    - "CommunityToolkit.Mvvm 8.4.0"
    - "MediatR 12.4.1"
    - "EF Core 9.0.4 / Npgsql.EFCore.PostgreSQL 9.0.4"
    - "Microsoft.CodeAnalysis.BannedApiAnalyzers 4.* (upgraded from 3.11.0)"
    - "Testcontainers.PostgreSql 4.11.0 (upgraded from 3.11.0)"
    - "xunit 2.9.2 + coverlet.collector 6.0.2 (via Directory.Build.targets)"
    - "NSubstitute 5.3.0"
  patterns:
    - "Directory.Build.props for global build settings across all projects"
    - "Directory.Build.targets for test project package injection"
    - "BannedApiAnalyzers wired via AdditionalFiles in Directory.Build.props"
    - "Newtonsoft.Json 13.0.4 overrides vulnerable 12.0.3 transitive dep from SukiUI"
key-files:
  created:
    - "DiarioDeBordo.sln"
    - "Directory.Build.targets"
    - ".editorconfig"
    - "BannedSymbols.txt"
    - "global.json"
    - "installer/postgres/.gitkeep"
    - "src/DiarioDeBordo.Desktop/DiarioDeBordo.Desktop.csproj"
    - "src/DiarioDeBordo.Desktop/Program.cs"
    - "src/DiarioDeBordo.Infrastructure/DiarioDeBordo.Infrastructure.csproj"
    - "src/DiarioDeBordo.UI/DiarioDeBordo.UI.csproj"
    - "src/Modules/Module.{Acervo,Agregacao,Busca,Identidade,IntegracaoExterna,Portabilidade,Preferencias,Reproducao}/*.csproj"
    - "tests/Tests.{Contract,Domain,E2E,Integration,Performance,Security}/*.csproj"
    - "tests/Tests.{Contract,Domain,E2E,Integration,Performance,Security}/PlaceholderTest.cs"
  modified:
    - "Directory.Build.props — added Newtonsoft.Json 13.0.4 override, CA1515 suppression context"
decisions:
  - "SDK 10.0 used instead of SDK 9.0 (only SDK 10.0.104 installed) — TargetFramework net10.0 kept from committed Directory.Build.props"
  - "Testcontainers.PostgreSql bumped to 4.11.0 (3.11.0 no longer in NuGet registry)"
  - "Newtonsoft.Json 13.0.4 global override added to suppress NU1903 vulnerability from SukiUI transitive dep"
  - "CA1515 suppressed in test .csproj files (not via Directory.Build.props condition — evaluated before IsTestProject is set)"
  - "PlaceholderTest classes made public sealed (required by xUnit1000 for test discovery)"
  - "Desktop project gets Program.cs entry point (required by WinExe OutputType — CS5001 prevention)"
metrics:
  duration: "~35 min"
  completed: "2025-04-03"
  tasks: 2
  files: 41
---

# Phase 02 Plan 01: Solution Structure Summary

**One-liner:** 20-project .NET 10 modular monolith skeleton with TreatWarningsAsErrors, BannedApiAnalyzers (MD5/SHA1/DES/RC2/TripleDES/HMACMD5/FromSqlRaw blocked at compile time), xunit + coverlet for all test projects.

**Status:** ✅ Complete
**Commit:** `d4a585e`

## What Was Done

Created the complete .NET solution skeleton for DiarioDeBordo:

- **20 projects** in `DiarioDeBordo.sln` (13 src + 7 tests)
- **4 core src projects**: Core, Infrastructure, UI, Desktop
- **9 module projects**: Acervo, Agregacao, Busca, Identidade, IntegracaoExterna, Portabilidade, Preferencias, Reproducao, Shared
- **7 test projects**: Unit, Domain, Integration, E2E, Security, Performance, Contract

Build config enforces:
- `TreatWarningsAsErrors=true` + `AnalysisMode=All`
- `Nullable=enable` + `EnforceCodeStyleInBuild=true`
- BannedApiAnalyzers blocking MD5, SHA1, DES, TripleDES, RC2, HMACMD5, HMACSHA1, System.Random, FromSqlRaw at compile-time

## Acceptance Criteria

- [x] `dotnet build DiarioDeBordo.sln --configuration Release` → 0 errors, 0 warnings
- [x] 20 projects in solution (13 src + 7 tests)
- [x] `Directory.Build.props` with `TreatWarningsAsErrors=true`, `Nullable=enable`, `AnalysisMode=All`
- [x] `BannedSymbols.txt` wired up — RS0030 fires on MD5.Create() usage (smoke-tested)
- [x] `.editorconfig` present at root
- [x] `dotnet sln DiarioDeBordo.sln list` shows all 20 projects

## Deviations from Plan

### Auto-fixed Issues

**1. [Rule 1 - Bug] .NET SDK 10.0 creates .slnx instead of .sln by default**
- **Found during:** Task 1.1
- **Issue:** `dotnet new sln` with SDK 10 creates `.slnx` format; plan expects `.sln`
- **Fix:** Used `dotnet new sln --format sln` to force traditional format
- **Files modified:** `DiarioDeBordo.sln`
- **Commit:** d4a585e

**2. [Rule 1 - Bug] Testcontainers.PostgreSql 3.11.0 not in NuGet registry**
- **Found during:** Task 1.1 (first build attempt)
- **Issue:** NU1603 — 3.11.0 not available, 4.0.0+ required
- **Fix:** Updated to 4.11.0 (latest stable)
- **Files modified:** `tests/Tests.Integration/Tests.Integration.csproj`, `tests/Tests.E2E/Tests.E2E.csproj`
- **Commit:** d4a585e

**3. [Rule 2 - Security] Newtonsoft.Json 12.0.3 vulnerability from SukiUI**
- **Found during:** Task 1.1 (first build attempt)
- **Issue:** NU1903 high severity vulnerability in transitive dep from SukiUI/Avalonia
- **Fix:** Added `Newtonsoft.Json 13.0.4` global PackageReference in `Directory.Build.props` to force upgrade
- **Files modified:** `Directory.Build.props`
- **Commit:** d4a585e

**4. [Rule 1 - Bug] CA1515/xUnit1000 conflict on placeholder test classes**
- **Found during:** Task 1.1 (second build attempt)
- **Issue:** `AnalysisMode=All` enables CA1515 (make public types internal) vs xUnit1000 (test classes must be public)
- **Fix:** Suppressed CA1515 directly in each new test .csproj; made PlaceholderTest `public sealed`
- **Files modified:** All 6 new test .csproj files
- **Commit:** d4a585e

**5. [Rule 1 - Bug] Desktop project missing entry point**
- **Found during:** Task 1.1 (second build attempt)
- **Issue:** CS5001 — `OutputType=WinExe` requires a static Main method
- **Fix:** Added `Program.cs` with placeholder `Main(string[] args)` entry point
- **Files modified:** `src/DiarioDeBordo.Desktop/Program.cs`
- **Commit:** d4a585e

**6. [Rule 1 - Environment] SDK 10.0 used instead of SDK 9.0**
- **Found during:** Initial setup
- **Issue:** global.json requested SDK 9.0.0 but only 10.0.104 is installed
- **Fix:** Updated global.json to target SDK 10.0.0 with rollForward latestMinor; Directory.Build.props already had TargetFramework=net10.0 from previous committed version
- **Files modified:** `global.json`
- **Commit:** d4a585e

## Known Stubs

- `src/DiarioDeBordo.Desktop/Program.cs` — Empty Main() entry point; full Avalonia host wired in Phase 2 desktop plan
- `src/DiarioDeBordo.Infrastructure/Placeholder.cs` — No EF DbContext yet; wired in Phase 2 infrastructure plan
- `src/DiarioDeBordo.UI/Placeholder.cs` — No Avalonia views yet; wired in Phase 2 UI plan
- `src/Modules/Module.*/Placeholder.cs` — 7 placeholder modules; each implemented in respective future plans
- `tests/Tests.*/PlaceholderTest.cs` — 6 placeholder test suites; real tests added in subsequent plans

## Self-Check: PASSED
