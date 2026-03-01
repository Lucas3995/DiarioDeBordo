using DiarioDeBordo.Domain.Common;

namespace DiarioDeBordo.Domain.Obras;

/// <summary>
/// Entidade de domínio que representa uma obra acompanhada pelo usuário
/// (manga, manhwa, anime, livro, filme, série, webnovel, etc.).
///
/// Nesta fase, armazena os campos consolidados necessários para a listagem paginada.
/// Futuras features (origens de consumo, cálculo de previsão, histórico) serão
/// introduzidas em camadas incrementais.
///
/// Datas são armazenadas em UTC; a exibição em fuso Brasil (UTC-3) é responsabilidade
/// da camada de apresentação (frontend).
/// </summary>
public sealed class Obra : Entity
{
    /// <summary>Nome da obra.</summary>
    public string Nome { get; private set; } = string.Empty;

    /// <summary>Tipo da obra. Determina a unidade de posição exibida (capítulo, episódio, minuto, etc.).</summary>
    public TipoObra Tipo { get; private set; }

    /// <summary>
    /// Posição atual (número inteiro): capítulo, episódio, minuto, etc.,
    /// conforme o tipo. O frontend resolve o rótulo a partir do tipo.
    /// </summary>
    public int PosicaoAtual { get; private set; }

    /// <summary>
    /// Data (UTC) da última atualização de posição — consolidada entre origens.
    /// A exibição no frontend usa formato relativo (ex.: "há 3 dias") com
    /// tooltip dd/MM/yyyy em UTC-3 (America/Sao_Paulo).
    /// </summary>
    public DateTime DataUltimaAtualizacaoPosicao { get; private set; }

    /// <summary>
    /// Quando não nulo, indica o tipo de previsão consolidada disponível:
    /// dias até a próxima parte ou partes já publicadas desde a última leitura.
    /// </summary>
    public ProximaInfoTipo? ProximaInfoTipo { get; private set; }

    /// <summary>Dias estimados até a publicação da próxima parte. Só populado quando ProximaInfoTipo == DiasAteProxima.</summary>
    public int? DiasAteProximaParte { get; private set; }

    /// <summary>Partes já publicadas desde a última leitura. Só populado quando ProximaInfoTipo == PartesJaPublicadas.</summary>
    public int? PartesJaPublicadas { get; private set; }

    /// <summary>
    /// Campo de ranqueamento/preferência do usuário.
    /// Usado como ordenação default da lista. Menor valor = maior prioridade.
    /// </summary>
    public int OrdemPreferencia { get; private set; }

    private Obra() { }

    /// <summary>Cria uma nova obra com todos os campos obrigatórios.</summary>
    public Obra(
        string nome,
        TipoObra tipo,
        int posicaoAtual,
        DateTime dataUltimaAtualizacaoPosicao,
        int ordemPreferencia)
    {
        ValidarNome(nome);
        ValidarOrdemPreferencia(ordemPreferencia);
        ValidarPosicaoAtual(posicaoAtual);

        Nome = nome;
        Tipo = tipo;
        PosicaoAtual = posicaoAtual;
        DataUltimaAtualizacaoPosicao = DateTime.SpecifyKind(dataUltimaAtualizacaoPosicao, DateTimeKind.Utc);
        OrdemPreferencia = ordemPreferencia;
    }

    /// <summary>Define a previsão de próxima parte como "dias até próxima".</summary>
    public void DefinirDiasAteProxima(int dias)
    {
        if (dias < 0) throw new ArgumentException("Dias até próxima parte não pode ser negativo.", nameof(dias));
        ProximaInfoTipo = Obras.ProximaInfoTipo.DiasAteProxima;
        DiasAteProximaParte = dias;
        PartesJaPublicadas = null;
    }

    /// <summary>Define a previsão de próxima parte como "partes já publicadas".</summary>
    public void DefinirPartesJaPublicadas(int quantidade)
    {
        if (quantidade <= 0) throw new ArgumentException("Quantidade de partes publicadas deve ser positiva.", nameof(quantidade));
        ProximaInfoTipo = Obras.ProximaInfoTipo.PartesJaPublicadas;
        PartesJaPublicadas = quantidade;
        DiasAteProximaParte = null;
    }

    /// <summary>Remove qualquer previsão consolidada.</summary>
    public void LimparProximaInfo()
    {
        ProximaInfoTipo = null;
        DiasAteProximaParte = null;
        PartesJaPublicadas = null;
    }

    /// <summary>
    /// Atualiza a posição atual e a data da última atualização de posição.
    /// </summary>
    /// <param name="novaPosicao">Nova posição (capítulo, episódio, etc.); deve ser &gt;= 0.</param>
    /// <param name="dataUltimaAtualizacao">Data da última atualização; se nulo, usa UTC now.</param>
    public void AtualizarPosicao(int novaPosicao, DateTime? dataUltimaAtualizacao = null)
    {
        ValidarPosicaoAtual(novaPosicao);
        PosicaoAtual = novaPosicao;
        DataUltimaAtualizacaoPosicao = DateTime.SpecifyKind(
            dataUltimaAtualizacao ?? DateTime.UtcNow,
            DateTimeKind.Utc);
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome da obra não pode ser vazio.", nameof(nome));
        if (nome.Length > 300)
            throw new ArgumentException("O nome da obra não pode ter mais de 300 caracteres.", nameof(nome));
    }

    private static void ValidarOrdemPreferencia(int ordemPreferencia)
    {
        if (ordemPreferencia < 0)
            throw new ArgumentException("A ordem de preferência não pode ser negativa.", nameof(ordemPreferencia));
    }

    private static void ValidarPosicaoAtual(int posicaoAtual)
    {
        if (posicaoAtual < 0)
            throw new ArgumentException("A posição atual não pode ser negativa.", nameof(posicaoAtual));
    }
}
