using CommunityToolkit.Mvvm.Input;

namespace DiarioDeBordo.UI.ViewModels;

/// <summary>Representa um item de relação na lista do modal de detalhe (Plan 05).</summary>
public sealed class RelacaoItemViewModel
{
    public Guid Id { get; }
    public string NomeTipo { get; }
    public string TituloDestino { get; }
    public bool IsInversa { get; }

    public IAsyncRelayCommand RemoverCommand { get; }

    public RelacaoItemViewModel(Guid id, string nomeTipo, string tituloDestino, bool isInversa, Func<Guid, Task> remover)
    {
        Id = id;
        NomeTipo = nomeTipo;
        TituloDestino = tituloDestino;
        IsInversa = isInversa;
        RemoverCommand = new AsyncRelayCommand(() => remover(id));
    }
}
