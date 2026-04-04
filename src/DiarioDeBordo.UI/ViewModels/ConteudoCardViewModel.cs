using CommunityToolkit.Mvvm.Input;
using DiarioDeBordo.Core.Enums;

namespace DiarioDeBordo.UI.ViewModels;

/// <summary>
/// Thin wrapper around ConteudoResumoDto that bundles the card's action commands.
/// Eliminates the need for cross-DataContext bindings (parent ViewModel casts) inside
/// DataTemplate deferred builders, which cannot resolve namespace aliases at runtime.
/// </summary>
[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage(
    Justification = "Pure UI wrapper — no business logic; covered by E2E tests.")]
public sealed class ConteudoCardViewModel
{
    public Guid Id { get; }
    public string Titulo { get; }
    public string Formato { get; }
    public string? Subtipo { get; }
    public decimal? Nota { get; }
    public Classificacao? Classificacao { get; }

    public IAsyncRelayCommand AbrirDetalheCommand { get; }
    public IAsyncRelayCommand ExcluirCommand { get; }

    public ConteudoCardViewModel(
        Guid id,
        string titulo,
        string formato,
        string? subtipo,
        decimal? nota,
        Classificacao? classificacao,
        Func<Guid, Task> abrirDetalhe,
        Func<Guid, Task> excluir)
    {
        Id = id;
        Titulo = titulo;
        Formato = formato;
        Subtipo = subtipo;
        Nota = nota;
        Classificacao = classificacao;
        AbrirDetalheCommand = new AsyncRelayCommand(() => abrirDetalhe(id));
        ExcluirCommand = new AsyncRelayCommand(() => excluir(id));
    }
}
