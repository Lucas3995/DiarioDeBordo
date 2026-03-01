import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

/**
 * Referência a um dialog aberto. Permite fechar com resultado e observar o resultado.
 */
@Injectable()
export class DialogRef<T = unknown> {
  private readonly afterClosedSubject = new Subject<T | undefined>();

  /** Observable que emite o resultado quando o dialog é fechado. */
  readonly afterClosed: Observable<T | undefined> = this.afterClosedSubject.asObservable();

  /** Fecha o dialog e opcionalmente envia o resultado. */
  close(result?: T): void {
    this.afterClosedSubject.next(result);
    this.afterClosedSubject.complete();
  }
}
