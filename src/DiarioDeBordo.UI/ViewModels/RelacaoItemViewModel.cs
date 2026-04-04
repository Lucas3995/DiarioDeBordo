namespace DiarioDeBordo.UI.ViewModels;

/// <summary>Representa um item de relação na lista do modal de detalhe (Plan 05).</summary>
public sealed class RelacaoItemViewModel
{
    public Guid Id { get; }
    public string NomeTipo { get; }
    public string TituloDestino { get; }
    public bool IsInversa { get; }

    public RelacaoItemViewModel(Guid id, string nomeTipo, string tituloDestino, bool isInversa)
    {
        Id = id;
        NomeTipo = nomeTipo;
        TituloDestino = tituloDestino;
        IsInversa = isInversa;
    }
}
