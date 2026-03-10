namespace DiarioDeBordo.Application.Obras;

/// <summary>
/// Mensagens de validação compartilhadas entre validators do bounded context Obras.
/// </summary>
public static class ObraValidationMessages
{
    public const string IdOuNomeObrigatorio = "Informe o Id ou o Nome da obra.";
    public const string PosicaoNaoPodeSerNegativa = "A posição não pode ser negativa.";
    public const string NomeObrigatorioParaCriar = "Para criar nova obra, o nome é obrigatório.";
    public const string TipoObrigatorioParaCriar = "Para criar nova obra, o tipo é obrigatório.";
    public const string OrdemObrigatoriaParaCriar = "Para criar nova obra, a ordem de preferência é obrigatória e não pode ser negativa.";
}
