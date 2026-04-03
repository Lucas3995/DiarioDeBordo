namespace DiarioDeBordo.Module.Acervo.DTOs;

/// <summary>DTO de resumo — usado na listagem de conteúdos (list view).</summary>
public sealed record ConteudoResumoDto(
    Guid Id,
    string Titulo,
    string Formato,
    string Papel,
    DateTimeOffset CriadoEm);
