import { ProximaInfo, TipoObra } from './obra.types';

/**
 * Projeção mínima de uma obra para exibição na tela de listagem paginada.
 * Contém apenas os dados necessários para a lista; detalhes (links, sinopse,
 * comentários, etc.) são carregados sob demanda via query separada.
 */
export interface ObraListItem {
  /** Identificador opaco. Formato (UUID/numérico) decidido pelo backend. */
  id: string;

  /** Nome da obra. */
  nome: string;

  /** Tipo da obra — define o rótulo de unidade da posição. */
  tipo: TipoObra;

  /**
   * Data da última atualização de posição, consolidada entre origens.
   * A hora não é relevante para exibição; usa-se apenas a data.
   * Exibição: texto relativo (ex.: "há 3 dias"); tooltip: dd/MM/yyyy.
   */
  ultimaAtualizacaoPosicao: Date;

  /**
   * Número inteiro da parte em que o usuário parou.
   * A formatação (ex.: "capítulo 47") é responsabilidade da UI,
   * usando o rótulo do tipo.
   */
  posicaoAtual: number;

  /**
   * Informação sobre a próxima publicação (opcional).
   * Pode ser dias até a próxima parte ou partes já publicadas desde
   * a última atualização. Ausente quando não disponível.
   */
  proximaInfo?: ProximaInfo;

  /**
   * Campo de ranqueamento por preferência do usuário.
   * Utilizado para a ordenação default da listagem (menor valor = maior preferência).
   */
  ordemPreferencia: number;
}
