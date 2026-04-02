# ADR-004: Stack Tecnológica — C#/.NET 9 LTS + MediatR + EF Core + Velopack

**Data:** 2026-04-02
**Status:** Aceito

## Contexto

Necessidade de uma stack de implementação madura, cross-platform (Linux + Windows), com bom suporte a DDD e CQRS, e com mecanismo de distribuição e auto-update que não exija .NET pré-instalado na máquina do usuário.

Forças em tensão:
- Cross-platform obrigatório desde a Etapa 1
- Suporte de longo prazo (LTS) para uma aplicação pessoal que precisa de estabilidade
- Comunicação entre módulos desacoplada (CQRS/mediator pattern)
- ORM maduro com suporte a PostgreSQL
- Distribuição e auto-update sem dependência de runtime pré-instalado

**Referência:** Padrões Técnicos v4, seção 2 — Stack Tecnológica.

## Decisão

Adotamos C# com .NET 9 LTS. MediatR para comunicação interna entre módulos (CQRS + mediator pattern). EF Core com provider PostgreSQL (Npgsql) para persistência. Velopack para distribuição e auto-update com code signing.

## Consequências

### Positivas
- .NET 9 LTS tem suporte de longo prazo — estabilidade para aplicativo pessoal de uso contínuo
- MediatR permite desacoplamento entre módulos via Commands, Queries e Notifications sem referência direta
- EF Core + PostgreSQL via Npgsql é combinação madura com migrations versionadas
- Velopack lida com diferenças de instalação Linux/Windows — bundle do runtime, auto-update, code signing
- CommunityToolkit.Mvvm integra com Avalonia UI sem overhead de frameworks MVVM pesados
- Self-contained publish via Velopack elimina dependência de .NET runtime na máquina do usuário

### Negativas / Trade-offs
- MediatR pode esconder dependências implícitas se os handlers não forem bem organizados
- EF Core adiciona overhead de ORM — queries complexas podem exigir fallback para SQL nativo via FromSqlRaw
- Velopack é menos maduro que NSIS/Inno Setup, mas mais adequado para cross-platform com .NET

## Alternativas Consideradas

| Alternativa | Por que não adotada |
|---|---|
| Dapper | Menos abstração — todo SQL manual; sem migrations automáticas; maior esforço de manutenção |
| NServiceBus | Overkill para aplicação desktop single-user; custo de licença comercial |
| NSIS / Inno Setup | Mais manual; não tem suporte nativo a auto-update cross-platform com .NET bundle |
| .NET 8 | Sem LTS diferencial sobre o .NET 9; preferimos a versão mais recente com suporte |
