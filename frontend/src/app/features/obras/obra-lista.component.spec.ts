import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';
import { ObraListaComponent } from './obra-lista.component';
import { ListaObrasService } from '../../application/lista-obras.service';
import { ListaObrasResult } from '../../domain/lista-obras.port';
import { TipoObra } from '../../domain/obra.types';
import { ObraListItem } from '../../domain/obra-list-item';
import { By } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { DialogService } from '../../shared/dialog/dialog.service';
import { DialogRef } from '../../shared/dialog/dialog-ref';
import { AtualizarPosicaoComponent } from './atualizar-posicao/atualizar-posicao.component';

const OBRAS_MOCK: ObraListItem[] = [
  {
    id: '1',
    nome: 'One Piece',
    tipo: TipoObra.Manga,
    ultimaAtualizacaoPosicao: new Date('2026-02-20'),
    posicaoAtual: 1110,
    ordemPreferencia: 1,
    proximaInfo: { tipo: 'dias_ate_proxima', dias: 5 },
  },
  {
    id: '2',
    nome: 'Solo Leveling',
    tipo: TipoObra.Manhwa,
    ultimaAtualizacaoPosicao: new Date('2026-02-15'),
    posicaoAtual: 179,
    ordemPreferencia: 2,
    proximaInfo: { tipo: 'partes_ja_publicadas', quantidade: 2 },
  },
  {
    id: '3',
    nome: 'Interstellar',
    tipo: TipoObra.Filme,
    ultimaAtualizacaoPosicao: new Date('2026-01-10'),
    posicaoAtual: 90,
    ordemPreferencia: 3,
  },
];

const RESULTADO_PAGINA_1: ListaObrasResult = { items: OBRAS_MOCK, totalCount: 25 };
const RESULTADO_VAZIO: ListaObrasResult = { items: [], totalCount: 0 };

describe('ObraListaComponent', () => {
  let fixture: ComponentFixture<ObraListaComponent>;
  let component: ObraListaComponent;
  let serviceSpy: jasmine.SpyObj<ListaObrasService>;
  let dialogServiceSpy: jasmine.SpyObj<DialogService>;

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj<ListaObrasService>('ListaObrasService', ['listarPagina']);
    serviceSpy.listarPagina.and.returnValue(of(RESULTADO_PAGINA_1));
    dialogServiceSpy = jasmine.createSpyObj<DialogService>('DialogService', ['open']);
    dialogServiceSpy.open.and.returnValue(new DialogRef());

    await TestBed.configureTestingModule({
      imports: [ObraListaComponent],
      providers: [
        { provide: ListaObrasService, useValue: serviceSpy },
        { provide: DialogService, useValue: dialogServiceSpy },
        provideRouter([]),
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ObraListaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve ser criado', () => {
    expect(component).toBeTruthy();
  });

  describe('inicialização', () => {
    it('deve chamar o serviço com pageIndex 0 e pageSize padrão (10) ao iniciar', () => {
      expect(serviceSpy.listarPagina).toHaveBeenCalledWith({ pageIndex: 0, pageSize: 10 });
    });

    it('deve armazenar os items retornados', () => {
      expect(component.obras).toEqual(OBRAS_MOCK);
    });

    it('deve armazenar o totalCount', () => {
      expect(component.totalCount).toBe(25);
    });
  });

  describe('exibição na tabela', () => {
    it('deve exibir o nome de cada obra', () => {
      const nomes = fixture.debugElement.queryAll(By.css('[data-testid="obra-nome"]'));
      expect(nomes.length).toBe(3);
      expect(nomes[0].nativeElement.textContent.trim()).toBe('One Piece');
      expect(nomes[1].nativeElement.textContent.trim()).toBe('Solo Leveling');
    });

    it('deve exibir o tipo de cada obra', () => {
      const tipos = fixture.debugElement.queryAll(By.css('[data-testid="obra-tipo"]'));
      expect(tipos.length).toBe(3);
      expect(tipos[0].nativeElement.textContent.trim()).toContain('manga');
    });

    it('deve exibir a posição formatada com unidade do tipo (ex.: "capítulo 1110")', () => {
      const posicoes = fixture.debugElement.queryAll(By.css('[data-testid="obra-posicao"]'));
      expect(posicoes[0].nativeElement.textContent.trim()).toBe('capítulo 1110');
      expect(posicoes[2].nativeElement.textContent.trim()).toBe('minuto 90');
    });

    it('deve exibir última atualização (formato relativo)', () => {
      const datas = fixture.debugElement.queryAll(By.css('[data-testid="obra-ultima-atualizacao"]'));
      expect(datas.length).toBe(3);
      expect(datas[0].nativeElement.textContent.trim()).toBeTruthy();
    });

    it('deve exibir a coluna Previsão com "X dias" quando tipo é dias_ate_proxima', () => {
      const previsoes = fixture.debugElement.queryAll(By.css('[data-testid="obra-previsao"]'));
      expect(previsoes[0].nativeElement.textContent.trim()).toContain('5');
    });

    it('deve exibir a coluna Previsão com partes publicadas quando tipo é partes_ja_publicadas', () => {
      const previsoes = fixture.debugElement.queryAll(By.css('[data-testid="obra-previsao"]'));
      expect(previsoes[1].nativeElement.textContent.trim()).toContain('2');
    });

    it('deve exibir célula Previsão vazia quando não há proximaInfo', () => {
      const previsoes = fixture.debugElement.queryAll(By.css('[data-testid="obra-previsao"]'));
      expect(previsoes[2].nativeElement.textContent.trim()).toBe('');
    });

    it('deve exibir botão "Ver mais" para cada obra', () => {
      const botoes = fixture.debugElement.queryAll(By.css('[data-testid="obra-ver-mais"]'));
      expect(botoes.length).toBe(3);
    });

    it('deve exibir botão/link "Atualizar posição" com data-testid para acessibilidade', () => {
      const btn = fixture.debugElement.query(By.css('[data-testid="link-atualizar-posicao"]'));
      expect(btn).toBeTruthy();
      expect(btn.nativeElement.textContent?.trim()).toContain('Atualizar posição');
    });

    it('ao clicar em Atualizar posição deve abrir o dialog com o componente de atualização (demanda 1 — popup)', () => {
      const btn = fixture.debugElement.query(By.css('[data-testid="link-atualizar-posicao"]'));
      btn.nativeElement.click();
      fixture.detectChanges();
      expect(dialogServiceSpy.open).toHaveBeenCalledWith(AtualizarPosicaoComponent, jasmine.anything());
    });
  });

  describe('paginação', () => {
    it('deve exibir o seletor de pageSize', () => {
      const seletor = fixture.debugElement.query(By.css('[data-testid="page-size-select"]'));
      expect(seletor).toBeTruthy();
    });

    it('deve ter opções de pageSize 10, 25, 50 e 100', () => {
      const opcoes = fixture.debugElement.queryAll(By.css('[data-testid="page-size-option"]'));
      const valores = opcoes.map((o) => +o.nativeElement.value);
      expect(valores).toContain(10);
      expect(valores).toContain(25);
      expect(valores).toContain(50);
      expect(valores).toContain(100);
    });

    it('deve exibir informação de total de registros', () => {
      const info = fixture.debugElement.query(By.css('[data-testid="paginacao-total"]'));
      expect(info).toBeTruthy();
      expect(info.nativeElement.textContent).toContain('25');
    });

    it('deve exibir botões de próxima e anterior', () => {
      const anterior = fixture.debugElement.query(By.css('[data-testid="btn-anterior"]'));
      const proximo = fixture.debugElement.query(By.css('[data-testid="btn-proximo"]'));
      expect(anterior).toBeTruthy();
      expect(proximo).toBeTruthy();
    });
  });

  describe('mudança de página', () => {
    it('deve chamar o serviço com pageIndex 1 ao avançar para a próxima página', () => {
      serviceSpy.listarPagina.calls.reset();
      component.irParaProximaPagina();
      expect(serviceSpy.listarPagina).toHaveBeenCalledWith({ pageIndex: 1, pageSize: 10 });
    });

    it('não deve ir para página anterior quando já está na página 0', () => {
      serviceSpy.listarPagina.calls.reset();
      component.irParaPaginaAnterior();
      expect(serviceSpy.listarPagina).not.toHaveBeenCalled();
    });

    it('deve ir para a página anterior quando está em página > 0', () => {
      component.irParaProximaPagina();
      serviceSpy.listarPagina.calls.reset();
      component.irParaPaginaAnterior();
      expect(serviceSpy.listarPagina).toHaveBeenCalledWith({ pageIndex: 0, pageSize: 10 });
    });
  });

  describe('mudança de pageSize', () => {
    it('deve reiniciar para a página 0 ao mudar pageSize', () => {
      component.irParaProximaPagina();
      serviceSpy.listarPagina.calls.reset();
      component.mudarPageSize(25);
      expect(serviceSpy.listarPagina).toHaveBeenCalledWith({ pageIndex: 0, pageSize: 25 });
    });

    it('deve atualizar o pageSize atual', () => {
      component.mudarPageSize(50);
      expect(component.pageSize).toBe(50);
    });
  });

  describe('lista vazia', () => {
    beforeEach(() => {
      serviceSpy.listarPagina.and.returnValue(of(RESULTADO_VAZIO));
      component.carregarPagina();
      fixture.detectChanges();
    });

    it('deve exibir mensagem quando a lista está vazia', () => {
      const msg = fixture.debugElement.query(By.css('[data-testid="lista-vazia"]'));
      expect(msg).toBeTruthy();
    });

    it('não deve exibir a tabela quando a lista está vazia', () => {
      const tabela = fixture.debugElement.query(By.css('[data-testid="obras-tabela"]'));
      expect(tabela).toBeNull();
    });
  });

  describe('dialog Atualizar posição (demanda 1)', () => {
    it('ao fechar o dialog com sucesso deve recarregar a listagem (carregarPagina)', () => {
      const dialogRef = new DialogRef<{ salvou?: boolean }>() as DialogRef;
      dialogServiceSpy.open.and.returnValue(dialogRef);
      serviceSpy.listarPagina.calls.reset();

      const btn = fixture.debugElement.query(By.css('[data-testid="link-atualizar-posicao"]'));
      btn.nativeElement.click();
      fixture.detectChanges();
      expect(dialogServiceSpy.open).toHaveBeenCalled();

      dialogRef.close({ salvou: true });
      fixture.detectChanges();

      expect(serviceSpy.listarPagina).toHaveBeenCalledWith({
        pageIndex: component.pageIndex,
        pageSize: component.pageSize,
      });
    });
  });
});
