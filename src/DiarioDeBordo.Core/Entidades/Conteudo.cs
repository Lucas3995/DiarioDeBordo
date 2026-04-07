using DiarioDeBordo.Core.Enums;
using DiarioDeBordo.Core.Primitivos;

namespace DiarioDeBordo.Core.Entidades;

/// <summary>Aggregate Root do BC Acervo. Protege consistência dos dados de um item do acervo.</summary>
public sealed class Conteudo
{
    public Guid Id { get; init; }
    public required string Titulo { get; set; }
    public string? Descricao { get; set; }
    public string? Anotacoes { get; set; }
    public decimal? Nota { get; private set; }
    public FormatoMidia Formato { get; set; }
    public string? Subtipo { get; set; }
    public PapelConteudo Papel { get; init; }
    public TipoColetanea? TipoColetaneaValor { get; private set; }
    public Guid UsuarioId { get; init; }
    public DateTimeOffset CriadoEm { get; init; }
    public DateTimeOffset AtualizadoEm { get; set; }

    /// <summary>Reação imediata do usuário (D-06/D-08). Null = não classificado.</summary>
    public Classificacao? Classificacao { get; private set; }

    /// <summary>Indica se este conteúdo é filho de outro (sessão/episódio). Oculto na lista principal (D-19).</summary>
    public bool IsFilho { get; init; }

    /// <summary>Total esperado de sessões filhas para cálculo de progresso percentual (D-21). Null = mostra contagem absoluta.</summary>
    public int? TotalEsperadoSessoes { get; private set; }

    // Child collections (owned by aggregate)
    private readonly List<Fonte> _fontes = [];
    private readonly List<ImagemConteudo> _imagens = [];

    public IReadOnlyList<Fonte> Fontes => _fontes.AsReadOnly();
    public IReadOnlyList<ImagemConteudo> Imagens => _imagens.AsReadOnly();

    // Progresso (value object, 1:1)
    public Progresso Progresso { get; private set; } = new();

    // ---- Factory ----

    /// <summary>
    /// Cria um Conteudo com validação de invariantes.
    /// Prefer this over direct construction in production code.
    /// </summary>
    public static Conteudo Criar(
        Guid usuarioId,
        string titulo,
        PapelConteudo papel = PapelConteudo.Item,
        TipoColetanea? tipoColetanea = null,
        FormatoMidia formato = FormatoMidia.Nenhum)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("TITULO_OBRIGATORIO", "O título é obrigatório."); // I-01

        if (papel == PapelConteudo.Coletanea && tipoColetanea is null)
            throw new DomainException("TIPO_COLETANEA_OBRIGATORIO", "TipoColetanea é obrigatório para Coletânea."); // I-02

        if (papel == PapelConteudo.Item && tipoColetanea is not null)
            throw new DomainException("TIPO_COLETANEA_DEVE_SER_NULO", "TipoColetanea deve ser nulo quando Papel é Item."); // I-02

        var now = DateTimeOffset.UtcNow;
        return new Conteudo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Titulo = titulo.Trim(),
            Papel = papel,
            TipoColetaneaValor = tipoColetanea,
            Formato = formato,
            CriadoEm = now,
            AtualizadoEm = now,
            IsFilho = false,
        };
    }

    /// <summary>
    /// Cria um Conteudo filho (sessão/episódio). IsFilho=true, oculto da lista principal.
    /// </summary>
    public static Conteudo CriarComoFilho(
        Guid usuarioId,
        string titulo,
        FormatoMidia formato = FormatoMidia.Nenhum,
        DateTimeOffset? dataConsumo = null)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("TITULO_OBRIGATORIO", "O título é obrigatório."); // I-01

        var now = dataConsumo ?? DateTimeOffset.UtcNow;
        return new Conteudo
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuarioId,
            Titulo = titulo.Trim(),
            Papel = PapelConteudo.Item,
            Formato = formato,
            CriadoEm = now,
            AtualizadoEm = now,
            IsFilho = true,
        };
    }

    // ---- Invariant-enforcing operations ----

    /// <summary>I-03: Nota no intervalo [0, 10].</summary>
    public void DefinirNota(decimal nota)
    {
        if (nota < 0 || nota > 10)
            throw new DomainException("NOTA_FORA_DA_FAIXA", "Nota deve estar entre 0 e 10.");
        Nota = nota;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>Limpa a nota (set to null). D-06: independente de Classificacao.</summary>
    public void LimparNota()
    {
        Nota = null;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>Define a classificação imediata. D-06/D-07: independente de Nota. Null = não classificado.</summary>
    public void DefinirClassificacao(Classificacao? classificacao)
    {
        Classificacao = classificacao;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>Define o total esperado de sessões filhas (D-21). Null = sem total definido.</summary>
    public void DefinirTotalEsperadoSessoes(int? total)
    {
        if (total.HasValue && total.Value <= 0)
            throw new DomainException("TOTAL_ESPERADO_INVALIDO", "Total esperado de sessões deve ser maior que zero.");
        TotalEsperadoSessoes = total;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>I-04, I-05: Máx. 20 imagens, máx. 10MB por imagem.</summary>
    public void AdicionarImagem(string caminho, OrigemImagem origem, long tamanhoBytes)
    {
        if (_imagens.Count >= 20)
            throw new DomainException("LIMITE_IMAGENS_ATINGIDO", "Limite de 20 imagens por conteúdo atingido.");

        const long maxBytes = 10L * 1024 * 1024; // 10MB
        if (tamanhoBytes > maxBytes)
            throw new DomainException("IMAGEM_EXCEDE_TAMANHO", "Imagem excede o limite de 10MB.");

        var principal = _imagens.Count == 0; // First image is automatically principal
        _imagens.Add(new ImagemConteudo
        {
            Id = Guid.NewGuid(),
            ConteudoId = Id,
            Caminho = caminho,
            OrigemTipo = origem,
            Principal = principal,
        });
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>I-06: Apenas uma imagem principal.</summary>
    public void DefinirImagemPrincipal(Guid imagemId)
    {
        var imagem = _imagens.FirstOrDefault(i => i.Id == imagemId)
            ?? throw new DomainException("NAO_ENCONTRADO", "Imagem não encontrada neste conteúdo.");

        foreach (var img in _imagens)
            img.Principal = false;

        imagem.Principal = true;
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>I-07: Prioridade de fonte única por conteúdo.</summary>
    public void AdicionarFonte(TipoFonte tipo, string valor, int prioridade, string? plataforma = null)
    {
        if (_fontes.Any(f => f.Prioridade == prioridade))
            throw new DomainException("PRIORIDADE_FONTE_DUPLICADA", "Prioridade já utilizada por outra fonte deste conteúdo.");

        _fontes.Add(new Fonte
        {
            Id = Guid.NewGuid(),
            ConteudoId = Id,
            Tipo = tipo,
            Valor = valor,
            Prioridade = prioridade,
            Plataforma = plataforma,
        });
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    public void AlterarProgresso(EstadoProgresso estado, string? posicaoAtual = null)
    {
        Progresso = Progresso with { Estado = estado, PosicaoAtual = posicaoAtual };
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>I-10: TipoColetanea imutável após criação.</summary>
    public void AlterarTipoColetanea(TipoColetanea novoTipo)
    {
        // TipoColetanea is set once at Criar() and cannot be changed
        throw new DomainException("TIPO_COLETANEA_IMUTAVEL", "TipoColetanea não pode ser alterado após criação.");
    }

    /// <summary>Remove uma fonte pelo ID. Throws DomainException se não encontrada.</summary>
    public void RemoverFonte(Guid fonteId)
    {
        var fonte = _fontes.FirstOrDefault(f => f.Id == fonteId)
            ?? throw new DomainException("NAO_ENCONTRADO", "Fonte não encontrada neste conteúdo.");
        _fontes.Remove(fonte);
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Reordena fontes na sequência fornecida. Atribui prioridades 1..N.
    /// Throws DomainException se os IDs não correspondem exatamente às fontes existentes (I-07).
    /// </summary>
    public void ReordenarFontes(IReadOnlyList<Guid> fonteIdsOrdenados)
    {
        ArgumentNullException.ThrowIfNull(fonteIdsOrdenados);

        if (fonteIdsOrdenados.Count != _fontes.Count ||
            !fonteIdsOrdenados.ToHashSet().SetEquals(_fontes.Select(f => f.Id)))
            throw new DomainException("FONTES_INCONSISTENTES",
                "A lista de IDs fornecida não corresponde às fontes existentes deste conteúdo.");

        for (var i = 0; i < fonteIdsOrdenados.Count; i++)
        {
            var fonte = _fontes.First(f => f.Id == fonteIdsOrdenados[i]);
            fonte.Prioridade = i + 1;
        }
        AtualizadoEm = DateTimeOffset.UtcNow;
    }

    /// <summary>Remove uma imagem pelo ID. Throws DomainException se não encontrada.</summary>
    public void RemoverImagem(Guid imagemId)
    {
        var imagem = _imagens.FirstOrDefault(i => i.Id == imagemId)
            ?? throw new DomainException("NAO_ENCONTRADO", "Imagem não encontrada neste conteúdo.");
        _imagens.Remove(imagem);
        AtualizadoEm = DateTimeOffset.UtcNow;
    }
}
