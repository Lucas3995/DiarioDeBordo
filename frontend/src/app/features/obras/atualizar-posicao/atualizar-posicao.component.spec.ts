import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { AtualizarPosicaoComponent } from './atualizar-posicao.component';
import { AtualizarPosicaoService, ObraDetalhe } from '../../../application/atualizar-posicao.service';
import { TipoObra } from '../../../domain/obra.types';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';

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
  let router: Router;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj<AtualizarPosicaoService>('AtualizarPosicaoService', [
      'obterPorId',
      'obterPorNome',
      'atualizarPosicao',
    ]);

    await TestBed.configureTestingModule({
      imports: [AtualizarPosicaoComponent],
      providers: [
        { provide: AtualizarPosicaoService, useValue: serviceSpy },
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

    it('quando API retorna 404 e criarSeNaoExistir é true deve definir prévia sintética e não exibir erro de obra não encontrada (Demanda 5 — item 2)', () => {
      serviceSpy.obterPorNome.and.returnValue(throwError(() => ({ status: 404 })));
      component.tipoIdentificador = 'nome';
      component.valorIdentificador = 'Obra Inexistente';
      component.criarSeNaoExistir = true;
      component.tipoParaCriar = TipoObra.Manga;
      component.novaPosicao = 5;
      component.nomeParaCriar = 'Obra Inexistente';

      component.verPreview();

      expect(component.erro).toBeNull();
      expect(component.preview).not.toBeNull();
      expect(component.preview!.tipo).toBe(TipoObra.Manga);
      expect(component.preview!.nome).toBe(component.valorIdentificador.trim());
      expect((component.preview as ObraDetalheComObraNova).obraNova).toBe(true);
    });

    it('quando API retorna 404 e criarSeNaoExistir é false deve definir erro', () => {
      serviceSpy.obterPorNome.and.returnValue(throwError(() => ({ status: 404 })));
      component.tipoIdentificador = 'nome';
      component.valorIdentificador = 'Qualquer';
      component.criarSeNaoExistir = false;

      component.verPreview();

      expect(component.erro).toContain('Obra não encontrada');
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
});
