---
phase: "02"
plan: "04"
subsystem: "ci"
tags: [github-actions, ci, coverage, matrix-build, dotnet-tools]
dependency_graph:
  requires: []
  provides: [ci-pipeline, coverage-gate, dotnet-tools-manifest]
  affects: [all-source-plans]
tech_stack:
  added: [github-actions, dotnet-reportgenerator-globaltool@5.4.4]
  patterns: [matrix-build, cobertura-coverage, xmllint-coverage-gate]
key_files:
  created:
    - .github/workflows/ci.yml
    - .config/dotnet-tools.json
  modified:
    - Directory.Build.props
    - README.md
decisions:
  - "Integration and E2E tests Linux-only: GitHub-hosted Windows runners support Docker in containers but requires complex setup — deferring to match plan design"
  - "xmllint for Linux coverage gate (installed by default on ubuntu-latest), pwsh Select-Xml for Windows — both parse Cobertura XML from reportgenerator"
  - "fail-fast: false ensures both ubuntu and windows report failures independently"
  - "reportgenerator 5.4.4 (latest stable) instead of 5.3.11 from prompt — using plan-specified version for consistency"
metrics:
  duration_minutes: 8
  completed_date: "2026-04-03T03:19:41Z"
  tasks_completed: 2
  files_modified: 4
---

# Phase 02 Plan 04: CI Pipeline — GitHub Actions, Coverage Gate, Matrix Build — Summary

**One-liner:** GitHub Actions matrix build (ubuntu × windows) with .NET 9, ≥95% Cobertura coverage gate, reportgenerator tool manifest, and per-OS test scope.

## What Was Built

A complete GitHub Actions CI pipeline that runs on every push to `main`/`teste-com-gsd` and on PRs targeting `main`. The pipeline performs:

1. **Matrix build** across `ubuntu-latest` and `windows-latest` with `fail-fast: false`
2. **dotnet tool restore** from `.config/dotnet-tools.json` (reportgenerator 5.4.4)
3. **dotnet restore + build** in Release mode — `TreatWarningsAsErrors=true` enforced via `Directory.Build.props`
4. **Test execution by scope:**
   - Unit + Domain tests: both OS (no infrastructure dependencies)
   - Integration + E2E tests: Linux only (Testcontainers requires Docker)
5. **Coverage merging** via `reportgenerator` producing `Cobertura.xml` + HTML report
6. **Coverage gate** failing build if line-rate < 0.95:
   - Linux: `xmllint` + `bc` shell arithmetic
   - Windows: PowerShell `[xml]` + `[double]` comparison
7. **Artifact uploads**: coverage report (14 days) + test TRX results (7 days)
8. **Codecov** optional integration (`fail_ci_if_error: false`, needs `CODECOV_TOKEN` secret)

## Files Created/Modified

| File | Action | Description |
|------|--------|-------------|
| `.github/workflows/ci.yml` | Created | Full CI pipeline — 130 lines |
| `.config/dotnet-tools.json` | Created | dotnet-reportgenerator-globaltool 5.4.4 manifest |
| `Directory.Build.props` | Modified | Added `ExcludeByAttribute` for test projects (generated code exclusion) |
| `README.md` | Modified | Added CI badge: `Lucas3995/DiarioDeBordo/actions/workflows/ci.yml` |

## Decisions Made

1. **reportgenerator version 5.4.4** — Used plan-specified version (PLAN.md Task 4.1 specifies 5.4.4); prompt context mentioned 5.3.11 but plan takes precedence.

2. **Integration/E2E Linux-only** — Windows runners support Docker containers but require extra configuration steps not needed now. Unit + Domain tests cover Windows correctness sufficiently.

3. **`fail-fast: false`** — Both platforms must report. A Linux failure should not hide a Windows failure.

4. **Coverage gate split by OS** — `xmllint` (Linux, default-installed) vs `pwsh [xml]` (Windows, native) — both parse identical Cobertura XML from reportgenerator.

5. **`ExcludeByAttribute` scoped to test projects** — `GeneratedCodeAttribute` and `CompilerGeneratedAttribute` exclusions only make sense during coverage collection in test projects, not in the main build.

## Verification

```bash
# YAML syntax validation
python3 -c "import yaml; yaml.safe_load(open('.github/workflows/ci.yml')); print('YAML valid')"
# → YAML valid ✓

# Files exist
ls -la .github/workflows/ci.yml .config/dotnet-tools.json
# → Both present ✓
```

**Note:** `dotnet tool restore` will confirm reportgenerator 5.4.4 once executed in an environment with .NET SDK installed. The workflow itself won't run real tests until Wave 2+ source code exists.

## Deviations from Plan

### Auto-fixed Issues

None — plan executed exactly as written.

### Minor Adjustments

**1. Directory.Build.props `ExcludeByAttribute` placement** — Plan said to add a new `PropertyGroup Condition IsTestProject` with `CollectCoverage`, `CoverletOutputFormat`, and `ExcludeByAttribute`. Since `CollectCoverage` and `CoverletOutputFormat` were already in the global `PropertyGroup` (from Plan 02-01/02-02), only `ExcludeByAttribute` was added under the conditional group to avoid duplication. This is correct — the coverage settings apply globally, and the exclusion attribute is test-project-specific.

## Known Stubs

None. This plan creates infrastructure files (YAML + JSON), not application code with data flows.

## Self-Check: PASSED

- `.github/workflows/ci.yml` — FOUND ✓
- `.config/dotnet-tools.json` — FOUND ✓
- `Directory.Build.props` — FOUND (modified) ✓
- `README.md` — FOUND (modified) ✓
- Commit `76253e4` — FOUND ✓
- YAML syntax validated with `python3 yaml.safe_load` → exits 0 ✓
