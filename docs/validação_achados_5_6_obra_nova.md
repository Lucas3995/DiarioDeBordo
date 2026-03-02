# Validação pós-correção — Achados 5 e 6 (Obra nova)

**Escopo da validação:** Ficheiros alterados para resolver os achados 5 e 6 do [Relatório de Inadequações — Demanda 2](relatório_inadequações_demanda_2_obra_nova.md): `prompt-obra-nova.component.spec.ts`, `atualizar-posicao.component.spec.ts`.

**Referência:** [Relatório de alterações achados 5 e 6](relatório_alterações_achados_5_6_obra_nova.md).

## Resultado da análise (batedor-de-codigos)

- **Achado 5 (PromptObraNovaResult em dois lugares):** **Sanado.** O spec de `PromptObraNovaComponent` importa `PromptObraNovaResult` de `./prompt-obra-nova.component` e não redefine a interface. Uma única fonte de verdade.
- **Achado 6 (Sleepy Test):** **Sanado.** O teste "ao salvar com sucesso deve chamar dialogRef.close(...)" passou a usar `fakeAsync` e `tick(DELAY_FECHAMENTO_APOS_SUCESSO_MS)`; não há mais `setTimeout(..., 1600)` nem `done()` nesse teste. Teste rápido e estável (FIRST).

## Suíte de testes

Executada após as alterações: **131 testes a passar** (frontend, `./scripts/frontend-test-docker.sh`).
