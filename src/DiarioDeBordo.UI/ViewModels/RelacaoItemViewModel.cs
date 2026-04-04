using CommunityToolkit.Mvvm.Input;

namespace DiarioDeBordo.UI.ViewModels;

/// <summary>Representa um item de relação na lista do modal de detalhe (Plan 05).</summary>
public sealed class RelacaoItemViewModel
{
    public Guid Id { get; }
    public string NomeTipo { get; }
    public string TituloDestino { get; }
    public bool IsInversa { get; }

    /// <summary>True quando a relação ainda não foi persistida — aguarda Salvar.</summary>
    public bool IsPendente { get; }

    // Dados necessários para persistir quando Salvar for clicado
    internal Guid ConteudoDestinoId { get; }
    internal string NomeTipoPersistir { get; }
    internal string NomeInversoPersistir { get; }

    public IAsyncRelayCommand RemoverCommand { get; }

    /// <summary>Construtor para relação já persistida (carregada do banco).</summary>
    public RelacaoItemViewModel(Guid id, string nomeTipo, string tituloDestino, bool isInversa, Func<Guid, Task> remover)
    {
        Id = id;
        NomeTipo = nomeTipo;
        TituloDestino = tituloDestino;
        IsInversa = isInversa;
        IsPendente = false;
        ConteudoDestinoId = Guid.Empty;
        NomeTipoPersistir = nomeTipo;
        NomeInversoPersistir = string.Empty;
        RemoverCommand = new AsyncRelayCommand(() => remover(id));
    }

    /// <summary>Construtor para relação pendente (não persistida — aguarda Salvar).</summary>
    public RelacaoItemViewModel(
        string nomeTipo,
        string nomeInverso,
        Guid conteudoDestinoId,
        string tituloDestino,
        Func<RelacaoItemViewModel, Task> removerPendente)
    {
        Id = Guid.Empty;
        NomeTipo = nomeTipo;
        TituloDestino = tituloDestino;
        IsInversa = false;
        IsPendente = true;
        ConteudoDestinoId = conteudoDestinoId;
        NomeTipoPersistir = nomeTipo;
        NomeInversoPersistir = nomeInverso;
        RemoverCommand = new AsyncRelayCommand(() => removerPendente(this));
    }
}
