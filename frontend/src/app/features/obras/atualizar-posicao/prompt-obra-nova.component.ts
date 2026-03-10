import { Component, ChangeDetectionStrategy, inject, Input, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormControl } from '@angular/forms';
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
  imports: [ReactiveFormsModule],
  styleUrl: './prompt-obra-nova.component.scss',
  templateUrl: './prompt-obra-nova.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PromptObraNovaComponent implements OnInit {
  private readonly dialogRef = inject(DialogRef<PromptObraNovaResult | undefined>);
  private readonly dialogData = inject(DIALOG_DATA, { optional: true }) as PromptObraNovaData | undefined;

  /** Apenas para testes unitários quando o componente não é aberto via DialogService. Em uso real os dados vêm de DIALOG_DATA. */
  private _data?: PromptObraNovaData;
  @Input() set data(v: PromptObraNovaData | undefined) {
    this._data = v;
    if (v?.nomeDefault) this.form.controls.nome.setValue(v.nomeDefault);
  }
  get data(): PromptObraNovaData | undefined {
    return this._data;
  }

  readonly form = new FormGroup({
    nome: new FormControl('', { nonNullable: true }),
    tipo: new FormControl<TipoObra>(TipoObra.Manga, { nonNullable: true }),
    ordemPreferencia: new FormControl(0, { nonNullable: true }),
  });
  readonly tiposObra = Object.values(TipoObra);

  ngOnInit(): void {
    const data = this._data ?? this.dialogData;
    if (data?.nomeDefault) this.form.controls.nome.setValue(data.nomeDefault);
  }

  prosseguir(): void {
    this.dialogRef.close({
      prosseguir: true,
      nome: this.form.controls.nome.value,
      tipo: this.form.controls.tipo.value,
      ordemPreferencia: this.form.controls.ordemPreferencia.value,
    });
  }

  cancelar(): void {
    this.dialogRef.close(undefined);
  }
}
