import { InjectionToken } from '@angular/core';

/** Atraso em ms antes de fechar o dialog ou navegar após sucesso ao salvar. */
export const DELAY_FECHAMENTO_APOS_SUCESSO_MS = 1500;

/** Contrato para "sair" após sucesso (fechar dialog ou navegar). */
export interface SaidaAposSucesso {
  fecharComSucesso(): void;
}

export const SAIDA_APOS_SUCESSO = new InjectionToken<SaidaAposSucesso>('SAIDA_APOS_SUCESSO');
