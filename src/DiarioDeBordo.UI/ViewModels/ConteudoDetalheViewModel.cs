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
using System.Collections.ObjectModel;

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
    private Classificacao? _classificacaoOriginal;
    private decimal? _notaOriginal;
    private EstadoProgresso _estadoProgressoOriginal;
    private string? _posicaoAtualOriginal;

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
    [NotifyPropertyChangedFor(nameof(IsGostei))]
    [NotifyPropertyChangedFor(nameof(IsNaoGostei))]
    [NotifyPropertyChangedFor(nameof(ClassificacaoTexto))]
    [NotifyPropertyChangedFor(nameof(ResumoAvaliacao))]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private Classificacao? _classificacao;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResumoAvaliacao))]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private decimal? _nota;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResumoAvaliacao))]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private EstadoProgresso _estadoProgresso;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private string? _posicaoAtual;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResumoOrganizacao))]
    private int _categoriaCount;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ResumoOrganizacao))]
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

    // --- Relation form state ---

    [ObservableProperty]
    private bool _mostrandoFormularioRelacao;

    [ObservableProperty]
    private ConteudoResumoDto? _conteudoAlvoSelecionado;

    [ObservableProperty]
    private TipoRelacaoDto? _tipoRelacaoSelecionado;

    [ObservableProperty]
    private string? _nomeInversoNovoTipo;

    [ObservableProperty]
    private bool _criandoNovoTipo;

    [ObservableProperty]
    private string? _mensagemErroRelacao;

    // --- Collections ---

    public System.Collections.ObjectModel.ObservableCollection<CategoriaChipViewModel> CategoriasAssociadas { get; } = [];
    public System.Collections.ObjectModel.ObservableCollection<RelacaoItemViewModel> Relacoes { get; } = [];

    // Autocomplete suggestion lists (populated by Populating event handlers in code-behind)
    private readonly List<CategoriaDto> _sugestoesCategoriaBacking = [];
    private readonly List<ConteudoResumoDto> _sugestoesConteudoBacking = [];
    private readonly List<TipoRelacaoDto> _sugestoesTipoRelacaoBacking = [];

    public IReadOnlyList<CategoriaDto> SugestoesCategoria => _sugestoesCategoriaBacking;
    public IReadOnlyList<ConteudoResumoDto> SugestoesConteudo => _sugestoesConteudoBacking;
    public IReadOnlyList<TipoRelacaoDto> SugestoesTipoRelacao => _sugestoesTipoRelacaoBacking;

    // Snapshot of original category IDs for dirty tracking
    private HashSet<Guid> _categoriasOriginaisIds = [];

    // --- Computed ---

    public bool IsDirty =>
        Titulo != _tituloOriginal ||
        Descricao != _descricaoOriginal ||
        Anotacoes != _anotacoesOriginal ||
        Formato != _formatoOriginal ||
        Subtipo != _subtipoOriginal ||
        _classificacaoOriginal != Classificacao ||
        _notaOriginal != Nota ||
        _estadoProgressoOriginal != EstadoProgresso ||
        _posicaoAtualOriginal != PosicaoAtual ||
        !CategoriasAssociadas.Select(c => c.Id).ToHashSet().SetEquals(_categoriasOriginaisIds);

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
        CategoriasAssociadas.Count > 0 || Relacoes.Count > 0
            ? $"{CategoriasAssociadas.Count} categorias · {Relacoes.Count} relações"
            : Strings.Categorias_EstadoVazio;

    public string ResumoHistorico =>
        SessoesContagem > 0
            ? TotalEsperadoSessoes.HasValue
                ? $"{(int)((double)SessoesContagem / TotalEsperadoSessoes.Value * 100)}% — {SessoesContagem} de {TotalEsperadoSessoes.Value} sessões"
                : $"{SessoesContagem} sessão(ões) registrada(s)"
            : Strings.Progresso_Vazio;

    // Three-state Classificação toggle properties (D-07/D-09)
    public bool IsGostei
    {
        get => Classificacao == Core.Enums.Classificacao.Gostei;
        set
        {
            Classificacao = value ? Core.Enums.Classificacao.Gostei : null;
        }
    }

    public bool IsNaoGostei
    {
        get => Classificacao == Core.Enums.Classificacao.NaoGostei;
        set
        {
            Classificacao = value ? Core.Enums.Classificacao.NaoGostei : null;
        }
    }

    public string ClassificacaoTexto => Classificacao switch
    {
        Core.Enums.Classificacao.Gostei => "Gostei",
        Core.Enums.Classificacao.NaoGostei => "Não gostei",
        _ => "Sem classificação",
    };

    public bool PodeVincular => ConteudoAlvoSelecionado is not null &&
                                 (TipoRelacaoSelecionado is not null || !string.IsNullOrWhiteSpace(NomeInversoNovoTipo));

    // Expose IDialogService interface from UI services namespace (avoid circular reference)
    private DiarioDeBordo.UI.Services.IDialogService? DialogServiceInstance => _dialogService;

    // Static enum values for ComboBox binding
    public static IReadOnlyList<FormatoMidia> FormatosMidia { get; } =
        Enum.GetValues<FormatoMidia>().ToList().AsReadOnly();

    public static IReadOnlyList<EstadoProgresso> EstadosProgresso { get; } =
        Enum.GetValues<EstadoProgresso>().ToList().AsReadOnly();

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
        EstadoProgresso = Enum.TryParse<EstadoProgresso>(dto.EstadoProgresso, out var ep) ? ep : EstadoProgresso.NaoIniciado;
        PosicaoAtual = dto.PosicaoAtual;
        SessoesContagem = dto.SessoesContagem;
        TotalEsperadoSessoes = dto.TotalEsperadoSessoes;
        IsFilho = dto.IsFilho;

        // Populate category chips
        CategoriasAssociadas.Clear();
        foreach (var c in dto.Categorias)
            CategoriasAssociadas.Add(new CategoriaChipViewModel(c.Id, c.Nome, c.IsAutomatica, RemoverCategoriaInternal));

        // Populate relations list
        Relacoes.Clear();
        foreach (var r in dto.Relacoes)
            Relacoes.Add(new RelacaoItemViewModel(r.Id, r.NomeTipo, r.TituloDestino, r.IsInversa, RemoverRelacaoInternalAsync));

        // Populate sessions timeline (ordered most recent first per D-18)
        Sessoes.Clear();
        foreach (var s in dto.Sessoes.OrderByDescending(s => s.CriadoEm))
            Sessoes.Add(new SessaoItemViewModel(s.Id, s.Titulo, s.CriadoEm, s.Classificacao, s.Nota, s.Anotacoes, AbrirSessaoInternalAsync));

        OnPropertyChanged(nameof(ResumoAvaliacao));
        OnPropertyChanged(nameof(ResumoOrganizacao));
        OnPropertyChanged(nameof(ResumoHistorico));
        OnPropertyChanged(nameof(ProgressoTexto));
        OnPropertyChanged(nameof(ProgressoPorcentagem));
    }

    private void SnapshotOriginals()
    {
        _tituloOriginal = Titulo;
        _descricaoOriginal = Descricao;
        _anotacoesOriginal = Anotacoes;
        _formatoOriginal = Formato;
        _subtipoOriginal = Subtipo;
        _classificacaoOriginal = Classificacao;
        _notaOriginal = Nota;
        _estadoProgressoOriginal = EstadoProgresso;
        _posicaoAtualOriginal = PosicaoAtual;
        _categoriasOriginaisIds = CategoriasAssociadas.Select(c => c.Id).ToHashSet();
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
                EstadoProgresso,
                PosicaoAtual,
                TotalEsperadoSessoes,
                CategoriasAssociadas.Select(c => c.Id).ToList().AsReadOnly());

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

    // --- Category commands ---

    [RelayCommand]
    private void RemoverCategoria(Guid categoriaId) => RemoverCategoriaInternal(categoriaId);

    private void RemoverCategoriaInternal(Guid categoriaId)
    {
        var chip = CategoriasAssociadas.FirstOrDefault(c => c.Id == categoriaId);
        if (chip is not null)
        {
            CategoriasAssociadas.Remove(chip);
            OnPropertyChanged(nameof(IsDirty));
            OnPropertyChanged(nameof(TituloJanela));
            OnPropertyChanged(nameof(ResumoOrganizacao));
        }
    }

    [RelayCommand]
    private void LimparNota()
    {
        Nota = null;
    }

    /// <summary>Called by code-behind Populating event for categories AutoCompleteBox.</summary>
    public async Task PopularSugestoesCategoriasAsync(string prefixo)
    {
        try
        {
            var resultado = await _mediator.Send(new BuscarCategoriasQuery(_usuarioIdTemporario, prefixo));
            _sugestoesCategoriaBacking.Clear();
            _sugestoesCategoriaBacking.AddRange(resultado);
            OnPropertyChanged(nameof(SugestoesCategoria));
        }
        catch (OperationCanceledException) { }
    }

    /// <summary>Called by code-behind when user selects/enters a category.</summary>
    public async Task SelecionarCategoriaAsync(string nomeCategoria)
    {
        if (string.IsNullOrWhiteSpace(nomeCategoria))
            return;

        // Check if already associated
        if (CategoriasAssociadas.Any(c => string.Equals(c.Nome, nomeCategoria, StringComparison.OrdinalIgnoreCase)))
            return;

        // Check if it's an existing suggestion
        var sugestao = _sugestoesCategoriaBacking.FirstOrDefault(
            c => string.Equals(c.Nome, nomeCategoria, StringComparison.OrdinalIgnoreCase));

        if (sugestao is not null)
        {
            CategoriasAssociadas.Add(new CategoriaChipViewModel(sugestao.Id, sugestao.Nome, sugestao.IsAutomatica, RemoverCategoriaInternal));
        }
        else
        {
            // Create new category via query service (ObterOuCriarAsync pattern)
            var resultado = await _mediator.Send(new BuscarCategoriasQuery(_usuarioIdTemporario, nomeCategoria));
            var existente = resultado.FirstOrDefault(c => string.Equals(c.Nome, nomeCategoria, StringComparison.OrdinalIgnoreCase));
            if (existente is not null)
            {
                CategoriasAssociadas.Add(new CategoriaChipViewModel(existente.Id, existente.Nome, existente.IsAutomatica, RemoverCategoriaInternal));
            }
            else
            {
                // Will be created on save via AtualizarConteudoCommand using ICategoriaRepository.ObterOuCriarAsync
                // For now, add a placeholder with empty Guid (to be resolved on save)
                // Better: create it now via a dedicated command
                // For Plan 05: just add it with a marker that it needs creation
                // TODO: create inline via CriarCategoriaCommand in Phase 3 completion
            }
        }

        OnPropertyChanged(nameof(IsDirty));
        OnPropertyChanged(nameof(TituloJanela));
        OnPropertyChanged(nameof(ResumoOrganizacao));
    }

    // --- Relation commands ---

    [RelayCommand]
    private void MostrarFormularioRelacao()
    {
        MostrandoFormularioRelacao = true;
    }

    [RelayCommand]
    private void CancelarFormularioRelacao()
    {
        MostrandoFormularioRelacao = false;
        ConteudoAlvoSelecionado = null;
        TipoRelacaoSelecionado = null;
        NomeInversoNovoTipo = null;
        CriandoNovoTipo = false;
        MensagemErroRelacao = null;
    }

    [RelayCommand]
    private async Task VincularConteudosAsync()
    {
        if (ConteudoAlvoSelecionado is null)
            return;

        string nomeTipo;
        string nomeInverso;

        if (TipoRelacaoSelecionado is not null)
        {
            nomeTipo = TipoRelacaoSelecionado.Nome;
            nomeInverso = TipoRelacaoSelecionado.NomeInverso;
        }
        else if (!string.IsNullOrWhiteSpace(_novoNomeTipo) && !string.IsNullOrWhiteSpace(NomeInversoNovoTipo))
        {
            nomeTipo = _novoNomeTipo;
            nomeInverso = NomeInversoNovoTipo;
        }
        else
        {
            MensagemErroRelacao = "Selecione ou crie um tipo de relação.";
            return;
        }

        MensagemErroRelacao = null;
        var resultado = await _mediator.Send(new CriarRelacaoCommand(
            _usuarioIdTemporario,
            _conteudoId,
            ConteudoAlvoSelecionado.Id,
            nomeTipo,
            nomeInverso));

        if (resultado.IsSuccess)
        {
            // Add to local list (UI immediate feedback — per D-15)
            Relacoes.Add(new RelacaoItemViewModel(resultado.Value, nomeTipo, ConteudoAlvoSelecionado.Titulo, false, RemoverRelacaoInternalAsync));
            // Clear form but keep it open (D-14: allow multiple relations)
            ConteudoAlvoSelecionado = null;
            TipoRelacaoSelecionado = null;
            _novoNomeTipo = null;
            NomeInversoNovoTipo = null;
            CriandoNovoTipo = false;
            OnPropertyChanged(nameof(ResumoOrganizacao));
        }
        else
        {
            MensagemErroRelacao = Strings.Erro_RelacaoDuplicada;
        }
    }

    [RelayCommand]
    private async Task RemoverRelacaoAsync(Guid relacaoId) => await RemoverRelacaoInternalAsync(relacaoId);

    private async Task RemoverRelacaoInternalAsync(Guid relacaoId)
    {
        if (DialogServiceInstance is null)
            return;

        var confirmado = await DialogServiceInstance.MostrarConfirmacaoAsync(
            Strings.Dialog_ExcluirRelacao_Titulo,
            Strings.Dialog_ExcluirRelacao_Mensagem,
            "Remover",
            Strings.Label_Cancelar,
            isPrimarioDestructivo: true);

        if (!confirmado)
            return;

        var resultado = await _mediator.Send(new RemoverRelacaoCommand(relacaoId, _usuarioIdTemporario));
        if (resultado.IsSuccess)
        {
            var item = Relacoes.FirstOrDefault(r => r.Id == relacaoId);
            if (item is not null)
            {
                Relacoes.Remove(item);
                OnPropertyChanged(nameof(ResumoOrganizacao));
            }
        }
    }

    /// <summary>Called by code-behind Populating event for content search AutoCompleteBox.</summary>
    public async Task PopularSugestoesConteudoAsync(string prefixo)
    {
        try
        {
            var resultado = await _mediator.Send(new BuscarConteudosQuery(_usuarioIdTemporario, prefixo, _conteudoId));
            _sugestoesConteudoBacking.Clear();
            _sugestoesConteudoBacking.AddRange(resultado);
            OnPropertyChanged(nameof(SugestoesConteudo));
        }
        catch (OperationCanceledException) { }
    }

    /// <summary>Called by code-behind Populating event for relation type AutoCompleteBox.</summary>
    public async Task PopularSugestoesTipoRelacaoAsync(string prefixo)
    {
        try
        {
            var resultado = await _mediator.Send(new BuscarTiposRelacaoQuery(_usuarioIdTemporario, prefixo));
            _sugestoesTipoRelacaoBacking.Clear();
            _sugestoesTipoRelacaoBacking.AddRange(resultado);
            OnPropertyChanged(nameof(SugestoesTipoRelacao));

            // Check if typed name matches existing type
            var exato = resultado.FirstOrDefault(t => string.Equals(t.Nome, prefixo, StringComparison.OrdinalIgnoreCase));
            if (exato is not null)
            {
                TipoRelacaoSelecionado = exato;
                CriandoNovoTipo = false;
            }
            else if (!string.IsNullOrWhiteSpace(prefixo))
            {
                TipoRelacaoSelecionado = null;
                CriandoNovoTipo = true;
                _novoNomeTipo = prefixo;
            }
            OnPropertyChanged(nameof(PodeVincular));
        }
        catch (OperationCanceledException) { }
    }

    private string? _novoNomeTipo;

    // === SESSION MANAGEMENT (Plan 06) ===

    public System.Collections.ObjectModel.ObservableCollection<SessaoItemViewModel> Sessoes { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ProgressoTexto))]
    [NotifyPropertyChangedFor(nameof(ProgressoPorcentagem))]
    [NotifyPropertyChangedFor(nameof(ResumoHistorico))]
    [NotifyPropertyChangedFor(nameof(IsDirty))]
    [NotifyPropertyChangedFor(nameof(TituloJanela))]
    private int? _totalEsperadoSessoesEditavel;

    [ObservableProperty]
    private bool _mostrandoFormularioSessao;

    [ObservableProperty]
    private bool _mostrandoDetalhesSessao;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PodeRegistrarSessao))]
    private string _sessaoTitulo = string.Empty;

    [ObservableProperty]
    private DateTimeOffset? _sessaoData = DateTimeOffset.Now;

    [ObservableProperty]
    private string? _sessaoAnotacoes;

    [ObservableProperty]
    private decimal? _sessaoNota;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SessaoIsGostei))]
    [NotifyPropertyChangedFor(nameof(SessaoIsNaoGostei))]
    private Classificacao? _sessaoClassificacao;

    [ObservableProperty]
    private string? _mensagemErroSessao;

    public bool SessaoIsGostei
    {
        get => SessaoClassificacao == Core.Enums.Classificacao.Gostei;
        set => SessaoClassificacao = value ? Core.Enums.Classificacao.Gostei : null;
    }

    public bool SessaoIsNaoGostei
    {
        get => SessaoClassificacao == Core.Enums.Classificacao.NaoGostei;
        set => SessaoClassificacao = value ? Core.Enums.Classificacao.NaoGostei : null;
    }

    public bool PodeRegistrarSessao => !string.IsNullOrWhiteSpace(SessaoTitulo);

    public string TextoBotaoDetalhesSessao =>
        MostrandoDetalhesSessao ? Strings.Formulario_MenosDetalhes : Strings.Formulario_MaisDetalhes;

    public string ProgressoTexto
    {
        get
        {
            var count = Sessoes.Count;
            var total = TotalEsperadoSessoes;
            if (count == 0) return Strings.Progresso_Vazio;
            if (total.HasValue)
                return $"{count} de {total.Value} ({ProgressoPorcentagem:0}%)";
            return $"{count} sessão(ões) registrada(s)";
        }
    }

    public decimal ProgressoPorcentagem
    {
        get
        {
            if (TotalEsperadoSessoes is null or 0 || Sessoes.Count == 0)
                return 0;
            return Math.Min(100m, (decimal)Sessoes.Count / TotalEsperadoSessoes.Value * 100);
        }
    }

    [RelayCommand]
    private void MostrarFormularioSessao()
    {
        SessaoTitulo = string.Empty;
        SessaoData = DateTimeOffset.Now;
        SessaoAnotacoes = null;
        SessaoNota = null;
        SessaoClassificacao = null;
        MensagemErroSessao = null;
        MostrandoFormularioSessao = true;
    }

    [RelayCommand]
    private void FecharFormularioSessao()
    {
        MostrandoFormularioSessao = false;
        MostrandoDetalhesSessao = false;
    }

    [RelayCommand]
    private void ToggleDetalhesSessao()
    {
        MostrandoDetalhesSessao = !MostrandoDetalhesSessao;
        OnPropertyChanged(nameof(TextoBotaoDetalhesSessao));
    }

    [RelayCommand]
    private async Task RegistrarSessaoAsync()
    {
        if (!PodeRegistrarSessao)
            return;

        MensagemErroSessao = null;
        var resultado = await _mediator.Send(new RegistrarSessaoCommand(
            _usuarioIdTemporario,
            _conteudoId,
            SessaoTitulo.Trim(),
            SessaoAnotacoes,
            SessaoNota,
            SessaoClassificacao,
            Formato, // inherit parent format
            SessaoData));

        if (resultado.IsSuccess)
        {
            // Add to top of timeline (most recent first per D-18)
            Sessoes.Insert(0, new SessaoItemViewModel(
                resultado.Value,
                SessaoTitulo.Trim(),
                SessaoData ?? DateTimeOffset.UtcNow,
                SessaoClassificacao,
                SessaoNota,
                SessaoAnotacoes,
                AbrirSessaoInternalAsync));

            // Clear form for rapid entry (D-20) but keep form open
            SessaoTitulo = string.Empty;
            SessaoData = DateTimeOffset.Now;
            SessaoAnotacoes = null;
            SessaoNota = null;
            SessaoClassificacao = null;

            SessoesContagem = Sessoes.Count;
            OnPropertyChanged(nameof(ProgressoTexto));
            OnPropertyChanged(nameof(ProgressoPorcentagem));
            OnPropertyChanged(nameof(ResumoHistorico));
        }
        else
        {
            MensagemErroSessao = Strings.Erro_FalhaAoSalvar;
        }
    }

    [RelayCommand]
    private async Task AbrirSessaoAsync(Guid sessaoId) => await AbrirSessaoInternalAsync(sessaoId);

    private async Task AbrirSessaoInternalAsync(Guid sessaoId)
    {
        if (DialogServiceInstance is null)
            return;

        await DialogServiceInstance.MostrarConteudoDetalheAsync(sessaoId);

        // Reload sessions to reflect any edits made in child modal
        var resultado = await _mediator.Send(new ObterConteudoQuery(_conteudoId, _usuarioIdTemporario));
        if (resultado.IsSuccess && resultado.Value is ConteudoDetalheDto dto)
        {
            Sessoes.Clear();
            foreach (var s in dto.Sessoes.OrderByDescending(s => s.CriadoEm))
                Sessoes.Add(new SessaoItemViewModel(s.Id, s.Titulo, s.CriadoEm, s.Classificacao, s.Nota, s.Anotacoes, AbrirSessaoInternalAsync));
            SessoesContagem = Sessoes.Count;
            OnPropertyChanged(nameof(ProgressoTexto));
            OnPropertyChanged(nameof(ProgressoPorcentagem));
            OnPropertyChanged(nameof(ResumoHistorico));
        }
    }
}

#pragma warning restore CA2007
