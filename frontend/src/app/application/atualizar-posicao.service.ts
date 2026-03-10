import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  IAtualizarPosicaoPort,
  ObraDetalhe,
  AtualizarPosicaoRequest,
  AtualizarPosicaoResponse,
} from '../domain/atualizar-posicao.port';

export type { ObraDetalhe, AtualizarPosicaoRequest, AtualizarPosicaoResponse } from '../domain/atualizar-posicao.port';

/**
 * Caso de uso: Atualizar posição de obra.
 *
 * Delega à porta abstrata, mantendo isolamento da infraestrutura HTTP (CQRS).
 */
@Injectable()
export class AtualizarPosicaoService {
  constructor(private readonly porta: IAtualizarPosicaoPort) {}

  obterPorId(id: string): Observable<ObraDetalhe> {
    return this.porta.obterPorId(id);
  }

  obterPorNome(nome: string): Observable<ObraDetalhe> {
    return this.porta.obterPorNome(nome);
  }

  atualizarPosicao(request: AtualizarPosicaoRequest): Observable<AtualizarPosicaoResponse> {
    return this.porta.atualizarPosicao(request);
  }
}
