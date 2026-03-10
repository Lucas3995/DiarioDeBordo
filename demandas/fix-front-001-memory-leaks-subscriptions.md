# Memory Leaks — Subscriptions sem Cleanup

## Metadados

| Campo | Valor |
|-------|-------|
| **ID** | FIX-FRONT-001 |
| **Tipo** | Correção (bug latente) |
| **Prioridade** | Alta (P0 — degradação em produção) |
| **sourceArtifact** | `.github/reference/fonte-de-verdade-refactoring.md` — achado #21 |
| **Diretriz violada** | Angular lifecycle management; `.github/instructions/angular-frontend.instructions.md` §Signals vs RxJS |

---

## 1. Contexto

5 chamadas `subscribe()` sem cleanup em dois componentes. A cada navegação, subscriptions persistem no heap, acumulando callbacks e referências a componentes destruídos. O projeto já usa `ChangeDetectionStrategy.OnPush` e Signals nesses componentes, mas não adotou `takeUntilDestroyed()`.

**Ocorrências:**
- `AtualizarPosicaoComponent` — 4 subscribe() (linhas ~83, ~107, ~167, ~222). A linha ~132 usa `take(1)` e está segura.
- `LoginFormComponent` — 1 subscribe() (linha ~33).

---

## 2. User Story

Como desenvolvedor, quero que todas as subscriptions de componentes sejam encerradas ao destruir o componente para evitar memory leaks e comportamento imprevisível.

---

## 3. Critérios de aceitação

1. Cada subscribe() vulnerável usa `takeUntilDestroyed()` via `DestroyRef` injetado no construtor.
2. Nenhum subscribe() sem mecanismo de cleanup (takeUntilDestroyed, take(1), ou async pipe) em todo o frontend.
3. Testes existentes passam sem alteração.
4. Comportamento funcional idêntico ao anterior.

---

## 4. Alterações necessárias

1. **Injetar** `DestroyRef` em `AtualizarPosicaoComponent` e `LoginFormComponent`.
2. **Adicionar** `.pipe(takeUntilDestroyed(this.destroyRef))` antes de cada `subscribe()` vulnerável.
3. **Verificar** demais componentes por padrão semelhante (grep `subscribe(` sem takeUntil/take).

---

## 5. Requisitos técnicos/metodológicos aplicáveis

- TDD: testes existentes devem continuar passando; considerar adicionar teste específico de cleanup se componente tiver spec.
- Angular lifecycle: `DestroyRef` + `takeUntilDestroyed()` é o padrão idiomático Angular 21.
- Gestão de memória: eliminar referências pendentes ao destruir componentes.
