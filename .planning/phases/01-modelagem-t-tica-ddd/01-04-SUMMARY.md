---
phase: 01
plan: 04
title: "Esboço dos BCs de Suporte e Contratos de Integração"
subsystem: domain-modeling
tags: [ddd, bounded-contexts, sketch, interfaces, security]
dependency_graph:
  requires: ["01-02", "01-03"]
  provides: ["docs/domain/reproducao-esboco.md", "docs/domain/integracao-externa-esboco.md", "docs/domain/busca-esboco.md", "docs/domain/portabilidade-esboco.md", "docs/domain/identidade-esboco.md", "docs/domain/preferencias-esboco.md"]
  affects: ["Phase 2 walking skeleton", "Module.Identidade stub", "Module.Preferencias stub"]
tech_stack:
  added: []
  patterns: ["IAdaptadorPlataforma ACL pattern", "IDadosExportaveisProvider aggregation pattern", "Result<T> return type convention", "PaginacaoParams transversal contract"]
key_files:
  created:
    - docs/domain/reproducao-esboco.md
    - docs/domain/integracao-externa-esboco.md
    - docs/domain/busca-esboco.md
    - docs/domain/portabilidade-esboco.md
    - docs/domain/identidade-esboco.md
    - docs/domain/preferencias-esboco.md
  modified: []
decisions:
  - "Admin area invariant enforced at ALL layers (service + ViewModel + UI) — not just UI visibility"
  - "Argon2id as password hashing algorithm (ADR-005 reference)"
  - "DtdProcessing.Prohibit for all XML/RSS parsing — no exceptions"
  - "SSRF prevention: reject private IPs before executing HTTP request"
  - "5MB payload limit + 10s timeout on all external requests"
  - "Result<T> and PaginacaoParams defined as DiarioDeBordo.Core transversal contracts"
  - "Walking skeleton (Phase 2) requires only IUsuarioAutenticadoProvider stub + IConteudoRepository real + IPreferenciasProvider stub"
metrics:
  duration_minutes: 40
  completed_date: "2026-04-02"
  tasks_completed: 2
  files_created: 6
  files_modified: 0
---

# Phase 01 Plan 04: Esboço dos BCs de Suporte e Contratos de Integração — Summary

**One-liner:** 6 sketch files for support BCs with C# interface signatures, SSRF/XXE/admin-invisibility security invariants, and Phase 2 walking skeleton interface table.

## What Was Built

Seis documentos de esboço para os BCs de suporte do sistema, seguindo a decisão D-03 (BCs de suporte recebem esboço nesta fase — modelagem tática completa adiada). Cada esboço documenta: responsabilidade, escopo negativo explícito, interfaces com assinaturas C# completas, eventos de domínio relevantes, e o que é adiado.

### Arquivos criados

| Arquivo | BC | Interfaces-chave |
|---|---|---|
| `reproducao-esboco.md` | Reprodução (Suporte) | `IConteudoParaReproducaoProvider`, eventos `ReproducaoIniciadaNotification`/`Concluida` |
| `integracao-externa-esboco.md` | Integração Externa (Suporte) | `IAdaptadorPlataforma`, restrições SSRF/XXE/5MB |
| `busca-esboco.md` | Busca (Suporte) | `IBuscaConteudoService`, `FiltroBusca`, `ExecutarOperacaoEmLoteAsync`, contratos transversais |
| `portabilidade-esboco.md` | Portabilidade (Suporte) | `IDadosExportaveisProvider`, `IPortabilidadeService` |
| `identidade-esboco.md` | Identidade (Genérico) | `IUsuarioAutenticadoProvider`, `Role`, invariante admin invisível |
| `preferencias-esboco.md` | Preferências (Genérico) | `IPreferenciasProvider`, `ConfiguracaoUsoSaudavel`, tabela walking skeleton |

### Destaques de segurança documentados

**BC Integração Externa:**
- `DtdProcessing.Prohibit` em todo XML/RSS (prevenção de XXE)
- SSRF prevention: rejeitar IPs privados antes de executar requisição HTTP
- Limite de 5MB por payload + timeout de 10s — invioláveis

**BC Identidade:**
- Área admin é invisível em TODAS as camadas: UI não renderiza o link, ViewModels não registram rotas admin, camada de serviço recusa commands admin sem revelar que existem
- Acesso não autorizado retorna "Página não encontrada" — nunca "Acesso negado" (não revela existência da área)
- Argon2id para hash de senhas (parâmetros mínimos: 64MB memória, 3 iterações)

### Contratos transversais documentados em `busca-esboco.md`

`PaginacaoParams` e `Result<T>` definidos em `DiarioDeBordo.Core` — aparecem em todos os módulos como padrão de retorno de queries e commands.

### Tabela de interfaces para walking skeleton em `preferencias-esboco.md`

Documenta quais interfaces precisam de stub vs. implementação real na Phase 2:
- `IUsuarioAutenticadoProvider` → stub (usuário hardcoded)
- `IConteudoRepository` → implementação real (PostgreSQL)
- `IPreferenciasProvider` → stub (defaults hardcoded)
- Demais interfaces → não necessárias na Phase 2

## Tasks Completed

| Task | Title | Commit | Files |
|---|---|---|---|
| 4.1 | Esboços Reprodução, Integração Externa, Busca | `a152f00` | reproducao-esboco.md, integracao-externa-esboco.md, busca-esboco.md |
| 4.2 | Esboços Portabilidade, Identidade, Preferências | `71357cf` | portabilidade-esboco.md, identidade-esboco.md, preferencias-esboco.md |

## Deviations from Plan

None — plan executed exactly as written. All acceptance criteria verified:

- ✅ 6 arquivos `*-esboco.md` em `docs/domain/`
- ✅ Todos contêm "Phase" e "adiado"
- ✅ `identidade-esboco.md` contém "comportamento genérico" (invariante admin invisível)
- ✅ `identidade-esboco.md` contém "Argon2id"
- ✅ `busca-esboco.md` contém "PaginacaoParams", "Result<T>", "IBuscaConteudoService", "FiltroBusca", "ExecutarOperacaoEmLoteAsync"
- ✅ `integracao-externa-esboco.md` contém "DtdProcessing.Prohibit", "SSRF", "5MB", "IAdaptadorPlataforma"
- ✅ `preferencias-esboco.md` contém tabela "## Interfaces obrigatórias para o Walking Skeleton"

## Known Stubs

None — this is a pure documentation phase. No production code was written.

## Self-Check: PASSED

All 6 files verified to exist. Commits `a152f00` and `71357cf` confirmed in git log.
