import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AtualizarPosicaoComponent } from './atualizar-posicao.component';
import { AtualizarPosicaoService, ObraDetalhe } from '../../../application/atualizar-posicao.service';
import { TipoObra } from '../../../domain/obra.types';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';
import { DialogRef } from '../../../shared/dialog/dialog-ref';
import { DialogService } from '../../../shared/dialog/dialog.service';
import { DELAY_FECHAMENTO_APOS_SUCESSO_MS } from '../../../shared/dialog/saida-apos-sucesso';
import { PromptObraNovaComponent, PromptObraNovaResult } from './prompt-obra-nova.component';

/** Prévia sintética marcada como obra nova (Demanda 5 — item 4 opcional). */
type ObraDetalheComObraNova = ObraDetalhe & { obraNova?: boolean };

const OBRA_PREVIEW_MOCK: ObraDetalhe = {
  id: 'id-1',
  nome: 'One Piece',
  tipo: TipoObra.Manga,
  posicaoAtual: 1110,
  dataUltimaAtualizacaoPosicao: '2026-02-20T00:00:00Z',
  ordemPreferencia: 1,
};

describe('AtualizarPosicaoComponent', () => {
  let fixture: ComponentFixture<AtualizarPosicaoComponent>;
  let component: AtualizarPosicaoComponent;
  let serviceSpy: jasmine.SpyObj<AtualizarPosicaoService>;
  let dialogServiceSpy: jasmine.SpyObj<DialogService>;
  let router: Router;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj<AtualizarPosicaoService>('AtualizarPosicaoService', [
      'obterPorId',
      'obterPorNome',
      'atualizarPosicao',
    ]);
    dialogServiceSpy = jasmine.createSpyObj<DialogService>('DialogService', ['open']);

    await TestBed.configureTestingModule({
      imports: [AtualizarPosicaoComponent],
      providers: [
        { provide: AtualizarPosicaoService, useValue: serviceSpy },
        { provide: DialogService, useValue: dialogServiceSpy },
        provideRouter([]),
      ],
    }).compileComponents();

    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.stub();

    fixture = TestBed.createComponent(AtualizarPosicaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  describe('template — Demanda 2 item 1 (sem checkbox e sem bloco Dados da nova obra)', () => {
    it('não deve exibir o checkbox "Criar obra se não existir"', () => {
      fixture.detectChanges();
      const checkbox = fixture.debugElement.query(By.css('[data-testid="atualizar-posicao-criar"]'));
      expect(checkbox).toBeFalsy();
    });

    it('não deve exibir a secção "Dados da nova obra" (campos nome/tipo/ordem no form principal)', () => {
      fixture.detectChanges();
      expect(fixture.nativeElement.textContent).not.toMatch(/Dados da nova obra/i);
      expect(fixture.debugElement.query(By.css('[data-testid="atualizar-posicao-nome-criar"]'))).toBeFalsy();
    });
  });

  describe('verPreview()', () => {
    it('com identificador vazio deve definir erro e não chamar o serviço', () => {
      component.valorIdentificador = '   ';
      component.verPreview();

      expect(serviceSpy.obterPorId).not.toHaveBeenCalled();
      expect(serviceSpy.obterPorNome).not.toHaveBeenCalled();
      expect(component.erro).toBe('Informe o código (id) ou o nome da obra.');
    });

    it('por nome com obra existente deve atribuir a prévia e limpar erro', () => {
      serviceSpy.obterPorNome.and.returnValue(of(OBRA_PREVIEW_MOCK));
      component.tipoIdentificador = 'nome';
      component.valorIdentificador = 'One Piece';
      component.verPreview();

      expect(serviceSpy.obterPorNome).toHaveBeenCalledWith('One Piece');
      expect(component.preview).toEqual(OBRA_PREVIEW_MOCK);
      expect(component.erro).toBeNull();
    });

    it('quando API retorna 404 deve abrir o prompt de obra nova (Demanda 2 — item 3)', () => {
      const resultadoProsseguir: PromptObraNovaResult = {
        prosseguir: true,
        nome: 'Obra Inexistente',
        tipo: TipoObra.Manga,
        ordemPreferencia: 0,
      };
      const dialogRefStub = { afterClosed: of(resultadoProsseguir) };
      dialogServiceSpy.open.and.returnValue(dialogRefStub as unknown as DialogRef<unknown>);

      serviceSpy.obterPorNome.and.returnValue(throwError(() => ({ status: 404 })));
      component.tipoIdentificador = 'nome';
      component.valorIdentificador = 'Obra Inexistente';
      component.novaPosicao = 5;

      component.verPreview();

      expect(dialogServiceSpy.open).toHaveBeenCalledWith(PromptObraNovaComponent, jasmine.objectContaining({
        data: jasmine.objectContaining({ nomeDefault: 'Obra Inexistente' }),
      }));
      expect(component.erro).toBeNull();
      expect(component.preview).not.toBeNull();
      expect(component.preview!.tipo).toBe(TipoObra.Manga);
      expect(component.preview!.nome).toBe('Obra Inexistente');
      expect((component.preview as ObraDetalheComObraNova).obraNova).toBe(true);
    });

    it('quando API retorna 404 e usuário cancela o prompt deve exibir mensagem informativa (Demanda 2 — item 3)', () => {
      dialogServiceSpy.open.and.returnValue({
        afterClosed: of(undefined),
      } as unknown as DialogRef<unknown>);

      serviceSpy.obterPorNome.and.returnValue(throwError(() => ({ status: 404 })));
      component.tipoIdentificador = 'nome';
      component.valorIdentificador = 'Qualquer';

      component.verPreview();

      expect(dialogServiceSpy.open).toHaveBeenCalledWith(PromptObraNovaComponent, jasmine.any(Object));
      expect(component.erro).toMatch(/Obra não encontrada/i);
      expect(component.preview).toBeNull();
    });
  });

  describe('template — prévia obra nova (Demanda 5 — item 3)', () => {
    it('quando preview é de obra nova deve exibir "Antes: — (obra nova)" em vez de posição/data', () => {
      const previewObraNova: ObraDetalheComObraNova = {
        ...OBRA_PREVIEW_MOCK,
        obraNova: true,
      };
      component.preview = previewObraNova;
      component.novaPosicao = 10;
      fixture.detectChanges();

      const previewEl = fixture.debugElement.query(By.css('[data-testid="atualizar-posicao-preview"]'));
      expect(previewEl).toBeTruthy();
      const text = previewEl.nativeElement.textContent;
      expect(text).toMatch(/Antes:.*—.*obra nova/i);
    });
  });

  describe('salvar() — Demanda 2 itens 4 e 5', () => {
    it('quando já existe estado de dados de criação (preenchido pelo prompt) deve enviar request com criarSeNaoExistir true e nome/tipo/ordem', () => {
      serviceSpy.atualizarPosicao.and.returnValue(of({ id: 'id-1', criada: true }));
      component.valorIdentificador = 'Obra Nova';
      component.novaPosicao = 1;
      component.nomeParaCriar = 'Obra Nova';
      component.tipoParaCriar = TipoObra.Manga;
      component.ordemPreferenciaParaCriar = 0;

      component.salvar();

      expect(serviceSpy.atualizarPosicao).toHaveBeenCalledWith(
        jasmine.objectContaining({
          criarSeNaoExistir: true,
          nomeParaCriar: 'Obra Nova',
          tipoParaCriar: TipoObra.Manga,
          ordemPreferenciaParaCriar: 0,
        })
      );
    });

    it('quando PATCH retorna 404 deve abrir prompt de obra nova; ao prosseguir deve reenviar com criarSeNaoExistir true (Demanda 2 — item 4)', () => {
      const resultadoPrompt: PromptObraNovaResult = {
        prosseguir: true,
        nome: 'Obra Cadastrar',
        tipo: TipoObra.Livro,
        ordemPreferencia: 2,
      };
      dialogServiceSpy.open.and.returnValue({
        afterClosed: of(resultadoPrompt),
      } as unknown as DialogRef<unknown>);

      serviceSpy.atualizarPosicao.and.returnValues(
        throwError(() => ({ status: 404 })),
        of({ id: 'id-novo', criada: true })
      );

      component.valorIdentificador = 'Obra Inexistente';
      component.novaPosicao = 10;
      component.salvar();

      expect(dialogServiceSpy.open).toHaveBeenCalledWith(PromptObraNovaComponent, jasmine.any(Object));
      expect(serviceSpy.atualizarPosicao).toHaveBeenCalledTimes(2);
      const segundoRequest = serviceSpy.atualizarPosicao.calls.argsFor(1)[0];
      expect(segundoRequest.criarSeNaoExistir).toBe(true);
      expect(segundoRequest.nomeParaCriar).toBe('Obra Cadastrar');
      expect(segundoRequest.tipoParaCriar).toBe(TipoObra.Livro);
      expect(segundoRequest.ordemPreferenciaParaCriar).toBe(2);
    });
  });
});

describe('AtualizarPosicaoComponent uso em popup (demanda 1 — DialogRef injetado)', () => {
  let fixture: ComponentFixture<AtualizarPosicaoComponent>;
  let component: AtualizarPosicaoComponent;
  let serviceSpy: jasmine.SpyObj<AtualizarPosicaoService>;
  let dialogRefSpy: jasmine.SpyObj<DialogRef<{ salvou: boolean }>>;
  let dialogServiceSpy: jasmine.SpyObj<DialogService>;
  let router: Router;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj<AtualizarPosicaoService>('AtualizarPosicaoService', [
      'obterPorId',
      'obterPorNome',
      'atualizarPosicao',
    ]);
    dialogRefSpy = jasmine.createSpyObj<DialogRef<{ salvou: boolean }>>('DialogRef', ['close']);
    dialogServiceSpy = jasmine.createSpyObj<DialogService>('DialogService', ['open']);
    await TestBed.configureTestingModule({
      imports: [AtualizarPosicaoComponent],
      providers: [
        { provide: AtualizarPosicaoService, useValue: serviceSpy },
        { provide: DialogRef, useValue: dialogRefSpy },
        { provide: DialogService, useValue: dialogServiceSpy },
        provideRouter([]),
      ],
    }).compileComponents();
    router = TestBed.inject(Router);
    spyOn(router, 'navigate').and.stub();
    fixture = TestBed.createComponent(AtualizarPosicaoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('ao salvar com sucesso deve chamar dialogRef.close({ salvou: true }) e não navegar', fakeAsync(() => {
    serviceSpy.atualizarPosicao.and.returnValue(
      of({ id: 'id-1', criada: false })
    );
    component.valorIdentificador = 'One Piece';
    component.novaPosicao = 1111;
    component.salvar();

    tick(DELAY_FECHAMENTO_APOS_SUCESSO_MS);

    expect(dialogRefSpy.close).toHaveBeenCalledWith({ salvou: true });
    expect(router.navigate).not.toHaveBeenCalled();
  }));

  it('deve exibir botão Fechar/Cancelar que chama dialogRef.close() sem resultado (demanda 1)', () => {
    const btnFechar = fixture.debugElement.query(
      By.css('[data-testid="atualizar-posicao-fechar"]')
    );
    expect(btnFechar).toBeTruthy();
    btnFechar.nativeElement.click();
    expect(dialogRefSpy.close).toHaveBeenCalledWith(undefined);
  });
});
