# Documentação — DiarioDeBordo

Este diretório contém a documentação de design e arquitetura do sistema.

## Estrutura

| Diretório | Conteúdo |
|---|---|
| `domain/` | Modelos táticos DDD por bounded context |
| `adr/` | Architecture Decision Records |
| `threat-model/` | Modelo de ameaças STRIDE com DFDs |

## Convenção de nomenclatura

- Documentos de bounded context: `domain/{nome-do-bc}.md` (BCs core) ou `domain/{nome-do-bc}-esboco.md` (BCs de suporte)
- ADRs: `adr/ADR-{NNN}-{titulo-kebab-case}.md`
- Threat model: `threat-model/overview.md`, `threat-model/dfd-nivel-1.md`, `threat-model/stride-table.md`
