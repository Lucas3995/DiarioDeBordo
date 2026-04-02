# BC Integração Externa — Esboço

**Classificação:** Suporte
**Projeto .NET:** `DiarioDeBordo.Module.IntegracaoExterna`
**Status de modelagem:** Esboço — modelagem tática completa será feita na Phase de implementação do BC Integração Externa

## Responsabilidade

Implementar adaptadores de plataformas externas (RSS, YouTube, entre outros) que buscam itens de feed e metadados de conteúdos externos. Cada plataforma tem seu próprio adaptador que implementa `IAdaptadorPlataforma`. Este BC é o único ponto onde o sistema faz requisições HTTP de saída para fontes externas.

## O que este BC NÃO faz

- Não persiste dados — apenas traduz e repassa informações externas
- Não decide o que mostrar ao usuário — apenas provê os dados brutos ao BC Agregação
- Não aplica ranqueamento ou ordenação semântica
- Não gerencia subscrições ou configurações de fontes (isso é responsabilidade do BC Acervo)
- Não mantém cache de dados externos (a decisão de caching pertence ao consumidor)

## Interfaces publicadas (definidas em Core, implementadas aqui)

```csharp
// Consumida por Module.Agregacao:
public interface IAdaptadorPlataforma
{
    bool SuportaPlataforma(string tipo);

    Task<Result<IReadOnlyList<ItemFeedDto>>> MontarFeedAsync(
        FonteSubscricao fonte,
        PaginacaoParams paginacao,
        CancellationToken ct);

    Task<Result<MetadadosExternosDto?>> ObterMetadadosAsync(
        string urlOuIdentificador,
        string plataforma,
        CancellationToken ct);
}

public sealed record MetadadosExternosDto(
    string? Titulo,
    string? Descricao,
    string? ThumbnailUrl,
    string? Autor);

public sealed record ItemFeedDto(
    string UrlOuIdentificador,
    string? Titulo,
    string? Descricao,
    string? Autor,
    DateTimeOffset? PublicadoEm,
    string Plataforma);
```

## Interfaces consumidas (definidas em Core, implementadas em outro BC)

Nenhuma — este BC não consome interfaces de outros BCs. Recebe dados de chamadores externos (Module.Agregacao) e faz requisições HTTP.

## Eventos de domínio relevantes

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| Nenhum evento de domínio | — | — | Este BC não publica eventos de domínio; responde sincronamente a queries |

## Restrições de segurança invioláveis

As seguintes restrições são **invariantes de segurança** — não podem ser contornadas por configuração ou por qualquer adaptador (conforme Padrões Técnicos v4, seção de segurança):

### Prevenção de XXE (XML External Entity)
- `DtdProcessing.Prohibit` em **todo** parser XML/RSS — sem exceção
- Feeds RSS/Atom são tratados como entrada não confiável

### Prevenção de SSRF (Server-Side Request Forgery)
- Rejeitar URLs que resolvam para IPs privados antes de executar a requisição
  - Bloqueados: `10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`, `127.0.0.0/8`, `::1`
- Whitelist de protocolos permitidos: `https`, `http` apenas
  - Rejeitar: `file://`, `ftp://`, `data:`, `javascript:`, e qualquer outro esquema

### Limites de payload e timeout
- Limite máximo de **5MB** por payload de resposta externa
- Timeout máximo de **10 segundos** por requisição HTTP
- Nenhuma requisição pode exceder esses limites independentemente de configuração

## O que é adiado para a fase de implementação

- Implementação concreta do adaptador RSS (parsing de Atom e RSS 2.0)
- Implementação concreta do adaptador YouTube (API v3 ou scraping)
- Metadados específicos por plataforma (ex: duração de vídeo, número de episódios)
- Tratamento de throttling e rate limits por plataforma
- Estratégia de retry com backoff exponencial
- Resolução de DNS com validação de IP privado (implementação concreta da proteção SSRF)
- Adaptadores para outras plataformas (Instagram, Amazon, Spotify, etc.)
