#pragma warning disable CA2007 // UI ViewModel: no ConfigureAwait — ObservableCollection must be modified on UI thread

using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Module.Acervo.Commands;
using DiarioDeBordo.Module.Acervo.DTOs;
using DiarioDeBordo.Module.Acervo.Queries;
using DiarioDeBordo.Module.Acervo.Resources;
using MediatR;

namespace DiarioDeBordo.UI.ViewModels;

public sealed partial class ConteudoDetalheViewModel : ObservableObject
{
    private readonly IMediator _mediator;
    private readonly DiarioDeBordo.UI.Services.IDialogService _dialogService;
    private readonly Guid _conteudoId;

    // Walking skeleton: hard-coded usuarioId until authentication is implemented (Phase 8)
    private static readonly Guid _usuarioIdTemporario = Guid.Parse("00000000-0000-0000-0000-000000000001");

    // IDialogService needs to be the type from Services namespace - we avoid circular dependency
    // by using the interface defined in the UI project

    // Snapshot for dirty tracking
    private string _tituloOriginal = string.Empty;
    private string? _descricaoOriginal;
    private string? _anotacoesOriginal;
    private FormatoMidia _formatoOriginal;
    private string? _subtipoOriginal;

    // --- Editable properties ---

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private string _titulo = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private string? _descricao;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private string? _anotacoes;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    [NotifyPropertyChangedFor(nameof(ResumoIdentificacao))]
    private FormatoMidia _formato;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private string? _subtipo;

    // --- Display-only (from DTO, not editable in Plan 04) ---

    [ObservableProperty]
    private Classificacao? _classificacao;

    [ObservableProperty]
    private decimal? _nota;

    [ObservableProperty]
    private int _categoriaCount;

    [ObservableProperty]
    private int _relacaoCount;

    [ObservableProperty]
    private int _sessoesContagem;

    [ObservableProperty]
    private int? _totalEsperadoSessoes;

    [ObservableProperty]
    private bool _isFilho;

    // --- UI state ---

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isSalvando;

    [ObservableProperty]
    private string? _mensagemErro;

    // --- Computed ---

    public bool IsDirty =>
        Titulo != _tituloOriginal ||
        Descricao != _descricaoOriginal ||
        Anotacoes != _anotacoesOriginal ||
        Formato != _formatoOriginal ||
        Subtipo != _subtipoOriginal;

    public string TituloJanela => IsDirty
        ? Strings.Modal_Titulo_Existente + " •"
        : Strings.Modal_Titulo_Existente;

    // Expander summary properties (read-only, computed from display data)
    public string ResumoIdentificacao => $"{Formato}";

    public string ResumoAvaliacao =>
        Nota.HasValue || Classificacao.HasValue
            ? $"{(Nota.HasValue ? $"Nota: {Nota:0.#}" : string.Empty)}{(Classificacao == Core.Enums.Classificacao.Gostei ? " 👍" : Classificacao == Core.Enums.Classificacao.NaoGostei ? " 👎" : string.Empty)}".Trim()
            : Strings.Avaliacao_SemAvaliacao;

    public string ResumoOrganizacao =>
        CategoriaCount > 0 || RelacaoCount > 0
            ? $"{CategoriaCount} categorias · {RelacaoCount} relações"
            : Strings.Categorias_EstadoVazio;

    public string ResumoHistorico =>
        SessoesContagem > 0
            ? TotalEsperadoSessoes.HasValue
                ? $"{(int)((double)SessoesContagem / TotalEsperadoSessoes.Value * 100)}% — {SessoesContagem} de {TotalEsperadoSessoes.Value} sessões"
                : $"{SessoesContagem} sessão(ões) registrada(s)"
            : Strings.Progresso_Vazio;

    // Expose IDialogService interface from UI services namespace (avoid circular reference)
    private DiarioDeBordo.UI.Services.IDialogService? DialogServiceInstance => _dialogService;

    public ConteudoDetalheViewModel(IMediator mediator, Guid conteudoId, DiarioDeBordo.UI.Services.IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(mediator);
        ArgumentNullException.ThrowIfNull(dialogService);
        _mediator = mediator;
        _conteudoId = conteudoId;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task CarregarAsync()
    {
        IsLoading = true;
        MensagemErro = null;
        try
        {
            var resultado = await _mediator.Send(new ObterConteudoQuery(_conteudoId, _usuarioIdTemporario));
            if (resultado.IsSuccess && resultado.Value is ConteudoDetalheDto dto)
            {
                PopularCampos(dto);
                SnapshotOriginals();
                OnPropertyChanged(nameof(IsDirty));
                OnPropertyChanged(nameof(TituloJanela));
            }
            else
            {
                MensagemErro = Strings.Erro_FalhaAoCarregar;
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void PopularCampos(ConteudoDetalheDto dto)
    {
        Titulo = dto.Titulo;
        Descricao = dto.Descricao;
        Anotacoes = dto.Anotacoes;
        Formato = Enum.TryParse<FormatoMidia>(dto.Formato, out var fmt) ? fmt : FormatoMidia.Nenhum;
        Subtipo = dto.Subtipo;
        Classificacao = dto.Classificacao;
        Nota = dto.Nota;
        CategoriaCount = dto.Categorias.Count;
        RelacaoCount = dto.Relacoes.Count;
        SessoesContagem = dto.SessoesContagem;
        TotalEsperadoSessoes = dto.TotalEsperadoSessoes;
        IsFilho = dto.IsFilho;

        OnPropertyChanged(nameof(ResumoAvaliacao));
        OnPropertyChanged(nameof(ResumoOrganizacao));
        OnPropertyChanged(nameof(ResumoHistorico));
    }

    private void SnapshotOriginals()
    {
        _tituloOriginal = Titulo;
        _descricaoOriginal = Descricao;
        _anotacoesOriginal = Anotacoes;
        _formatoOriginal = Formato;
        _subtipoOriginal = Subtipo;
    }

    [RelayCommand]
    private async Task SalvarAsync()
    {
        if (string.IsNullOrWhiteSpace(Titulo))
        {
            MensagemErro = Strings.Erro_TituloObrigatorio;
            return;
        }

        IsSalvando = true;
        MensagemErro = null;
        try
        {
            var cmd = new AtualizarConteudoCommand(
                _conteudoId,
                _usuarioIdTemporario,
                Titulo,
                Descricao,
                Anotacoes,
                Nota,
                Classificacao,
                Formato,
                Subtipo,
                EstadoProgresso.NaoIniciado, // Will be wired in Plan 05
                null,
                TotalEsperadoSessoes,
                []);  // Categories wired in Plan 05

            var resultado = await _mediator.Send(cmd);
            if (resultado.IsSuccess)
            {
                // Signal to window: content was modified
                (Owner as Views.ConteudoDetalheWindow)?.Close(true);
            }
            else
            {
                MensagemErro = Strings.Erro_FalhaAoSalvar;
            }
        }
        finally
        {
            IsSalvando = false;
        }
    }

    [RelayCommand]
    private async Task ExcluirAsync()
    {
        if (DialogServiceInstance is null)
            return;

        var confirmado = await DialogServiceInstance.MostrarConfirmacaoAsync(
            Strings.Dialog_ExcluirConteudo_Titulo,
            Strings.Dialog_ExcluirConteudo_Mensagem,
            Strings.Modal_BotaoExcluir,
            Strings.Label_Cancelar,
            isPrimarioDestructivo: true);

        if (!confirmado)
            return;

        var resultado = await _mediator.Send(new ExcluirConteudoCommand(_conteudoId, _usuarioIdTemporario));
        if (resultado.IsSuccess)
        {
            // null = deleted
            (Owner as Views.ConteudoDetalheWindow)?.Close(null);
        }
        else
        {
            MensagemErro = Strings.Erro_FalhaAoExcluir;
        }
    }

    [RelayCommand]
    private async Task CancelarAsync()
    {
        if (IsDirty && DialogServiceInstance is not null)
        {
            var descartar = await DialogServiceInstance.MostrarConfirmacaoAsync(
                Strings.Dialog_DescartarAlteracoes_Titulo,
                Strings.Dialog_DescartarAlteracoes_Mensagem,
                "Descartar",
                Strings.Label_Cancelar);

            if (!descartar)
                return;
        }

        (Owner as Views.ConteudoDetalheWindow)?.Close(false);
    }

    // Owner reference — set by ConteudoDetalheWindow after it creates this VM
    public Window? Owner { get; set; }

    // Enum values for ComboBox binding
    public static IReadOnlyList<FormatoMidia> FormatosMidia { get; } =
        Enum.GetValues<FormatoMidia>().ToList().AsReadOnly();
}

#pragma warning restore CA2007
