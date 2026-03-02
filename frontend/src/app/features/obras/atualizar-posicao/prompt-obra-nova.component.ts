import { Component, inject, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DialogRef } from '../../../shared/dialog/dialog-ref';
import { DIALOG_DATA } from '../../../shared/dialog/dialog.service';
import { TipoObra } from '../../../domain/obra.types';

export interface PromptObraNovaResult {
  prosseguir: true;
  nome: string;
  tipo: TipoObra;
  ordemPreferencia: number;
}

export interface PromptObraNovaData {
  nomeDefault: string;
}

@Component({
  selector: 'app-prompt-obra-nova',
  standalone: true,
  imports: [CommonModule, FormsModule],
  styleUrl: './prompt-obra-nova.component.scss',
  template: `
    <div class="prompt-obra-nova">
      <header class="dialog-header">
        <h2 class="dialog-titulo" id="prompt-obra-nova-titulo">Obra nova</h2>
        <p class="instrucao" id="prompt-obra-nova-descricao">
          Esta obra ainda não está cadastrada. Confirme o nome e o tipo para criar e atualizar a posição.
        </p>
      </header>
      <div class="formulario">
        <div class="campo">
          <label class="label" for="prompt-obra-nova-nome">Nome</label>
          <input
            id="prompt-obra-nova-nome"
            type="text"
            class="input"
            [(ngModel)]="nome"
            placeholder="Nome da obra"
            data-testid="prompt-obra-nova-nome"
          />
        </div>
        <div class="campo">
          <label class="label" for="prompt-obra-nova-tipo">Tipo</label>
          <select
            id="prompt-obra-nova-tipo"
            class="input input-select"
            [(ngModel)]="tipo"
            data-testid="prompt-obra-nova-tipo"
          >
            @for (t of tiposObra; track t) {
              <option [value]="t">{{ t }}</option>
            }
          </select>
        </div>
        <div class="campo">
          <label class="label" for="prompt-obra-nova-ordem">Ordem de preferência</label>
          <input
            id="prompt-obra-nova-ordem"
            type="number"
            min="0"
            class="input input-number"
            [(ngModel)]="ordemPreferencia"
            data-testid="prompt-obra-nova-ordem"
          />
        </div>
        <div class="botoes">
          <button
            type="button"
            class="btn btn-primario"
            (click)="prosseguir()"
            data-testid="prompt-obra-nova-prosseguir"
          >
            Prosseguir
          </button>
          <button
            type="button"
            class="btn btn-secundario"
            (click)="cancelar()"
            data-testid="prompt-obra-nova-cancelar"
          >
            Cancelar
          </button>
        </div>
      </div>
    </div>
  `,
})
export class PromptObraNovaComponent implements OnInit {
  private readonly dialogRef = inject(DialogRef<PromptObraNovaResult | undefined>);
  private readonly dialogData = inject(DIALOG_DATA, { optional: true }) as PromptObraNovaData | undefined;

  /** Apenas para testes unitários quando o componente não é aberto via DialogService. Em uso real os dados vêm de DIALOG_DATA. */
  private _data?: PromptObraNovaData;
  @Input() set data(v: PromptObraNovaData | undefined) {
    this._data = v;
    if (v?.nomeDefault) this.nome = v.nomeDefault;
  }
  get data(): PromptObraNovaData | undefined {
    return this._data;
  }

  nome = '';
  tipo: TipoObra = TipoObra.Manga;
  ordemPreferencia = 0;
  readonly tiposObra = Object.values(TipoObra);

  ngOnInit(): void {
    const data = this._data ?? this.dialogData;
    if (data?.nomeDefault) this.nome = data.nomeDefault;
  }

  prosseguir(): void {
    this.dialogRef.close({
      prosseguir: true,
      nome: this.nome,
      tipo: this.tipo,
      ordemPreferencia: this.ordemPreferencia,
    });
  }

  cancelar(): void {
    this.dialogRef.close(undefined);
  }
}
