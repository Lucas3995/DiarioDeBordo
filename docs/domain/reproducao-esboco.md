# BC Reprodução — Esboço

**Classificação:** Suporte
**Projeto .NET:** `DiarioDeBordo.Module.Reproducao`
**Status de modelagem:** Esboço — modelagem tática completa será feita na Phase de implementação do BC Reprodução

## Responsabilidade

Consumir conteúdo internamente (texto puro, Markdown, HTML, áudio local, vídeo embed YouTube) e oferecer marcação de progresso ao usuário. Não armazena dados próprios — lê o conteúdo e suas fontes via interface readonly provida pelo BC Acervo.

## O que este BC NÃO faz

- Não armazena cópias de conteúdo (sem cache, sem download)
- Não faz download ou streaming de vídeos de plataformas externas
- Não substitui players dedicados (VLC, Spotify, navegador) para abertura externa
- Não persiste dados de progresso diretamente — apenas publica eventos que o BC Acervo consome
- Não gerencia coletâneas, categorias ou relações entre conteúdos

## Interfaces consumidas (definidas em Core, implementadas em outro BC)

```csharp
// Implementada em Module.Acervo:
public interface IConteudoParaReproducaoProvider
{
    Task<ConteudoReproducaoDto?> ObterAsync(Guid id, Guid usuarioId, CancellationToken ct);
}

public sealed record ConteudoReproducaoDto(
    Guid Id,
    string Titulo,
    FormatoMidia Formato,
    IReadOnlyList<FonteOrdenada> FontesOrdenadas,
    IReadOnlyList<GanchoDto> Ganchos);
```

## Interfaces publicadas (definidas em Core, implementadas aqui)

Nenhuma — o BC Reprodução não implementa interfaces para outros BCs. Ele consome serviços e publica notificações via MediatR.

## Eventos de domínio relevantes

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| `ReproducaoIniciadaNotification(ConteudoId, Posicao)` | Module.Reproducao | ViewModels de progresso | Quando o usuário inicia a reprodução de um conteúdo |
| `ReproducaoConcluidaNotification(ConteudoId)` | Module.Reproducao | BC Acervo (via handler) | Quando o conteúdo chega ao fim |
| `PosicaoAtualizada(ConteudoId, Posicao)` | Module.Reproducao | BC Acervo (via handler) | Periodicamente durante reprodução (throttled) |

> **Nota:** Esses eventos disparam `PersistirProgressoCommand` que vai ao BC Acervo via MediatR.
> Module.Reproducao **não escreve diretamente no banco de dados**.

## O que é adiado para a fase de implementação

- Implementação concreta dos renderers: Markdown, HTML, texto puro
- Player de áudio local (WaveformDisplay, controles de velocidade)
- Embed de vídeo YouTube (WebView ou player nativo)
- Lógica de fallback automático entre fontes (tentar próxima fonte quando atual falha)
- Lógica de ganchos automáticos (navegar para gancho, criar gancho em posição atual)
- Configuração de comportamento padrão por subtipo de formato (ex: texto → abrir interno; vídeo externo → abrir no browser)
- Tela de configuração do reprodutor por usuário
