# Relatório de Inadequações — Demanda 2 (Obra nova, prompt em vez de checkbox)

**Escopo:** Código alterado ou criado para a demanda 2 do módulo de atualização de obra: componente `AtualizarPosicaoComponent`, componente `PromptObraNovaComponent`, testes unitários e E2E relacionados.

## Resumo

- **Total de achados:** 7
- **Por categoria:** Bloaters (1), Dispensables (2), Couplers (0), Arch and Struct (0), Test Smells (2), Object-Orientation Abusers (1), Change Preventers (1)

---

## Achados

### 1. Duplicate Code — AtualizarPosicaoComponent (tratamento de sucesso após salvar)

**Categoria:** Dispensables  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts` (bloco em duas ocorrências: ~linhas 111–123 e ~linhas 166–176)  
**Evidência:** O mesmo bloco de lógica é repetido em dois pontos: (1) no callback `next` de `atualizarPosicao().subscribe()` dentro de `salvar()`; (2) no callback `next` de `atualizarPosicao().subscribe()` dentro de `abrirPromptObraNovaParaSalvar()`. Em ambos: atribuição a `sucesso`, `preview = null`, `carregando = false`, e a sequência condicional `saidaAposSucesso.fecharComSucesso()` / `dialogRef.close({ salvou: true })` / `router.navigate(['/obras'])` com o mesmo `setTimeout` e constante `DELAY_FECHAMENTO_APOS_SUCESSO_MS`.  
**Princípio/Referência violada:** DRY  
**Contexto adicional:** Extrair para um método privado (ex.: `tratarSucessoSalvamento(res: AtualizarPosicaoResponse)`) evita duplicação e reduz risco de um dos fluxos ficar desatualizado em alterações futuras.

---

### 2. Duplicate Code — AtualizarPosicaoComponent (aplicação do resultado do prompt)

**Categoria:** Dispensables  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts`: trecho em `abrirPromptObraNovaParaPreview` (linhas 88–91) e em `abrirPromptObraNovaParaSalvar` (linhas 159–162)  
**Evidência:** Nos dois métodos, quando `result?.prosseguir` é verdadeiro, o mesmo trecho é executado: `this.nomeParaCriar = result.nome`, `this.tipoParaCriar = result.tipo`, `this.ordemPreferenciaParaCriar = result.ordemPreferencia`.  
**Princípio/Referência violada:** DRY  
**Contexto adicional:** A extração para um método privado (ex.: `aplicarResultadoPromptObraNova(result: PromptObraNovaResult)`) centraliza a atualização do estado e facilita manutenção.

---

### 3. Long Method — AtualizarPosicaoComponent (abrirPromptObraNovaParaSalvar)

**Categoria:** Bloaters  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:153–188`  
**Evidência:** O método `abrirPromptObraNovaParaSalvar` tem mais de 30 linhas, com dois níveis de callback (subscribe dentro de subscribe), múltiplas responsabilidades (abrir dialog, aplicar resultado, montar request, chamar API, tratar sucesso/erro) e o bloco de sucesso duplicado em relação a `salvar()`.  
**Princípio/Referência violada:** SRP, legibilidade  
**Contexto adicional:** A extração do tratamento de sucesso (achado 1) já reduz o tamanho; considerar ainda extrair a “ação após prosseguir no prompt” (montar request + chamar serviço + tratar resposta) para um método privado, deixando `abrirPromptObraNovaParaSalvar` apenas com a abertura do dialog e o subscribe que delega.

---

### 4. Temporary Field — AtualizarPosicaoComponent (nomeParaCriar, tipoParaCriar, ordemPreferenciaParaCriar)

**Categoria:** Object-Orientation Abusers  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:30–33`  
**Evidência:** Os campos `nomeParaCriar`, `tipoParaCriar` e `ordemPreferenciaParaCriar` só são preenchidos quando o usuário prossegue no prompt “obra nova”; no restante do tempo permanecem com valores default ou vazios. O componente usa `nomeParaCriar.trim().length > 0` para inferir “estado de criação pendentes”. São campos que só têm significado em um fluxo específico (objeto “incompleto” na maior parte do tempo).  
**Princípio/Referência violada:** Coesão  
**Contexto adicional:** Leve: em formulários é comum estado condicional. Alternativa seria encapsular os três campos em um único objeto opcional (ex.: `dadosCriacaoPendentes: { nome, tipo, ordemPreferencia } | null`), deixando explícito que o estado é “bloco preenchido ou ausente” e evitando múltiplos campos temporários.

---

### 5. Duplicate Code / Shotgun Surgery (risco) — PromptObraNovaResult definido em dois lugares

**Categoria:** Dispensables / Change Preventers  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/prompt-obra-nova.component.ts` (interface exportada) e `frontend/src/app/features/obras/atualizar-posicao/prompt-obra-nova.component.spec.ts:7–13` (interface redefinida e reexportada)  
**Evidência:** A interface `PromptObraNovaResult` está definida no componente e repetida no spec com a mesma forma. O spec exporta a sua própria definição em vez de importar a do componente. Qualquer alteração no contrato do resultado exige alterar dois ficheiros e há risco de as definições divergirem.  
**Princípio/Referência violada:** DRY, CCP  
**Contexto adicional:** O spec deve importar `PromptObraNovaResult` de `./prompt-obra-nova.component` e remover a definição e o reexport locais.

---

### 6. Sleepy Test — AtualizarPosicaoComponent spec (espera fixa após salvar)

**Categoria:** Test Smells  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.spec.ts:241–255`  
**Evidência:** O teste “ao salvar com sucesso deve chamar dialogRef.close({ salvou: true }) e não navegar” usa `setTimeout(..., 1600)` e `done()` para esperar o fechamento atrasado. A suíte fica mais lenta e o teste depende de um delay fixo (acoplado a `DELAY_FECHAMENTO_APOS_SUCESSO_MS`), tornando-o frágil perante mudança do valor e caracterizando “Sleepy Test”.  
**Princípio/Referência violada:** FIRST (Fast), repetibilidade  
**Contexto adicional:** Preferir testar o comportamento sem espera real (ex.: mockar o tempo, usar fakeAsync/tick, ou injetar um valor de delay menor em teste) para manter o teste rápido e estável.

---

### 7. Duas fontes de dados no mesmo componente — PromptObraNovaComponent (Input + DIALOG_DATA)

**Categoria:** Change Preventers / acoplamento a dois modos de uso  
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/prompt-obra-nova.component.ts:91–100` e `107–110`  
**Evidência:** O componente aceita dados tanto por `@Input() set data(v)` quanto por `inject(DIALOG_DATA)`; em `ngOnInit` usa `this._data ?? this.dialogData` para decidir a origem. Na prática o componente é sempre aberto via `DialogService.open()`, que fornece `DIALOG_DATA`. O `@Input() data` é usado apenas nos testes unitários. Duas formas de injeção para o mesmo fim aumentam a superfície de mudança e a complexidade de raciocínio.  
**Princípio/Referência violada:** Simplicidade, um único caminho de configuração  
**Contexto adicional:** Se a intenção é usar apenas como dialog: documentar que `data` é apenas para testes ou fornecer `DIALOG_DATA` no TestBed (via provider) e remover o `@Input() data`. Se o componente for reutilizado como filho com Input, manter ambos mas documentar os dois modos de uso.

---

## Conclusão

Há **7 achados** que justificam refatoração leve a moderada: principalmente **código duplicado** no componente de atualização de posição (tratamento de sucesso e aplicação do resultado do prompt), **método longo** no fluxo de salvar após 404, **definição duplicada da interface** no spec, **teste com sleep** e **duas fontes de dados** no prompt. Nenhum achado indica falha de regra de negócio; são melhorias de qualidade e manutenibilidade para consumo por uma skill de refatoração (ex.: mestre-freire).
