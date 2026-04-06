# Validation — Phase 1: Modelagem Tática DDD

**Phase:** 01
**Date:** 2026-04-02
**Type:** Documentation phase — validation via grep/wc/ls commands (no automated tests)

---

## Exit Criteria

All commands below must pass before Phase 1 is considered complete.
Run from the repository root.

---

### Plan 01 — Estrutura docs/ e ADRs

```bash
# Directory structure
find docs/ -type d | sort
# Expected: docs/ docs/adr/ docs/domain/ docs/threat-model/

# ADR count
ls docs/adr/*.md | wc -l
# Expected: ≥ 5

# ADR-002: PostgreSQL port 15432
grep "15432" docs/adr/ADR-002-banco-de-dados.md
# Expected: match

# ADR-005: Argon2id
grep "Argon2id" docs/adr/ADR-005-seguranca.md
# Expected: match

# docs/README.md exists
test -f docs/README.md && echo "OK"
# Expected: OK
```

---

### Plan 02 — BC Acervo

```bash
# File exists with minimum content
wc -l docs/domain/acervo.md
# Expected: ≥ 200

# 5 Appendix A scenarios (1–5) explicitly mapped
grep -c "Cenário [1-5]" docs/domain/acervo.md
# Expected: ≥ 5

# Invariants numbered (I-01 through at minimum I-08 for cycle detection)
grep "I-08" docs/domain/acervo.md
# Expected: match (cycle detection invariant)

# Cycle detection algorithm documented
grep "ciclo\|DFS\|Tarjan" docs/domain/acervo.md
# Expected: match

# Repositories with C# signatures
grep "IConteudoRepository\|IColetaneaRepository\|ICategoriaRepository" docs/domain/acervo.md
# Expected: 3 matches

# Mermaid diagram present
grep "classDiagram\|mermaid" docs/domain/acervo.md
# Expected: match

# Domain events documented
grep "ItemFeedPersistidoNotification\|ProgressoAlteradoNotification" docs/domain/acervo.md
# Expected: match

# PersistirItemFeedCommand contract
grep "PersistirItemFeedCommand" docs/domain/acervo.md
# Expected: match
```

---

### Plan 03 — BC Agregação

```bash
# File exists with minimum content
wc -l docs/domain/agregacao.md
# Expected: ≥ 100

# No own repositories (intentional absence documented)
grep -i "sem reposit\|não tem reposit\|repositórios próprios\|ausência" docs/domain/agregacao.md
# Expected: match

# Appendix A scenarios 6 and 7 mapped
grep -c "Cenário [67]" docs/domain/agregacao.md
# Expected: ≥ 2

# Core integration contracts
grep "PersistirItemFeedCommand\|ISubscricaoFontesProvider\|IConteudoRegistradoProvider" docs/domain/agregacao.md
# Expected: ≥ 3 matches

# Invariant: no automatic persistence
grep "IA-01\|persistência automática\|sem persistência automática" docs/domain/agregacao.md
# Expected: match
```

---

### Plan 04 — Esboços dos BCs de Suporte

```bash
# 6 sketch files exist
ls docs/domain/*-esboco.md | wc -l
# Expected: 6

# Admin area security invariant
grep -i "invisível\|não existe\|admin" docs/domain/identidade-esboco.md
# Expected: match

# PaginacaoParams transversal contract in busca-esboco.md
grep "PaginacaoParams" docs/domain/busca-esboco.md
# Expected: match

# Result<T> transversal contract in busca-esboco.md
grep "Result<T>\|Result<" docs/domain/busca-esboco.md
# Expected: match

# Each sketch file mentions what is deferred
for f in docs/domain/*-esboco.md; do grep -l "adiado\|Fase\|phase\|fase" "$f"; done
# Expected: 6 file paths returned
```

---

### Plan 05 — Threat Model STRIDE

```bash
# 3 threat model files exist
ls docs/threat-model/*.md | wc -l
# Expected: 3

# STRIDE table has ≥ 15 threats
grep -c "| T-" docs/threat-model/stride-table.md
# Expected: ≥ 15

# All 6 STRIDE categories covered
grep -E "Spoofing|Tampering|Repudiation|Information Disclosure|Denial of Service|Elevation" docs/threat-model/stride-table.md | wc -l
# Expected: ≥ 6

# DFD level 0 has Mermaid diagram
grep "mermaid\|flowchart\|graph" docs/threat-model/overview.md
# Expected: match

# DFD level 1 covers 5 subsystems
grep -c "Autenticação\|Banco de Dados\|Rede\|Reprodutor\|Importação" docs/threat-model/dfd-nivel-1.md
# Expected: ≥ 5

# SSRF protection documented
grep "SSRF\|IPs privados\|endereços privados" docs/threat-model/stride-table.md
# Expected: match

# Argon2id or secure storage mentioned
grep "Argon2id\|DPAPI\|libsecret" docs/threat-model/stride-table.md
# Expected: match
```

---

## Phase-Level Gate

All 5 success criteria from ROADMAP.md must be satisfied:

| SC | Verified by |
|---|---|
| SC1: BCs core with aggregates, VOs, events, repositories | Plans 02 + 03 |
| SC2: All 7 Appendix A scenarios walked through | Plans 02 (1–5) + 03 (6–7) |
| SC3: Inter-context interfaces with clear contracts | Plans 02 + 03 (PersistirItemFeedCommand et al.) |
| SC4: Threat model before network/persistence layers | Plan 05 |
| SC5: ADRs for key decisions | Plan 01 (ADR-001 to ADR-005) |

---

*Generated: 2026-04-02 — Phase 1: Modelagem Tática DDD*
