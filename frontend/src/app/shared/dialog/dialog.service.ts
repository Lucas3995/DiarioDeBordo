import {
  Injectable,
  Type,
  inject,
  ViewContainerRef,
  InjectionToken,
  Injector,
  ComponentRef,
  Provider,
} from '@angular/core';
import { take } from 'rxjs';
import { DialogRef } from './dialog-ref';

/** Token para fornecer o ViewContainerRef onde os dialogs serão ancorados. */
export const DIALOG_CONTAINER_REF = new InjectionToken<ViewContainerRef>('DIALOG_CONTAINER_REF');

export interface DialogOptions<R = unknown> {
  /** Dados a passar ao componente aberto (injetável). */
  data?: unknown;
  /** Providers adicionais que podem depender do DialogRef (ex.: estratégia de fechamento). */
  getProviders?: (ref: DialogRef<R>) => Provider[];
}

/** Estilos aplicados ao overlay do dialog (centralizados para manutenção). */
const OVERLAY_STYLES: Record<string, string> = {
  position: 'fixed',
  inset: '0',
  backgroundColor: 'rgba(0,0,0,0.5)',
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  zIndex: '1000',
};

/**
 * Serviço para abrir componentes em um overlay/modal.
 * Requer que DIALOG_CONTAINER_REF seja fornecido (ex.: por um componente host no template)
 * ou que setContainer() seja chamado pelo DialogHostComponent.
 * Dois caminhos de configuração (token vs setContainer) existem para permitir uso com ou sem host explícito.
 *
 * Nota de arquitetura: o overlay é criado via DOM direto (document.createElement, appendChild) para
 * manter dependências mínimas; @angular/cdk/overlay poderia ser adotado para alinhamento com o ecossistema Angular.
 */
@Injectable({ providedIn: 'root' })
export class DialogService {
  private readonly injectedContainerRef = inject(DIALOG_CONTAINER_REF, { optional: true });
  /** Preenchido por setContainer() quando não se usa DIALOG_CONTAINER_REF (ex.: DialogHost). */
  private dynamicContainerRef: ViewContainerRef | null = null;

  /** Regista o ViewContainerRef quando não se usa DIALOG_CONTAINER_REF (ex.: app com DialogHost). */
  setContainer(ref: ViewContainerRef): void {
    this.dynamicContainerRef = ref;
  }

  private get containerRef(): ViewContainerRef | null {
    return this.injectedContainerRef ?? this.dynamicContainerRef;
  }

  /**
   * Abre um dialog com o componente dado.
   * Retorna DialogRef para fechar e observar afterClosed().
   * Se o container não estiver configurado, em modo desenvolvimento é logado um erro e é retornado um DialogRef sem UI.
   */
  open<T, R = unknown>(component: Type<T>, options?: DialogOptions<R>): DialogRef<R> {
    const ref = new DialogRef<R>();
    const container = this.containerRef;
    if (!container) {
      if (typeof ngDevMode !== 'undefined' && ngDevMode) {
        console.error(
          'DialogService.open: container não configurado. Garanta que DIALOG_CONTAINER_REF está fornecido ou que DialogHostComponent está no template e setContainer() foi chamado.'
        );
      }
      return ref;
    }

    const overlay = this.criarOverlay();
    const parent = container.element.nativeElement.parentElement ?? container.element.nativeElement;
    parent.appendChild(overlay);

    const extraProviders = options?.getProviders?.(ref) ?? [];
    const injector = Injector.create({
      providers: [{ provide: DialogRef, useValue: ref }, ...extraProviders],
      parent: container.injector,
    });

    const componentRef = this.anexarComponente(container, component, injector, overlay);
    this.registarLimpezaAoFechar(ref, componentRef, overlay);

    return ref;
  }

  private criarOverlay(): HTMLDivElement {
    const overlay = document.createElement('div');
    overlay.setAttribute('data-testid', 'dialog-overlay');
    Object.assign(overlay.style, OVERLAY_STYLES);
    return overlay;
  }

  private anexarComponente<T>(
    container: ViewContainerRef,
    component: Type<T>,
    injector: Injector,
    overlay: HTMLDivElement
  ): ComponentRef<T> {
    const componentRef = container.createComponent(component, { injector });
    overlay.appendChild(componentRef.location.nativeElement);
    return componentRef;
  }

  private registarLimpezaAoFechar<T, R>(
    ref: DialogRef<R>,
    componentRef: ComponentRef<T>,
    overlay: HTMLDivElement
  ): void {
    ref.afterClosed.pipe(take(1)).subscribe(() => {
      componentRef.destroy();
      overlay.remove();
    });
  }
}
