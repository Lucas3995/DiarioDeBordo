import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { InputAutocompleteComponent } from './input-autocomplete.component';
import { SimpleChange } from '@angular/core';

type ItemMock = { id: string; nome: string };

describe('InputAutocompleteComponent', () => {
  let fixture: ComponentFixture<InputAutocompleteComponent<ItemMock>>;
  let component: InputAutocompleteComponent<ItemMock>;

  const SUGESTOES: ItemMock[] = [
    { id: '1', nome: 'One Piece' },
    { id: '2', nome: 'Attack on Titan' },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InputAutocompleteComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(InputAutocompleteComponent<ItemMock>);
    component = fixture.componentInstance;
    component.suggestions = SUGESTOES;
    component.displayWith = (item) => item.nome;
    component.getItemId = (item) => item.id;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  describe('suggestions atualizadas (lista abre ao receber resultados)', () => {
    it('deve abrir a lista quando suggestions passam a ter itens (ex.: resposta da API)', () => {
      component.suggestions = [];
      fixture.detectChanges();
      expect(component.listVisible).toBeFalse();
      component.suggestions = SUGESTOES;
      component.ngOnChanges({
        suggestions: new SimpleChange([], SUGESTOES, false),
      });
      expect(component.listVisible).toBeTrue();
      expect(component.activeOptionId).toBeNull();
    });

    it('deve fechar a lista quando suggestions ficam vazias', () => {
      component.suggestions = SUGESTOES;
      component.listVisible = true;
      component.ngOnChanges({
        suggestions: new SimpleChange(SUGESTOES, [], false),
      });
      expect(component.listVisible).toBeFalse();
    });
  });

  describe('valueChange e searchTerm', () => {
    it('deve emitir valueChange ao digitar', () => {
      let valorEmitido: string | undefined;
      component.valueChange.subscribe((v) => (valorEmitido = v));
      component.onInput('One');
      expect(valorEmitido).toBe('One');
    });

    it('deve emitir searchTerm após debounce quando o utilizador digita', fakeAsync(() => {
      const emitidos: string[] = [];
      component.searchTerm.subscribe((termo) => emitidos.push(termo));
      component.onInput('O');
      component.onInput('On');
      component.onInput('One');
      expect(emitidos.length).toBe(0);
      tick(350);
      expect(emitidos).toEqual(['One']);
    }));

    it('não deve emitir searchTerm para o mesmo valor (distinctUntilChanged)', fakeAsync(() => {
      const emitidos: string[] = [];
      component.searchTerm.subscribe((termo) => emitidos.push(termo));
      component.onInput('One');
      tick(350);
      component.onInput('One');
      tick(350);
      expect(emitidos).toEqual(['One']);
    }));
  });

  describe('suggestionSelected', () => {
    it('deve emitir suggestionSelected ao chamar select()', () => {
      let itemEmitido: ItemMock | undefined;
      component.suggestionSelected.subscribe((item) => (itemEmitido = item as ItemMock));
      component.listVisible = true;
      component.select(SUGESTOES[0]);
      expect(itemEmitido).toEqual(SUGESTOES[0]);
      expect(component.listVisible).toBeFalse();
    });

    it('deve emitir suggestionSelected ao clicar numa opção (mousedown)', () => {
      let itemEmitido: ItemMock | undefined;
      component.suggestionSelected.subscribe((item) => (itemEmitido = item as ItemMock));
      component.listVisible = true;
      component.suggestions = SUGESTOES;
      fixture.detectChanges();
      const opcoes = fixture.debugElement.queryAll(By.css('[role="option"]'));
      opcoes[1].nativeElement.dispatchEvent(new Event('mousedown', { bubbles: true }));
      expect(itemEmitido).toEqual(SUGESTOES[1]);
    });
  });

  describe('teclado', () => {
    beforeEach(() => {
      component.listVisible = true;
      component.suggestions = SUGESTOES;
      fixture.detectChanges();
    });

    it('ArrowDown deve avançar o índice ativo', () => {
      const eventDown = new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true });
      spyOn(eventDown, 'preventDefault');
      component.onKeydown(eventDown);
      expect(component.activeOptionId).toBe(component.optionId(SUGESTOES[0]));
      component.onKeydown(eventDown);
      expect(component.activeOptionId).toBe(component.optionId(SUGESTOES[1]));
      expect(eventDown.preventDefault).toHaveBeenCalled();
    });

    it('ArrowUp deve retroceder o índice ativo', () => {
      component.onKeydown(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
      component.onKeydown(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
      expect(component.activeOptionId).toBe(component.optionId(SUGESTOES[1]));
      const eventUp = new KeyboardEvent('keydown', { key: 'ArrowUp', bubbles: true });
      spyOn(eventUp, 'preventDefault');
      component.onKeydown(eventUp);
      expect(component.activeOptionId).toBe(component.optionId(SUGESTOES[0]));
      expect(eventUp.preventDefault).toHaveBeenCalled();
    });

    it('Enter com opção ativa deve selecionar e emitir suggestionSelected', () => {
      component.onKeydown(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
      let itemEmitido: ItemMock | undefined;
      component.suggestionSelected.subscribe((item) => (itemEmitido = item as ItemMock));
      const eventEnter = new KeyboardEvent('keydown', { key: 'Enter', bubbles: true });
      spyOn(eventEnter, 'preventDefault');
      component.onKeydown(eventEnter);
      expect(itemEmitido).toEqual(SUGESTOES[0]);
      expect(component.listVisible).toBeFalse();
      expect(eventEnter.preventDefault).toHaveBeenCalled();
    });

    it('Escape deve ocultar a lista e limpar opção ativa', () => {
      component.onKeydown(new KeyboardEvent('keydown', { key: 'ArrowDown', bubbles: true }));
      expect(component.activeOptionId).toBeTruthy();
      const eventEscape = new KeyboardEvent('keydown', { key: 'Escape', bubbles: true });
      component.onKeydown(eventEscape);
      expect(component.listVisible).toBeFalse();
      expect(component.activeOptionId).toBeNull();
    });
  });

  describe('acessibilidade', () => {
    it('input deve ter aria-autocomplete="list" e role="combobox"', () => {
      const input = fixture.debugElement.query(By.css('input'));
      expect(input.nativeElement.getAttribute('aria-autocomplete')).toBe('list');
      expect(input.nativeElement.getAttribute('role')).toBe('combobox');
    });

    it('lista de sugestões deve ter role="listbox" e aria-label', () => {
      const listbox = fixture.debugElement.query(By.css('[role="listbox"]'));
      expect(listbox).toBeTruthy();
      expect(listbox.nativeElement.getAttribute('aria-label')).toBe('Sugestões');
    });

    it('cada opção deve ter role="option" e id único', () => {
      component.listVisible = true;
      component.suggestions = SUGESTOES;
      fixture.detectChanges();
      const opcoes = fixture.debugElement.queryAll(By.css('[role="option"]'));
      expect(opcoes.length).toBe(2);
      expect(opcoes[0].nativeElement.getAttribute('id')).toBe('autocomplete-option-1');
      expect(opcoes[1].nativeElement.getAttribute('id')).toBe('autocomplete-option-2');
    });
  });
});
