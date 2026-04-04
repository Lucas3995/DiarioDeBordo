#pragma warning disable CA2007 // UI ViewModel: no ConfigureAwait — stays on UI thread for property updates

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiarioDeBordo.Module.Acervo.Commands;
using MediatR;

namespace DiarioDeBordo.UI.ViewModels;

public sealed partial class CriarConteudoViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private static readonly Guid _usuarioIdTemporario = Guid.Parse("00000000-0000-0000-0000-000000000001");

    public event EventHandler? ConteudoCriado;
    public event EventHandler? Cancelado;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SalvarCommand))]
    [NotifyPropertyChangedFor(nameof(PodeSalvar))]
    [NotifyPropertyChangedFor(nameof(TextoBotaoSalvar))]
    private string _titulo = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TextoBotaoDetalhes))]
    private bool _mostrarDetalhes;

    [ObservableProperty]
    private string? _descricao;

    [ObservableProperty]
    private string? _anotacoes;

    [ObservableProperty]
    private string? _mensagemErro;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SalvarCommand))]
    [NotifyPropertyChangedFor(nameof(PodeSalvar))]
    [NotifyPropertyChangedFor(nameof(TextoBotaoSalvar))]
    private bool _isSalvando;

    public bool PodeSalvar => !string.IsNullOrWhiteSpace(Titulo) && !IsSalvando;

    public string TextoBotaoDetalhes => MostrarDetalhes ? "Ocultar detalhes" : "Adicionar detalhes";

    public string TextoBotaoSalvar => IsSalvando ? "Adicionando…" : "Adicionar";

    public CriarConteudoViewModel(IMediator mediator)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        _mediator = mediator;
    }

    [RelayCommand]
    private void ToggleDetalhes() => MostrarDetalhes = !MostrarDetalhes;

    [RelayCommand]
    private void Cancelar() => Cancelado?.Invoke(this, EventArgs.Empty);

    [RelayCommand(CanExecute = nameof(PodeSalvar))]
    private async Task SalvarAsync()
    {
        MensagemErro = null;
        IsSalvando = true;
        try
        {
            var resultado = await _mediator.Send(
                new CriarConteudoCommand(_usuarioIdTemporario, Titulo));

            if (resultado.IsSuccess)
                ConteudoCriado?.Invoke(this, EventArgs.Empty);
            else
                MensagemErro = resultado.Error!.Mensagem;
        }
        finally
        {
            IsSalvando = false;
        }
    }
}

#pragma warning restore CA2007
