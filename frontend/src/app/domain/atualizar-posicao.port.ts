import type { Observable } from 'rxjs';
import { TipoObra } from './obra.types';

export interface ObraDetalhe {
  id: string;
  nome: string;
  tipo: TipoObra;
  posicaoAtual: number;
  dataUltimaAtualizacaoPosicao: string;
  ordemPreferencia: number;
  obraNova?: boolean;
}

export interface AtualizarPosicaoRequest {
  idObra?: string;
  nomeObra?: string;
  novaPosicao: number;
  dataUltimaAtualizacao?: string;
  criarSeNaoExistir: boolean;
  nomeParaCriar?: string;
  tipoParaCriar?: TipoObra;
  ordemPreferenciaParaCriar?: number;
}

export interface AtualizarPosicaoResponse {
  id: string;
  criada: boolean;
}

export abstract class IAtualizarPosicaoPort {
  abstract obterPorId(id: string): Observable<ObraDetalhe>;
  abstract obterPorNome(nome: string): Observable<ObraDetalhe>;
  abstract atualizarPosicao(request: AtualizarPosicaoRequest): Observable<AtualizarPosicaoResponse>;
}
