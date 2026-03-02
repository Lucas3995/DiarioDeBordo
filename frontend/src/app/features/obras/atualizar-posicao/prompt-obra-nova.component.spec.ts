import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DialogRef } from '../../../shared/dialog/dialog-ref';
import { TipoObra } from '../../../domain/obra.types';
import { PromptObraNovaComponent, PromptObraNovaResult } from './prompt-obra-nova.component';

describe('PromptObraNovaComponent (Demanda 2 — prompt obra nova)', () => {
  let fixture: ComponentFixture<PromptObraNovaComponent>;
  let component: PromptObraNovaComponent;
  let dialogRefSpy: jasmine.SpyObj<DialogRef<PromptObraNovaResult | undefined>>;

  beforeEach(async () => {
    dialogRefSpy = jasmine.createSpyObj<DialogRef<PromptObraNovaResult | undefined>>('DialogRef', ['close']);
    await TestBed.configureTestingModule({
      imports: [PromptObraNovaComponent],
      providers: [{ provide: DialogRef, useValue: dialogRefSpy }],
    }).compileComponents();

    fixture = TestBed.createComponent(PromptObraNovaComponent);
    component = fixture.componentInstance;
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  describe('dados iniciais (item 2)', () => {
    it('deve receber nome default via data e exibir no campo nome', () => {
      const nomeDefault = 'Obra Inexistente';
      component.data = { nomeDefault };
      fixture.detectChanges();

      const inputNome = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-nome"]'));
      expect(inputNome).toBeTruthy();
      expect(component.nome).toBe(nomeDefault);
      // Valor no modelo é refletido no input via ngModel (verificado por component.nome)
    });

    it('deve exibir seletor de tipo (TipoObra)', () => {
      fixture.detectChanges();
      const selectTipo = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-tipo"]'));
      expect(selectTipo).toBeTruthy();
    });

    it('deve exibir campo ordem de preferência com valor default 0', () => {
      fixture.detectChanges();
      const inputOrdem = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-ordem"]'));
      expect(inputOrdem).toBeTruthy();
      expect(component.ordemPreferencia).toBe(0);
    });
  });

  describe('botões (item 2)', () => {
    it('deve exibir botão Prosseguir', () => {
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-prosseguir"]'));
      expect(btn).toBeTruthy();
    });

    it('deve exibir botão Cancelar', () => {
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-cancelar"]'));
      expect(btn).toBeTruthy();
    });
  });

  describe('ao Prosseguir (item 2)', () => {
    it('deve chamar dialogRef.close com resultado prosseguir: true, nome, tipo e ordemPreferencia', () => {
      component.nome = 'Nova Obra';
      component.tipo = TipoObra.Manga;
      component.ordemPreferencia = 1;
      fixture.detectChanges();

      const btn = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-prosseguir"]'));
      btn.nativeElement.click();

      expect(dialogRefSpy.close).toHaveBeenCalledWith({
        prosseguir: true,
        nome: 'Nova Obra',
        tipo: TipoObra.Manga,
        ordemPreferencia: 1,
      });
    });
  });

  describe('ao Cancelar (item 2)', () => {
    it('deve chamar dialogRef.close com undefined', () => {
      fixture.detectChanges();
      const btn = fixture.debugElement.query(By.css('[data-testid="prompt-obra-nova-cancelar"]'));
      btn.nativeElement.click();

      expect(dialogRefSpy.close).toHaveBeenCalledWith(undefined);
    });
  });
});
