import { Component, ViewChild, ViewContainerRef } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DialogService, DIALOG_CONTAINER_REF } from './dialog.service';
import { DialogRef } from './dialog-ref';

/** Componente mínimo para testar abertura no dialog. */
@Component({
  standalone: true,
  template: '<div data-testid="test-dialog-content">Conteúdo do dialog</div>',
})
class TestDialogContentComponent {}

/** Wrapper que fornece ViewContainerRef para o DialogService. */
@Component({
  standalone: true,
  template: '<ng-container #dialogHost></ng-container>',
})
class DialogHostWrapperComponent {
  @ViewChild('dialogHost', { read: ViewContainerRef }) vcr!: ViewContainerRef;
}

describe('DialogService', () => {
  let service: DialogService;
  let hostFixture: ComponentFixture<DialogHostWrapperComponent>;
  let containerRef: ViewContainerRef;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DialogHostWrapperComponent],
      providers: [
        DialogService,
        { provide: DIALOG_CONTAINER_REF, useFactory: () => containerRef },
      ],
    }).compileComponents();

    hostFixture = TestBed.createComponent(DialogHostWrapperComponent);
    hostFixture.detectChanges();
    containerRef = hostFixture.componentInstance.vcr;
    service = TestBed.inject(DialogService);
  });

  describe('contrato do retorno', () => {
    it('deve retornar DialogRef com close() e afterClosed() ao abrir', () => {
      const ref = service.open(TestDialogContentComponent);
      expect(ref).toBeInstanceOf(DialogRef);
      expect(typeof ref.close).toBe('function');
      expect(ref.afterClosed).toBeDefined();
    });

    it('afterClosed deve emitir o resultado quando close(result) é chamado', (done) => {
      const ref = service.open<TestDialogContentComponent, string>(TestDialogContentComponent);
      ref.afterClosed.subscribe((result: string | undefined) => {
        expect(result).toBe('sucesso');
        done();
      });
      ref.close('sucesso');
    });
  });

  describe('renderização (com container fornecido)', () => {
    it('deve exibir overlay no DOM após open()', () => {
      service.open(TestDialogContentComponent);
      hostFixture.detectChanges();
      const overlay = hostFixture.nativeElement.querySelector('[data-testid="dialog-overlay"]');
      expect(overlay).toBeTruthy();
    });

    it('deve exibir o conteúdo do componente no DOM após open()', () => {
      service.open(TestDialogContentComponent);
      hostFixture.detectChanges();
      const content = hostFixture.nativeElement.querySelector('[data-testid="test-dialog-content"]');
      expect(content).toBeTruthy();
      expect(content?.textContent?.trim()).toContain('Conteúdo do dialog');
    });

    it('deve remover o overlay do DOM ao chamar close()', () => {
      const ref = service.open(TestDialogContentComponent);
      hostFixture.detectChanges();
      expect(hostFixture.nativeElement.querySelector('[data-testid="dialog-overlay"]')).toBeTruthy();
      ref.close();
      hostFixture.detectChanges();
      expect(hostFixture.nativeElement.querySelector('[data-testid="dialog-overlay"]')).toBeFalsy();
    });

    it('fechar ao clicar no backdrop', () => {
      service.open(TestDialogContentComponent);
      hostFixture.detectChanges();
      const overlay = hostFixture.nativeElement.querySelector('[data-testid="dialog-overlay"]') as HTMLElement;
      overlay.click();
      hostFixture.detectChanges();
      expect(hostFixture.nativeElement.querySelector('[data-testid="dialog-overlay"]')).toBeFalsy();
    });

    it('fechar ao pressionar ESC', () => {
      service.open(TestDialogContentComponent);
      hostFixture.detectChanges();
      const esc = new KeyboardEvent('keydown', { key: 'Escape' });
      document.dispatchEvent(esc);
      hostFixture.detectChanges();
      expect(hostFixture.nativeElement.querySelector('[data-testid="dialog-overlay"]')).toBeFalsy();
    });
  });
});

describe('DialogService sem container', () => {
  let service: DialogService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DialogService],
    });
    service = TestBed.inject(DialogService);
  });

  it('fallback deve criar overlay no body e retornar DialogRef válido', () => {
    const ref = service.open(TestDialogContentComponent);
    expect(ref).toBeInstanceOf(DialogRef);

    // overlay é anexado ao body quando não existe container Angular
    const overlay = document.body.querySelector('[data-testid="dialog-overlay"]');
    expect(overlay).toBeTruthy();

    ref.close();
    expect(document.body.querySelector('[data-testid="dialog-overlay"]')).toBeFalsy();
  });

  it('fechar por clique no backdrop e ESC no fallback', () => {
    // clique no vento
    service.open(TestDialogContentComponent);
    let overlay = document.body.querySelector('[data-testid="dialog-overlay"]') as HTMLElement;
    expect(overlay).toBeTruthy();
    overlay.click();
    expect(document.body.querySelector('[data-testid="dialog-overlay"]')).toBeFalsy();

    // ESC também deve funcionar
    service.open(TestDialogContentComponent);
    const esc = new KeyboardEvent('keydown', { key: 'Escape' });
    document.dispatchEvent(esc);
    expect(document.body.querySelector('[data-testid="dialog-overlay"]')).toBeFalsy();
  });
});
