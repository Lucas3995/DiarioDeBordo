# Phase 1: Modelagem Tática DDD - Context

**Gathered:** 2026-04-02
**Status:** Ready for planning

<domain>
## Phase Boundary

Traduzir o design estratégico (Definição de Domínio v3, Mapa de Contexto v1) em design tático documentado e sem ambiguidades que bloqueariam a implementação — sem escrever código de produção.

A fase entrega:
- Modelo tático completo dos BCs core (Acervo + Agregação): agregados, entidades, value objects, eventos de domínio e repositórios identificados e documentados
- Esboço dos BCs de suporte (Reprodução, Integração Externa, Busca, Portabilidade, Identidade, Preferências): responsabilidades, contratos de interface e eventos de domínio relevantes
- Todos os cenários do Apêndice A da Definição de Domínio v3 percorridos no modelo tático sem ambiguidade
- Interfaces entre contextos (especialmente Acervo ↔ Agregação) com contratos claros e implementáveis independentemente
- Threat model STRIDE formal completo como referência para pentest futuro
- ADRs das decisões arquiteturais mais relevantes (monolito modular, bounded contexts, stack, segurança)

</domain>

<decisions>
## Implementation Decisions

### Formato dos Artefatos Táticos
- **D-01:** Usar combinação de **Mermaid para diagramas** (diagramas de classe, relacionamentos, fluxos de domínio) + **Markdown estruturado para descrições** (atributos, invariantes, regras de negócio, responsabilidades). Cada bounded context tem seu próprio documento com ambas as camadas.

### Escopo da Modelagem
- **D-02:** **BCs core (Acervo + Agregação) com profundidade total** — agregados, entidades, value objects, eventos de domínio, repositórios, invariantes e contratos de interface detalhados.
- **D-03:** **BCs de suporte com esboço** — Reprodução, Integração Externa, Busca, Portabilidade, Identidade, Preferências recebem documentação de responsabilidades, contratos de entrada/saída e eventos de domínio relevantes. Modelo tático completo é adiado para as fases em que cada BC entra em produção.

### Threat Model
- **D-04:** **STRIDE formal completo** — DFD de nível 0 e 1 das camadas críticas (banco de dados, rede/HTTP, processo desktop, secure storage, sistema de arquivos), tabela STRIDE sistemática por componente, mitigações listadas e rastreáveis. Funciona como referência base para o pentest full scope por milestone.

### Localização dos Artefatos
- **D-05:** Documentação tática vive em **`docs/`** na raiz do repositório. ADRs em **`docs/adr/`** (conforme já estabelecido nos Padrões Técnicos v4). Threat model em **`docs/threat-model/`** (conforme já estabelecido). Estrutura de diretórios criada nesta fase mesmo antes de qualquer código.

### Claude's Discretion
- Nomenclatura dos arquivos de documentação dentro de `docs/` (ex: `docs/domain/acervo.md` vs `docs/bounded-contexts/acervo.md`)
- Nível de detalhe dos diagramas Mermaid (diagrama de classe por agregado vs. diagrama consolidado por BC)
- Template interno dos ADRs (desde que cubra: contexto, decisão, consequências, alternativas consideradas)
- Numeração dos ADRs (ADR-001 e ADR-002 já existem como decisões documentadas no PROJECT.md — consolidar ou criar novos arquivos)

</decisions>

<canonical_refs>
## Canonical References

**Downstream agents MUST read these before planning or implementing.**

### Especificações de Domínio
- `especificacoes/1  - definicao-de-dominio.md` — Regras de negócio, modelo de domínio, invariantes, Apêndice A (cenários de validação). OBRIGATÓRIO para modelagem tática.
- `especificacoes/2 - mapa-de-dominio.md` — Classificação de subdomínios (principal, suporte, genérico), preocupação transversal de Uso Saudável.
- `especificacoes/3 - mapa-de-contexto.md` — Bounded contexts, linguagem ubíqua por contexto, padrões de relacionamento, anti-corruption layers.

### Padrões de Implementação
- `especificacoes/4 - plano-de-implementacao.md` — 10 etapas incrementais, walking skeleton, ordem de dependências entre fases.
- `especificacoes/5 - technical-standards.md` — Stack definitiva, arquitetura, segurança, padrões de código. Fonte da verdade técnica.

### Estado do Projeto
- `.planning/REQUIREMENTS.md` — Requisitos rastreáveis com IDs (ARQ-01, SEG-01, SEG-07 são os requisitos desta fase).
- `.planning/STATE.md` — Decisões acumuladas e contexto da sessão.

</canonical_refs>

<code_context>
## Existing Code Insights

### Reusable Assets
- Nenhum código existe ainda — projeto em fase pré-implementação.

### Established Patterns
- Estrutura de solução já definida nos Padrões Técnicos v4: `src/`, `tests/`, `installer/`, `docs/`. A modelagem tática deve respeitar essa estrutura ao definir onde cada bounded context se encaixa.
- Convenção de nomes já estabelecida: entidades em português brasileiro, termos técnicos em inglês.

### Integration Points
- Os artefatos desta fase (especialmente os contratos de interface entre BCs) são a base direta para o walking skeleton da Phase 2.

</code_context>

<specifics>
## Specific Ideas

- Os cenários do Apêndice A da Definição de Domínio v3 devem ser percorríveis explicitamente no modelo tático — não apenas compatíveis, mas com o caminho documentado (ex: "Cenário 1 → Entidade Conteúdo, Anotação contextual na relação Conteúdo-Coletânea, ...")
- O DFD do threat model deve cobrir especificamente: PostgreSQL bundled na porta 15432, DPAPI/libsecret para credenciais, comunicação in-process via MediatR, e adaptadores HTTP para fontes externas

</specifics>

<deferred>
## Deferred Ideas

- Modelagem tática completa dos BCs de suporte (Reprodução, Integração Externa, Busca, Portabilidade, Identidade, Preferências) — adiada para as fases em que cada BC entra em produção.
- Diagramas de sequência de fluxos complexos (ex: fluxo de persistência seletiva) — podem ser gerados durante o walking skeleton quando o comportamento for validado em código.

</deferred>

---

*Phase: 01-modelagem-t-tica-ddd*
*Context gathered: 2026-04-02 via discuss-phase*
