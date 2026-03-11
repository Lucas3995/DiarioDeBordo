import {
  Component,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  Output,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

/**
 * Item genérico para sugestões: qualquer tipo de objeto que o componente consiga
 * exibir via displayWith e identificar via getItemId (ex.: ObraBuscaItem).
 * Interface vazia para não impor assinatura de índice e permitir tipos concretos.
 */
// eslint-disable-next-line @typescript-eslint/no-empty-object-type
export interface AutocompleteItem {}

/**
 * Componente reutilizável de autocomplete: input + lista de sugestões,
 * pesquisa com debounce, seleção por teclado e rato.
 * Mantém consistência visual e de acessibilidade (aria-autocomplete, listbox, etc.).
 */
@Component({
  selector: 'app-input-autocomplete',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './input-autocomplete.component.html',
  styleUrl: './input-autocomplete.component.scss',
})
export class InputAutocompleteComponent<T extends AutocompleteItem>
  implements OnChanges, OnDestroy
{
  /** Valor atual do input (two-way com valueChange). */
  @Input() value = '';

  /** Placeholder do input. */
  @Input() placeholder = '';

  /** Lista de sugestões a exibir. */
  @Input() suggestions: T[] = [];

  /** Função que retorna o texto a exibir para cada item. */
  @Input() displayWith: (item: T) => string = (item) => String(item);

  /** Função que retorna o id estável do item (para track e aria-activedescendant). */
  @Input() getItemId: (item: T) => string = (item) =>
    (item as { id?: string }).id ?? String(item);

  /**
   * Atraso em ms antes de emitir searchTerm (debounce).
   * Lido apenas na inicialização do componente; alterações posteriores a este valor não têm efeito.
   */
  @Input() debounceMs = 300;

  /** Label acessível do input. */
  @Input() ariaLabel = 'Autocomplete';

  /** Id do input (para associação com <label for="">). */
  @Input() inputId = 'autocomplete-input';

  /** data-testid opcional no input. */
  @Input() dataTestid: string | null = null;

  /** data-testid opcional na lista de sugestões (listbox). */
  @Input() listDataTestid: string | null = null;

  /** Emitido quando o valor do input muda. */
  @Output() valueChange = new EventEmitter<string>();

  /** Emitido após debounce quando o utilizador digita (para o pai buscar sugestões). */
  @Output() searchTerm = new EventEmitter<string>();

  /** Emitido quando o utilizador seleciona uma sugestão (rato ou Enter). */
  @Output() suggestionSelected = new EventEmitter<T>();

  listVisible = false;
  activeOptionId: string | null = null;
  private indiceAtivo = -1;
  private readonly termo$ = new Subject<string>();
  private sub?: Subscription;

  constructor() {
    this.sub = this.termo$
      .pipe(debounceTime(this.debounceMs), distinctUntilChanged())
      .subscribe((termo) => this.searchTerm.emit(termo));
  }

  ngOnChanges(changes: SimpleChanges): void {
    const sug = changes['suggestions'];
    if (sug && !sug.firstChange) {
      const cur = sug.currentValue as T[] | undefined;
      if (cur?.length) {
        this.listVisible = true;
        this.indiceAtivo = -1;
        this.activeOptionId = null;
      } else {
        this.listVisible = false;
        this.indiceAtivo = -1;
        this.activeOptionId = null;
      }
    }
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  onInput(valor: string): void {
    this.valueChange.emit(valor);
    this.termo$.next(valor.trim());
  }

  onFocus(): void {
    if (this.suggestions.length > 0) this.listVisible = true;
  }

  onBlur(): void {
    setTimeout(() => (this.listVisible = false), 150);
  }

  onKeydown(event: KeyboardEvent): void {
    if (!this.listVisible || this.suggestions.length === 0) return;
    if (event.key === 'ArrowDown') {
      event.preventDefault();
      this.indiceAtivo = Math.min(
        this.indiceAtivo + 1,
        this.suggestions.length - 1,
      );
      this.updateActiveId();
    } else if (event.key === 'ArrowUp') {
      event.preventDefault();
      this.indiceAtivo = Math.max(this.indiceAtivo - 1, 0);
      this.updateActiveId();
    } else if (
      event.key === 'Enter' &&
      this.indiceAtivo >= 0 &&
      this.suggestions[this.indiceAtivo]
    ) {
      event.preventDefault();
      this.select(this.suggestions[this.indiceAtivo]);
    } else if (event.key === 'Escape') {
      this.listVisible = false;
      this.indiceAtivo = -1;
      this.activeOptionId = null;
    }
  }

  select(item: T): void {
    this.suggestionSelected.emit(item);
    this.listVisible = false;
    this.indiceAtivo = -1;
    this.activeOptionId = null;
  }

  optionId(item: T): string {
    return `autocomplete-option-${this.getItemId(item)}`;
  }

  isActive(item: T): boolean {
    return this.optionId(item) === this.activeOptionId;
  }

  private updateActiveId(): void {
    const item = this.suggestions[this.indiceAtivo];
    this.activeOptionId = item ? this.optionId(item) : null;
  }
}
