namespace DiarioDeBordo.Core.Primitivos;

/// <summary>
/// Catálogo centralizado de códigos de erro do sistema.
/// Códigos são constantes — nunca use strings literais em handlers.
/// </summary>
public static class Erros
{
    // Acervo — Conteudo
    public static readonly Erro TituloObrigatorio = new("TITULO_OBRIGATORIO", "O título é obrigatório.");
    public static readonly Erro TipoColetaneaObrigatorio = new("TIPO_COLETANEA_OBRIGATORIO", "TipoColetanea é obrigatório para Coletânea.");
    public static readonly Erro TipoColetaneaDeveSerNuloParaItem = new("TIPO_COLETANEA_DEVE_SER_NULO", "TipoColetanea deve ser nulo quando Papel é Item.");
    public static readonly Erro NotaForaDaFaixa = new("NOTA_FORA_DA_FAIXA", "Nota deve estar entre 0 e 10.");
    public static readonly Erro LimiteImagensAtingido = new("LIMITE_IMAGENS_ATINGIDO", "Limite de 20 imagens por conteúdo atingido.");
    public static readonly Erro ImagemExcedeTamanho = new("IMAGEM_EXCEDE_TAMANHO", "Imagem excede o limite de 10MB.");
    public static readonly Erro ImagemPrincipalDuplicada = new("IMAGEM_PRINCIPAL_DUPLICADA", "Apenas uma imagem pode ser marcada como principal.");
    public static readonly Erro PrioridadeFonteDuplicada = new("PRIORIDADE_FONTE_DUPLICADA", "Prioridade já utilizada por outra fonte deste conteúdo.");

    // Acervo — Coletânea
    public static readonly Erro CicloDetetadoNaComposicao = new("CICLO_COMPOSICAO", "Operação criaria ciclo na composição da coletânea.");
    public static readonly Erro PosicaoDuplicadaOuComLacuna = new("POSICAO_INVALIDA", "Posição duplicada ou com lacuna na coletânea guiada.");
    public static readonly Erro TipoColetaneaImutavel = new("TIPO_COLETANEA_IMUTAVEL", "TipoColetanea não pode ser alterado após criação.");

    // Acervo — Categoria
    public static readonly Erro NomeCategoriaObrigatorio = new("NOME_CATEGORIA_OBRIGATORIO", "Nome de categoria é obrigatório.");

    // Acervo — Relações
    public static readonly Erro AutoReferenciaProibida = new("AUTO_REFERENCIA_PROIBIDA", "Um conteúdo não pode se relacionar consigo mesmo.");
    public static readonly Erro RelacaoDuplicada = new("RELACAO_DUPLICADA", "Essa relação já existe entre os dois conteúdos.");
    public static readonly Erro NomeTipoRelacaoObrigatorio = new("NOME_TIPO_RELACAO_OBRIGATORIO", "Nome do tipo de relação é obrigatório.");
    public static readonly Erro NomeInversoObrigatorio = new("NOME_INVERSO_OBRIGATORIO", "Nome inverso do tipo de relação é obrigatório.");
    public static readonly Erro TotalEsperadoInvalido = new("TOTAL_ESPERADO_INVALIDO", "Total esperado de sessões deve ser maior que zero.");

    // Genéricos
    public static readonly Erro NaoAutorizado = new("NAO_AUTORIZADO", "Operação não autorizada.");
    public static readonly Erro NaoEncontrado = new("NAO_ENCONTRADO", "Recurso não encontrado.");
}
