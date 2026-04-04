#pragma warning disable CA2007 // UI ViewModel: no ConfigureAwait — ObservableCollection must be modified on UI thread

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Acervo.Queries;
using DiarioDeBordo.Module.Acervo.Resources;
using DiarioDeBordo.UI.Services;
using MediatR;
using System.Collections.ObjectModel;

namespace DiarioDeBordo.UI.ViewModels;

public sealed partial class AcervoViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly Func<CriarConteudoViewModel> _criarVmFactory;
    private readonly IDialogService _dialogService;

    // Walking skeleton: hard-coded usuarioId until authentication is implemented (Phase 8)
    private static readonly Guid _usuarioIdTemporario = Guid.Parse("00000000-0000-0000-0000-000000000001");

    // Pagination state
    private int _totalConteudos;
    private int _paginaAtual = 1;
    private const int ItensPorPagina = 20;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isLoading;

    [ObservableProperty]
    private bool _mostrarFormularioCriar;

    [ObservableProperty]
    private CriarConteudoViewModel? _criarConteudoViewModel;

    public ObservableCollection<ConteudoCardViewModel> Conteudos { get; } = [];

    public bool IsEmpty => !IsLoading && Conteudos.Count == 0;

    public bool TemPaginaAnterior => _paginaAtual > 1;

    public bool TemProximaPagina => _paginaAtual * ItensPorPagina < _totalConteudos;

    public string StatusText => IsLoading
        ? "Carregando…"
        : $"{_totalConteudos} conteúdo(s) — Página {_paginaAtual}";

    public AcervoViewModel(IMediator mediator, Func<CriarConteudoViewModel> criarVmFactory, IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(criarVmFactory);
        ArgumentNullException.ThrowIfNull(dialogService);
        _mediator = mediator;
        _criarVmFactory = criarVmFactory;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        IsLoading = true;
        try
        {
            var paginacao = new PaginacaoParams(_paginaAtual, ItensPorPagina);
            var resultado = await _mediator.Send(
                new ListarConteudosQuery(_usuarioIdTemporario, paginacao));

            Conteudos.Clear();
            if (resultado.IsSuccess)
            {
                _totalConteudos = resultado.Value!.TotalItems;
                foreach (var item in resultado.Value!.Items)
                    Conteudos.Add(new ConteudoCardViewModel(
                        item.Id, item.Titulo, item.Formato, item.Subtipo,
                        item.Nota, item.Classificacao,
                        AbrirDetalheInternalAsync,
                        ExcluirConteudoInternalAsync));
            }
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsEmpty));
            OnPropertyChanged(nameof(TemPaginaAnterior));
            OnPropertyChanged(nameof(TemProximaPagina));
            OnPropertyChanged(nameof(StatusText));
        }
    }

    [RelayCommand]
    private async Task PaginaAnteriorAsync()
    {
        if (_paginaAtual > 1)
        {
            _paginaAtual--;
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task ProximaPaginaAsync()
    {
        if (TemProximaPagina)
        {
            _paginaAtual++;
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private async Task AbrirDetalheAsync(Guid conteudoId) =>
        await AbrirDetalheInternalAsync(conteudoId);

    private async Task AbrirDetalheInternalAsync(Guid conteudoId)
    {
        var resultado = await _dialogService.MostrarConteudoDetalheAsync(conteudoId);
        if (resultado is true || resultado is null) // modified or deleted
            await CarregarAsync();
    }

    [RelayCommand]
    private async Task ExcluirConteudoAsync(Guid conteudoId) =>
        await ExcluirConteudoInternalAsync(conteudoId);

    private async Task ExcluirConteudoInternalAsync(Guid conteudoId)
    {
        var conteudo = Conteudos.FirstOrDefault(c => c.Id == conteudoId);
        var titulo = conteudo?.Titulo ?? "este conteúdo";

        var confirmado = await _dialogService.MostrarConfirmacaoAsync(
            Strings.Dialog_ExcluirConteudo_Titulo,
            $"{Strings.Dialog_ExcluirConteudo_Mensagem} ({titulo})",
            Strings.Modal_BotaoExcluir,
            Strings.Label_Cancelar,
            isPrimarioDestructivo: true);

        if (!confirmado)
            return;

        var result = await _mediator.Send(new ExcluirConteudoCommand(conteudoId, _usuarioIdTemporario));
        if (result.IsSuccess)
        {
            _paginaAtual = 1;
            await CarregarAsync();
        }
    }

    [RelayCommand]
    private void AbrirFormularioCriar()
    {
        var vm = _criarVmFactory();
        vm.ConteudoCriado += OnConteudoCriadoAsync;
        vm.Cancelado += OnFormularioCancelado;
        CriarConteudoViewModel = vm;
        MostrarFormularioCriar = true;
    }

    [RelayCommand]
    private void FecharFormularioCriar()
    {
        MostrarFormularioCriar = false;
        if (CriarConteudoViewModel is not null)
        {
            CriarConteudoViewModel.ConteudoCriado -= OnConteudoCriadoAsync;
            CriarConteudoViewModel.Cancelado -= OnFormularioCancelado;
        }

        CriarConteudoViewModel = null;
    }

    private async void OnConteudoCriadoAsync(object? sender, EventArgs e)
    {
        FecharFormularioCriar();
        await CarregarAsync();
    }

    private void OnFormularioCancelado(object? sender, EventArgs e) => FecharFormularioCriar();
}

#pragma warning restore CA2007
