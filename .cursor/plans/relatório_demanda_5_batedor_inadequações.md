# Relatório de Inadequações — Demanda 5 (código alterado)

## Escopo
- `backend/src/DiarioDeBordo.Api/Program.cs` (AddJsonOptions)
- `frontend/src/app/application/atualizar-posicao.service.ts` (ObraDetalhe)
- `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts` (verPreview, helpers)
- `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.html` (preview obra nova)

## Resumo
- Total de achados: 4
- Por categoria: Dispensables (1), OO Abusers (1), Bloaters (1), Couplers (0), Arch and Struct (0), Change Preventers (0), Test Smells (0). Um achado é risco de bug (name shadowing).

## Achados

### 1. Código redundante (fallback desnecessário) — atualizar-posicao.component.ts
**Categoria:** Dispensables (Dead Code / redundância)
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:65`
**Evidência:** `const dataIso = this.dataParaEnvio ?? new Date().toISOString().slice(0, 10);` — o getter `dataParaEnvio` já retorna sempre uma string (hoje se `dataUltimaAtualizacao` for vazio), nunca `undefined`. O fallback `?? new Date().toISOString().slice(0, 10)` é redundante.
**Princípio/Referência violada:** DRY, clareza
**Contexto adicional:** Pode ser simplificado para `const dataIso = this.dataParaEnvio!;` ou apenas `this.dataParaEnvio` (o getter já garante valor).

---

### 2. Construção de prévia sintética inline no callback — atualizar-posicao.component.ts
**Categoria:** Bloaters (Long Method / lógica no callback)
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:62-79` (bloco error do subscribe)
**Evidência:** O callback de erro de `verPreview()` monta o objeto de prévia sintética com vários campos (`id`, `nome`, `tipo`, `posicaoAtual`, `dataUltimaAtualizacaoPosicao`, `ordemPreferencia`, `obraNova`) diretamente no callback. A responsabilidade “construir prévia sintética para obra nova” está acoplada ao tratamento de erro.
**Princípio/Referência violada:** SRP, legibilidade
**Contexto adicional:** Extrair para um método privado (ex.: `construirPreviaSinteticaObraNova()`) ou função pura que receba os campos do formulário e retorne `ObraDetalhe` com `obraNova: true` reduz o tamanho do callback e dá nome ao conceito.

---

### 3. Name shadowing (risco de recursão infinita) — atualizar-posicao.component.ts
**Categoria:** OO Abusers / bug potencial
**Localização:** `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:124-126`
**Evidência:**
```ts
formatarDataTooltip(s: string): string {
  return formatarDataTooltip(new Date(s));
}
```
O método do componente tem o mesmo nome que a função importada de `domain/datas`. Dentro do método, `formatarDataTooltip` refere-se ao próprio método, não ao import. A chamada é recursiva; se o template invocar este método, ocorre stack overflow. O template atual não usa este método; o risco permanece se alguém adicionar um tooltip.
**Princípio/Referência violada:** Clareza, ausência de bugs
**Contexto adicional:** Aliar o import (ex.: `import { formatarDataTooltip as formatarDataTooltipDomain }`) e chamar a função aliada no corpo, ou renomear o método do componente (ex.: `formatarDataTooltipParaTemplate`).

---

### 4. Interface sobrecarregada para dois contextos — atualizar-posicao.service.ts
**Categoria:** OO Abusers (Type Checking / discriminação implícita)
**Localização:** `frontend/src/app/application/atualizar-posicao.service.ts:8-17`
**Evidência:** A interface `ObraDetalhe` representa tanto a resposta da API (sempre com dados reais, sem `obraNova`) quanto a prévia sintética (com `obraNova: true`, `id: ''`). O tipo é único e a discriminação é feita por um campo opcional (`obraNova?: boolean`); o template e o componente fazem `@if (preview.obraNova)` e ramificações equivalentes.
**Princípio/Referência violada:** Tipagem discriminada, OCP (dois motivos de variação no mesmo tipo)
**Contexto adicional:** Opcionalmente, usar um tipo união (ex.: `ObraDetalheApi | ObraDetalheSintetico`) ou tipo genérico com discriminador deixa os dois casos explícitos e reduz checagens implícitas. Pode ser considerado refatoração futura se o time preferir manter um único DTO com flag.

---
