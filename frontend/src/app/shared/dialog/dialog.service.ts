import {
  Injectable,
  Type,
  inject,
  ViewContainerRef,
  InjectionToken,
  Injector,
  ComponentRef,
  Provider,
  ApplicationRef,
} from '@angular/core';
import { take } from 'rxjs';
import { DialogRef } from './dialog-ref';

/** Token para fornecer o ViewContainerRef onde os dialogs serão ancorados. */
export const DIALOG_CONTAINER_REF = new InjectionToken<ViewContainerRef>('DIALOG_CONTAINER_REF');

/** Token para injetar os dados passados em DialogOptions.data ao abrir o dialog. */
export const DIALOG_DATA = new InjectionToken<unknown>('DIALOG_DATA');

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
  private readonly appRef = inject(ApplicationRef);
  private readonly appInjector = inject(Injector);

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
   *
   * O serviço usa um `ViewContainerRef` fornecido via token ou `setContainer`
   * para injetar o componente na árvore Angular. Se nenhum container estiver
   * disponível, criamos um overlay no `document.body` e geramos o componente
   * usando `ApplicationRef` como fallback; nenhum
   * erro é lançado e o dialog continua funcionando. Esse caminho garante que
   * `open()` nunca falhe mesmo que a aplicação esqueça de configurar um host.
   */
  open<T, R = unknown>(component: Type<T>, options?: DialogOptions<R>): DialogRef<R> {
    const ref = new DialogRef<R>();
    const container = this.containerRef;

    const overlay = this.criarOverlay();
    const parentElement = container
      ? container.element.nativeElement.parentElement ?? container.element.nativeElement
      : document.body;
    parentElement.appendChild(overlay);

    const providers: Provider[] = [
      { provide: DialogRef, useValue: ref },
      ...(options?.getProviders?.(ref) ?? []),
    ];
    if (options?.data !== undefined) {
      providers.push({ provide: DIALOG_DATA, useValue: options.data });
    }

    const injector = Injector.create({
      providers,
      parent: container?.injector ?? this.appInjector,
    });

    let componentRef: ComponentRef<T>;
    if (container) {
      componentRef = this.anexarComponente(container, component, injector, overlay);
    } else {
      // fallback: bootstrap o componente diretamente no elemento de overlay.
      // `bootstrap` adiciona o componente ao ApplicationRef e insere sua
      // raiz no nó fornecido, facilitando o uso sem ViewContainerRef.
      componentRef = this.appRef.bootstrap(component, overlay);
    }

    // event listeners para fechamento via backdrop e tecla ESC
    const backdropClick = (ev: MouseEvent) => {
      if (ev.target === overlay) {
        ref.close();
      }
    };
    const escKey = (ev: KeyboardEvent) => {
      if (ev.key === 'Escape' || ev.key === 'Esc') {
        ref.close();
      }
    };
    overlay.addEventListener('click', backdropClick);
    document.addEventListener('keydown', escKey);

    this.registarLimpezaAoFechar(ref, componentRef, overlay, () => {
      overlay.removeEventListener('click', backdropClick);
      document.removeEventListener('keydown', escKey);
    });
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
    overlay: HTMLDivElement,
    cleanup?: () => void
  ): void {
    ref.afterClosed.pipe(take(1)).subscribe(() => {
      componentRef.destroy();
      overlay.remove();
      if (cleanup) cleanup();
    });
  }
}
