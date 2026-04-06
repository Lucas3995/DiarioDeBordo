---
phase: 01-modelagem-t-tica-ddd
plan: "01"
subsystem: documentation
tags: [adr, architecture, avalonia, postgresql, mediatr, argon2id, dpapi, velopack, dotnet9]

# Dependency graph
requires: []
provides:
  - "docs/ directory structure with domain/, adr/, and threat-model/ subdirectories"
  - "ADR-001: Avalonia UI + SukiUI cross-platform UI framework decision"
  - "ADR-002: PostgreSQL bundled porta 15432 com Secure Storage para credenciais"
  - "ADR-003: Monolito modular com bounded contexts via MediatR e DiarioDeBordo.Core"
  - "ADR-004: C#/.NET 9 LTS + MediatR + EF Core + Velopack stack tecnológica"
  - "ADR-005: Argon2id + DPAPI/libsecret + BannedSymbols abordagem de segurança"
affects:
  - 02-walking-skeleton
  - todos os contextos subsequentes (ADRs são a referência arquitetural para toda implementação)

# Tech tracking
tech-stack:
  added: []
  patterns:
    - "ADR format: contexto, decisão, consequências (positivas + trade-offs), alternativas consideradas"
    - "docs/adr/ como repositório de decisões arquiteturais rastreáveis"
    - "Citação explícita dos Padrões Técnicos v4 em cada ADR"

key-files:
  created:
    - "docs/README.md"
    - "docs/domain/.gitkeep"
    - "docs/adr/.gitkeep"
    - "docs/threat-model/.gitkeep"
    - "docs/adr/ADR-001-ui-framework.md"
    - "docs/adr/ADR-002-banco-de-dados.md"
    - "docs/adr/ADR-003-arquitetura.md"
    - "docs/adr/ADR-004-stack-tecnologica.md"
    - "docs/adr/ADR-005-seguranca.md"
  modified: []

key-decisions:
  - "docs/ como localização de toda documentação de design e arquitetura (D-05 do CONTEXT.md)"
  - "Cinco ADRs cobrem as decisões críticas: UI (Avalonia+SukiUI), banco (PostgreSQL bundled 15432), arquitetura (monolito modular), stack (C#/.NET 9+MediatR+EF Core+Velopack), segurança (Argon2id+DPAPI/libsecret+BannedSymbols)"
  - "Cada ADR cita explicitamente a seção relevante dos Padrões Técnicos v4 para rastreabilidade"

patterns-established:
  - "ADR: template com Data/Status/Contexto/Decisão/Consequências (positivas + trade-offs)/Alternativas Consideradas"
  - "Nomenclatura ADR: adr/ADR-{NNN}-{titulo-kebab-case}.md"

requirements-completed: [SEG-07, ARQ-01]

# Metrics
duration: 41min
completed: 2026-04-02
---

# Phase 01 Plan 01: Estrutura docs/ e ADRs — Summary

**Cinco ADRs com status Aceito formalizando as decisões arquiteturais do DiarioDeBordo: UI (Avalonia UI + SukiUI), banco (PostgreSQL bundled porta 15432 + Secure Storage), arquitetura (monolito modular + MediatR), stack (C#/.NET 9 LTS + Velopack) e segurança (Argon2id + DPAPI/libsecret + BannedSymbols)**

## Performance

- **Duration:** ~41 min
- **Started:** 2026-04-02T21:01:35Z
- **Completed:** 2026-04-02T21:42:01Z
- **Tasks:** 2
- **Files modified:** 9 (4 estrutura + 5 ADRs)

## Accomplishments

- Estrutura de diretórios `docs/` criada com `domain/`, `adr/` e `threat-model/` rastreados no git via `.gitkeep`
- `docs/README.md` com convenções de nomenclatura para bounded contexts, ADRs e threat model
- 5 ADRs completos cobrindo todas as decisões arquiteturais críticas, cada um citando a seção relevante dos Padrões Técnicos v4
- Padrão ADR estabelecido: contexto + forças em tensão, decisão, consequências positivas e trade-offs, alternativas consideradas com justificativas

## Task Commits

Cada task foi commitada atomicamente:

1. **Task 1.1: Criar estrutura de diretórios docs/** - `9731e10` (chore)
2. **Task 1.2: Criar ADR-001 a ADR-005** - `f3f0048` (docs)

## Files Created/Modified

- `docs/README.md` — Convenções de nomenclatura e estrutura de diretórios da documentação
- `docs/domain/.gitkeep` — Rastreamento do diretório de modelos táticos DDD
- `docs/adr/.gitkeep` — Rastreamento do diretório de ADRs
- `docs/threat-model/.gitkeep` — Rastreamento do diretório de threat model
- `docs/adr/ADR-001-ui-framework.md` — Decisão: Avalonia UI + SukiUI (vs MAUI, Electron, WPF)
- `docs/adr/ADR-002-banco-de-dados.md` — Decisão: PostgreSQL bundled porta 15432 + Secure Storage (vs SQLite, SQL Server)
- `docs/adr/ADR-003-arquitetura.md` — Decisão: Monolito Modular + DiarioDeBordo.Core + MediatR (vs microserviços, monolito sem modularização)
- `docs/adr/ADR-004-stack-tecnologica.md` — Decisão: C#/.NET 9 LTS + MediatR + EF Core + Velopack (vs Dapper, NServiceBus, NSIS)
- `docs/adr/ADR-005-seguranca.md` — Decisão: Argon2id + DPAPI/libsecret + BannedSymbols + ZeroMemory (vs bcrypt, config files encriptados, sem análise estática)

## Decisions Made

- Adotado template ADR com seções: Data/Status/Contexto/Decisão/Consequências (positivas + trade-offs)/Alternativas Consideradas
- Cada ADR inclui "forças em tensão" no Contexto para tornar explícito o problema de design
- Cada ADR cita explicitamente a seção dos Padrões Técnicos v4 como fundamentação — rastreabilidade direta entre decisão e documentação técnica
- Numeração sequencial ADR-001 a ADR-005 para as cinco decisões arquiteturais críticas identificadas no CONTEXT.md

## Deviations from Plan

Nenhuma — plano executado exatamente como especificado.

## Issues Encountered

Nenhum problema encontrado. Os planos 01-02, 01-03, 01-04 e 01-05 foram executados anteriormente (fora de ordem), o que resultou na existência prévia do diretório `docs/` e alguns arquivos de domínio. A execução do 01-01 criou corretamente as estruturas que deveriam existir antes dos demais planos, sem conflito.

## User Setup Required

Nenhum — documentação pura, sem configuração externa necessária.

## Next Phase Readiness

- Estrutura `docs/` estabelecida e rastreada no git — pronta para receber artefatos de fases subsequentes
- ADRs disponíveis como referência arquitetural para toda a implementação (Phase 02 — walking skeleton e além)
- Requisitos SEG-07 e ARQ-01 satisfeitos: ADRs existem em `docs/adr/` para todas as decisões arquiteturais relevantes

---
*Phase: 01-modelagem-t-tica-ddd*
*Completed: 2026-04-02*
