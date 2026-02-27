/**
 * Tipos de obra disponíveis no sistema.
 * Novos tipos são adicionados apenas pela equipe de desenvolvimento,
 * pois cada tipo pode ter campos e funcionalidades diferentes.
 */
export enum TipoObra {
  Manga = 'manga',
  Manhwa = 'manhwa',
  Manhua = 'manhua',
  Anime = 'anime',
  Livro = 'livro',
  Filme = 'filme',
  Serie = 'serie',
  Webnovel = 'webnovel',
}

/**
 * Rótulo da unidade de posição conforme o tipo da obra.
 * Ex.: manga/manhwa → capítulo; anime/série → episódio; filme → minuto.
 */
export const ROTULO_POSICAO: Record<TipoObra, string> = {
  [TipoObra.Manga]: 'capítulo',
  [TipoObra.Manhwa]: 'capítulo',
  [TipoObra.Manhua]: 'capítulo',
  [TipoObra.Anime]: 'episódio',
  [TipoObra.Livro]: 'capítulo',
  [TipoObra.Filme]: 'minuto',
  [TipoObra.Serie]: 'episódio',
  [TipoObra.Webnovel]: 'capítulo',
};

/**
 * Retorna a posição formatada conforme o tipo da obra.
 * Ex.: formatarPosicao(47, TipoObra.Manga) → "capítulo 47"
 */
export function formatarPosicao(posicao: number, tipo: TipoObra): string {
  return `${ROTULO_POSICAO[tipo]} ${posicao}`;
}

/**
 * Informação sobre a próxima publicação de uma obra,
 * consolidada entre suas origens/links.
 *
 * - dias_ate_proxima: quantos dias até a próxima parte ser publicada.
 * - partes_ja_publicadas: quantas partes já foram publicadas desde
 *   a última atualização de posição do usuário.
 */
export type ProximaInfo =
  | { tipo: 'dias_ate_proxima'; dias: number }
  | { tipo: 'partes_ja_publicadas'; quantidade: number };
