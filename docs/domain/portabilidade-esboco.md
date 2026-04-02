# BC Portabilidade — Esboço

**Classificação:** Suporte
**Projeto .NET:** `DiarioDeBordo.Module.Portabilidade`
**Status de modelagem:** Esboço — modelagem tática completa será feita na Phase de implementação do BC Portabilidade

## Responsabilidade

Orquestrar exportação e importação de dados do usuário entre instâncias do sistema. Não tem dados próprios — agrega dados de todos os módulos exportáveis via a interface `IDadosExportaveisProvider`. O arquivo exportado é legível por humanos e agnóstico de plataforma.

## O que este BC NÃO faz

- Não armazena dados — apenas lê de outros módulos e serializa
- Não sincroniza dados em tempo real entre instâncias
- Não faz backup automático ou agendado
- Não decide quais dados são exportáveis — cada módulo decide o que expõe via `IDadosExportaveisProvider`
- Não mantém histórico de exportações anteriores

## Interfaces consumidas (definidas em Core, implementadas em outros BCs)

```csharp
// Implementada em Module.Acervo, Module.Preferencias, e qualquer módulo com dados exportáveis:
public interface IDadosExportaveisProvider
{
    string NomeModulo { get; }
    Task<ExportacaoDto> ExportarAsync(Guid usuarioId, CancellationToken ct);
}

public sealed record ExportacaoDto(
    string NomeModulo,
    DateTimeOffset ExportadoEm,
    object Dados // serializado pelo módulo exportador
);
```

## Interfaces publicadas (definidas em Core, implementadas aqui)

```csharp
// Consumida pela UI para iniciar exportação/importação:
public interface IPortabilidadeService
{
    Task<Result<string>> ExportarAsync(Guid usuarioId, CancellationToken ct);
    Task<Result<ResumoImportacao>> ImportarAsync(Guid usuarioId, string dadosJson, CancellationToken ct);
}

public sealed record ResumoImportacao(
    int RegistrosImportados,
    int RegistrosIgnorados,
    IReadOnlyList<string> Avisos
);
```

## Eventos de domínio relevantes

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| `ExportacaoConcluida(UsuarioId, TamanhoBytes)` | Module.Portabilidade | UI (log de auditoria) | Quando exportação completa com sucesso |
| `ImportacaoConcluida(UsuarioId, ResumoImportacao)` | Module.Portabilidade | UI (notificação ao usuário) | Quando importação completa com sucesso |

## O que é adiado para a fase de implementação

- Formato concreto de exportação (JSON estruturado vs. outro formato legível por humanos)
- Estratégia de resolução de conflitos na importação (ex: conteúdo já existe com ID diferente)
- Checksum de integridade do arquivo exportado (verificação na importação)
- Versioning do formato de exportação (migração entre versões)
- Interface de usuário para seleção de módulos a exportar
- Exportação parcial (ex: exportar apenas conteúdos de uma categoria específica)
