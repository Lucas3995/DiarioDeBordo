---
phase: 2
phase_name: Walking Skeleton
discuss_date: 2026-04-02
decisions: [D-01, D-02, D-03, D-04]
---

# Phase 2: Walking Skeleton — Context

## Phase Goal
A arquitetura intencional está provada em código: uma funcionalidade real (criar conteúdo com título) atravessa todas as camadas de ponta a ponta, com separação verificável entre domínio, persistência e apresentação.

## Success Criteria (from ROADMAP.md)
1. Um conteúdo pode ser criado com título, salvo no banco e recuperado — de ponta a ponta — na aplicação desktop rodando em Linux e Windows
2. A separação de camadas é verificável: trocar a apresentação não exige alterar o domínio; trocar a persistência não exige alterar o domínio
3. Testes automatizados cobrem o fluxo completo e passam em CI
4. Cobertura de testes ≥ 95% está configurada e mantida desde este ponto

## Requirements
ARQ-02, SEG-02, SEG-03

---

## Decisions

### D-01 — PostgreSQL: bundling completo desde já (porta 15432)

**Decisão:** Implementar o bundling do PostgreSQL na porta 15432 desde a Fase 2 — exatamente como será em produção.

**Justificativa:** Evita divergência entre ambiente de desenvolvimento e produção. Problemas de bundling descobertos mais cedo custam menos. A porta 15432 já está documentada no ADR-002.

**Implicações para pesquisa e planejamento:**
- Pesquisar como distribuir o runtime PostgreSQL bundled com Velopack no Linux e Windows
- Implementar geração de senha na instalação e armazenamento no Secure Storage do SO (DPAPI/libsecret)
- O bootstrap do `DiarioDeBordo.Desktop` precisa iniciar o processo PostgreSQL antes de qualquer conexão
- Migrations devem rodar automaticamente na primeira execução

---

### D-02 — Estrutura completa de projetos .NET desde já

**Decisão:** Criar todos os projetos (.csproj) da solução desde a Fase 2, mesmo que a maioria dos módulos fique sem implementação.

**Estrutura a criar:**
```
Solution/
├── src/
│   ├── DiarioDeBordo.Desktop/
│   ├── DiarioDeBordo.Core/
│   ├── DiarioDeBordo.Infrastructure/
│   ├── DiarioDeBordo.UI/
│   └── Modules/
│       ├── Module.Acervo/          ← implementado nesta fase
│       ├── Module.Agregacao/       ← placeholder
│       ├── Module.Reproducao/      ← placeholder
│       ├── Module.IntegracaoExterna/ ← placeholder
│       ├── Module.Busca/           ← placeholder
│       ├── Module.Portabilidade/   ← placeholder
│       ├── Module.Identidade/      ← placeholder
│       ├── Module.Preferencias/    ← placeholder
│       └── Module.Shared/          ← implementado nesta fase (PaginatedList<T>, Result<T>)
├── tests/
│   ├── Tests.Unit/
│   ├── Tests.Integration/
│   ├── Tests.Domain/
│   ├── Tests.E2E/
│   ├── Tests.Security/
│   ├── Tests.Performance/
│   └── Tests.Contract/
├── Directory.Build.props           ← configurações obrigatórias (Nullable, TreatWarningsAsErrors, etc.)
├── .editorconfig
└── BannedSymbols.txt
```

**Módulos placeholder:** apenas o arquivo de projeto (.csproj) com as dependências corretas. Nenhuma implementação. Nenhum namespace vazio desnecessário.

**Implicações para pesquisa e planejamento:**
- Pesquisar estrutura de solução .NET 9 multi-projeto com Directory.Build.props compartilhado
- `BannedSymbols.txt` precisa estar configurado desde o primeiro build (MD5, SHA1, System.Random para fins crypto, etc.)
- Regras de dependência entre projetos devem ser codificadas nas referências do .csproj (não apenas documentadas)

---

### D-03 — UI: estrutura de navegação de estado da arte desde já

**Decisão:** Implementar a estrutura de navegação que persistirá por todas as fases futuras, fundamentada em pesquisa de UX/UI de estado da arte: acessibilidade (WCAG 2.2 AAA), storytelling voltado a sistemas, e boas práticas de interfaces desktop.

**O que isso significa para a Fase 2:**
- Não é "janela + campo + botão". É a estrutura de navegação completa — barra lateral, área de conteúdo, shell da aplicação — mesmo que a maioria das áreas mostre placeholder.
- A tela de "criar conteúdo" e "listar conteúdo" deve caber naturalmente nessa estrutura, não ser refatorada depois.

**Implicações para pesquisa e planejamento:**
- **Pesquisar:** estado da arte em UX para aplicações desktop (não mobile-first), padrões de navegação para gestão pessoal de conteúdo (ex: Calibre, Obsidian, Notion desktop, Linear), acessibilidade WCAG 2.2 AAA em Avalonia UI, storytelling em interfaces de sistema (progressive disclosure, affordance, affordance hierarchy)
- **Pesquisar:** componentes SukiUI disponíveis para navegação (SukiSideMenu, SukiTabView, etc.)
- **Pesquisar:** padrões de layout para disclosure progressivo — campo de título visível por padrão, demais campos revelados sob demanda
- O planner deve criar uma tarefa dedicada de UI design antes de qualquer implementação de tela
- WCAG 2.2 AAA deve ser verificável desde o primeiro commit de UI (contrast ratio, keyboard navigation, screen reader labels)

**O que NÃO está no escopo desta fase:**
- Temas claro/escuro completos (pode vir em Fase seguinte)
- Configuração de acessibilidade pelo usuário (Fase 10)
- Qualquer tela além de: shell de navegação, criar conteúdo, listar conteúdo

---

### D-04 — CI completo desde o primeiro commit de código

**Decisão:** GitHub Actions configurado desde o primeiro commit, com: build, testes automatizados, cobertura ≥ 95%, linters e validadores de qualidade.

**O que o CI deve verificar em cada PR/push:**
- Build da solução completa (sem warnings — TreatWarningsAsErrors=true)
- Todos os testes (unit, integration, domain)
- Cobertura ≥ 95% (bloquear PR se não atingir)
- Analyzer rules (AnalysisMode=All, EnforceCodeStyleInBuild=true)
- BannedSymbols.txt (erro de compilação, já coberto pelo build)
- .editorconfig compliance

**Plataformas:** Linux e Windows (matrix build — a aplicação precisa rodar em ambas)

**Implicações para pesquisa e planejamento:**
- Pesquisar configuração de GitHub Actions para .NET 9 com matrix Linux/Windows
- Pesquisar ferramenta de cobertura para .NET (Coverlet + ReportGenerator ou equivalente)
- O pipeline de CI deve ser um dos primeiros artefatos criados na fase

---

## Canonical References

| Arquivo | Papel nesta fase |
|---|---|
| `especificacoes/5 - technical-standards.md` | Fonte da verdade técnica: stack, segurança, padrões de código, Directory.Build.props |
| `especificacoes/1 - definicao-de-dominio.md` | Entidade Conteudo (atributos, invariantes), Apêndice A cenários 1-5 |
| `especificacoes/4 - plano-de-implementacao.md` | Escopo exato da Etapa 1 (Walking Skeleton) |
| `docs/domain/acervo.md` | Modelo tático completo do BC Acervo — agregados, invariantes, interfaces C# |
| `docs/adr/ADR-002-banco-de-dados.md` | Decisão PostgreSQL bundled porta 15432 |
| `docs/adr/ADR-001-ui-framework.md` | Decisão Avalonia UI + SukiUI |
| `.planning/ROADMAP.md` | Critérios de sucesso da Fase 2 |

---

## Notes for Researcher

- **PostgreSQL bundling** é o maior risco técnico desta fase. Pesquisar como outros projetos .NET desktop fazem (ex: pgAdmin bundla o PostgreSQL? Como o Velopack lida com processos externos?)
- **Avalonia + SukiUI navigation patterns** — quais componentes do SukiUI são adequados para uma shell de aplicação desktop com sidebar?
- **WCAG 2.2 AAA no Avalonia** — como verificar e garantir isso em testes? Existe ferramenta de auditoria de acessibilidade para Avalonia?
- **Directory.Build.props com AnalysisMode=All** — verificar se há analyzers que conflitem com o estilo do projeto ou gerem falsos positivos que precisem de exceção explícita
- **TDD desde o início** — toda implementação deve ter teste escrito antes. Pesquisar mocking para MediatR e EF Core em testes .NET 9

---

*Discuss-phase date: 2026-04-02 | Phase 2: Walking Skeleton*
