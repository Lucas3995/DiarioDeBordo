namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de detalhe — usado na tela de detalhe do conteúdo.</summary>
public sealed record ConteudoDetalheDto(
    Guid Id,
    string Titulo,
    string? Descricao,
    string? Anotacoes,
    decimal? Nota,
    string Formato,
    string Papel,
    DateTimeOffset CriadoEm);
