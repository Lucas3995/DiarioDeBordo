#pragma warning disable CA2007 // UI ViewModel: no ConfigureAwait — ObservableCollection must be modified on UI thread

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiarioDeBordo.Core.Primitivos;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Acervo.Queries;
using MediatR;
using System.Collections.ObjectModel;

namespace DiarioDeBordo.UI.ViewModels;

public sealed partial class AcervoViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly Func<CriarConteudoViewModel> _criarVmFactory;

    // Walking skeleton: hard-coded usuarioId until authentication is implemented (Phase 8)
    private static readonly Guid _usuarioIdTemporario = Guid.Parse("00000000-0000-0000-0000-000000000001");

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty))]
    private bool _isLoading;

    [ObservableProperty]
    private bool _mostrarFormularioCriar;

    [ObservableProperty]
    private CriarConteudoViewModel? _criarConteudoViewModel;

    public ObservableCollection<ConteudoResumoDto> Conteudos { get; } = [];

    public bool IsEmpty => !IsLoading && Conteudos.Count == 0;

    public AcervoViewModel(IMediator mediator, Func<CriarConteudoViewModel> criarVmFactory)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(criarVmFactory);
        _mediator = mediator;
        _criarVmFactory = criarVmFactory;
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        IsLoading = true;
        try
        {
            var resultado = await _mediator.Send(
                new ListarConteudosQuery(_usuarioIdTemporario, PaginacaoParams.Padrao));

            Conteudos.Clear();
            if (resultado.IsSuccess)
            {
                foreach (var item in resultado.Value!.Items)
                    Conteudos.Add(item);
            }
        }
        finally
        {
            IsLoading = false;
            OnPropertyChanged(nameof(IsEmpty));
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
