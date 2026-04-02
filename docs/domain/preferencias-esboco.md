# BC Preferências — Esboço

**Classificação:** Genérico
**Projeto .NET:** `DiarioDeBordo.Module.Preferencias`
**Status de modelagem:** Esboço — modelagem tática completa será feita na Phase de implementação do BC Preferências

## Responsabilidade

Armazenar e fornecer preferências de interface por usuário: tema visual, tipografia, acessibilidade, paginação, disclosure progressivo e configurações de uso saudável. Este BC apenas armazena e expõe preferências — **não aplica as preferências** (isso é responsabilidade de quem as consome).

## O que este BC NÃO faz

- Não aplica as preferências — apenas armazena e fornece
- Não tem preferências globais de admin que sobrescrevem as do usuário (na fase inicial)
- Não sincroniza preferências entre dispositivos (aplicação desktop local)
- Não valida se o sistema suporta a preferência escolhida (isso é responsabilidade da UI)

## Interfaces publicadas (definidas em Core, implementadas aqui)

```csharp
// Consumida por UI e por módulos que precisam respeitar preferências do usuário:
public interface IPreferenciasProvider
{
    Task<Tema> ObterTemaAsync(Guid usuarioId, CancellationToken ct);
    Task<int> ObterItensPorPaginaAsync(Guid usuarioId, CancellationToken ct);
    Task<bool> DisclosureProgressivoAtivoAsync(Guid usuarioId, CancellationToken ct);
    Task<ConfiguracaoUsoSaudavel> ObterUsoSaudavelAsync(Guid usuarioId, CancellationToken ct);
}

public enum Tema
{
    Claro,
    Escuro,
    SistemaOperacional
}

public sealed record ConfiguracaoUsoSaudavel(
    bool MonitoramentoTempoAtivo,
    TimeSpan? LimiteDiario,
    bool EscalaCinzaAtiva,
    bool LembretesAtivos
);
```

## Interfaces consumidas (definidas em Core, implementadas em outro BC)

```csharp
// Este BC implementa IDadosExportaveisProvider:
// As preferências do usuário são exportáveis via BC Portabilidade.
// Module.Preferencias implementa:
//   IDadosExportaveisProvider (NomeModulo = "Preferencias")
```

## Eventos de domínio relevantes

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| `PreferenciasAtualizadasNotification(UsuarioId)` | Module.Preferencias | UI (reaplica tema/fonte) | Quando usuário salva mudanças de preferência |

## O que é adiado para a fase de implementação

- Preferências de admin: defaults globais que se aplicam a novos usuários
- Modelo concreto de preferências de tipografia (família de fonte, tamanho, espaçamento, cor)
- Temas customizáveis pelo usuário (paletas de cores personalizadas)
- Preferências de acessibilidade avançadas (alto contraste, redução de movimento, tamanho de cursor)
- Persistência de preferências com migração de schema (versionamento)
- Interface de usuário para a tela de preferências

---

## Interfaces obrigatórias para o Walking Skeleton (Phase 2)

O walking skeleton (Phase 2) exercita apenas o BC Acervo (criar + recuperar conteúdo). As seguintes interfaces devem existir como stubs ou implementações minimais antes do início da Phase 2:

| Interface | Implementada em | Consumida por | Nível de completude para Phase 2 |
|---|---|---|---|
| `IUsuarioAutenticadoProvider` | Module.Identidade | Todos os módulos | Stub: retorna usuário fixo hardcoded |
| `IConteudoRepository` | Module.Acervo | Module.Acervo | Implementação real (PostgreSQL) |
| `IPreferenciasProvider` | Module.Preferencias | UI, Module.Acervo | Stub: retorna defaults hardcoded |
| `ISubscricaoFontesProvider` | Module.Acervo | Module.Agregacao | Não necessária na Phase 2 |
| `IAdaptadorPlataforma` | Module.IntegracaoExterna | Module.Agregacao | Não necessária na Phase 2 |
| `IDadosExportaveisProvider` | Module.Acervo, Module.Preferencias | Module.Portabilidade | Não necessária na Phase 2 |
| `IBuscaConteudoService` | Module.Busca | ViewModels | Não necessária na Phase 2 |

> **Regra:** Interfaces marcadas como "Não necessária na Phase 2" NÃO precisam de stub no walking skeleton — seus consumidores simplesmente não serão exercitados nesta fase.
