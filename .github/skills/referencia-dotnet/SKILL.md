---
name: referencia-dotnet
description: Referência de implementação para projetos .NET — padrões e features específicos da plataforma (Caching, EF Core, API Versioning). Complementa mercenario e mestre-freire com guidance stack-specific. Usar quando o projeto usa .NET/ASP.NET Core e precisa de orientação sobre caching, Entity Framework Core ou versionamento de APIs.
---

# Referência .NET — Padrões e Features da Plataforma

Referência de implementação para projetos que usam **.NET / ASP.NET Core / EF Core**. Fornece guidance sobre features e padrões específicos da plataforma.

**Nota:** Esta skill é **tech-specific** (.NET). Em projetos de outra stack, **não deve ser invocada**. Não substitui skills de workflow (análise, testes, refactoring) — apenas complementa com conhecimento da plataforma.

*Definições de termos: [Glossário Unificado](../../reference/glossario-unificado.md)*
*Fontes: Mastering Caching in ASP.NET Core (Abeysekara), 10 EF Core Secret Features (Martyniuk), ASP.NET Core API Versioning (Elliot One)*

---

## Escopo

| Área | Cobertura |
|------|-----------|
| **Caching** | In-Memory, Distributed (Redis), Response Caching |
| **EF Core** | Shadow Properties, Query Tags, Compiled Queries, DbContext Pooling, Value Converters, Temporal Tables, Database Seeding, Split Queries, Raw SQL, Multi-DB Migrations |
| **API Versioning** | Controller-based, Minimal APIs, ApiVersioningOptions |

---

## Restrições obrigatórias

- **Apenas referência:** Não substituir workflow de testes (quadro-de-recompensas), análise (batedor-de-codigos) ou refactoring (mestre-freire).
- **Não inventar features:** Consultar reference.md para confirmar que a feature existe e como funciona antes de orientar.
- **Versão mínima:** Indicar versão mínima do .NET/EF Core quando feature depende de versão específica.

---

## Quando usar (lookup rápido)

| Situação no projeto | Consultar |
|---------------------|-----------|
| Precisa cachear dados para performance | reference.md §1 Caching |
| Precisa escolher entre cache local vs distribuído | reference.md §1.4 Decision table |
| Precisa auditar mudanças sem tabela manual | reference.md §2 EF Core → Temporal Tables |
| Queries EF Core estão lentas | reference.md §2 → Compiled Queries, Split Queries, Query Tags |
| Precisa versionar API sem quebrar clientes | reference.md §3 API Versioning |
| Precisa suportar múltiplos bancos de dados | reference.md §2 → Multi-DB Migrations |

---

## Integração com outras skills

| Skill | Relação |
|-------|---------|
| **mercenario** | Ao implementar em .NET, consultar esta skill para features da plataforma que resolvem o problema |
| **mestre-freire** | Ao refatorar .NET, verificar se features nativas substituem implementações manuais |
| **batedor-de-codigos** | Pode detectar: cache manual onde IMemoryCache resolve, audit tables onde Temporal Tables resolve, etc. |
| **quadro-de-recompensas** | Testes de caching e EF Core seguem padrões próprios (in-memory provider, test fixtures) |
