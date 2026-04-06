using CommunityToolkit.Mvvm.Input;
using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.UI.ViewModels;

/// <summary>Representa um item de sessão na timeline do modal de detalhe (Plan 06).</summary>
public sealed class SessaoItemViewModel
{
    public Guid Id { get; }
    public string Titulo { get; }
    public DateTimeOffset CriadoEm { get; }
    public Classificacao? Classificacao { get; }
    public decimal? Nota { get; }
    public string? Anotacoes { get; }

    public bool IsGostei => Classificacao == DiarioDeBordo.Core.Enums.Classificacao.Gostei;
    public bool IsNaoGostei => Classificacao == DiarioDeBordo.Core.Enums.Classificacao.NaoGostei;

    public IAsyncRelayCommand AbrirCommand { get; }

    public SessaoItemViewModel(Guid id, string titulo, DateTimeOffset criadoEm, Classificacao? classificacao, decimal? nota, string? anotacoes, Func<Guid, Task> abrir)
    {
        Id = id;
        Titulo = titulo;
        CriadoEm = criadoEm;
        Classificacao = classificacao;
        Nota = nota;
        Anotacoes = anotacoes;
        AbrirCommand = new AsyncRelayCommand(() => abrir(id));
    }
}
