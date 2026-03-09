---
applyTo: "**/*.plan.md"
---

# Nomenclatura e rastreabilidade de planos

## Objetivo

Garantir que qualquer fonte de verdade/plano seja facil de localizar, ordenar e auditar além de ser otimizada para uso de agentes IA em metodologia spec driven development com spec as source porém descartando a spec ao concluir a demanda sobre a qual ela foi feita.

## Padrao de nomenclatura

Todo novo plano em `.cursor/plans/` deve seguir:

`YYYYMMDD__<TIPO>__<identificador>.plan.md`

### Tipos permitidos

- `sot-GLOBAL`
- `sot-TESTES`
- `sot-CODIGO`
- `sot-REFATORACAO`
- `sot-ENTREGA`
- `sot-OUTRO`

### Regras do identificador

- Usar `kebab-case`.
- Preferir referencia de demanda ou escopo.
- Sem espacos, acentos ou caracteres especiais.
- Permitir maiúsculas na segunda parte do tipo.

## Frontmatter minimo obrigatorio

Todo plano deve declarar no frontmatter:

- `name`
- `overview`
- `sourceArtifact`: card/requisito/bug/doc que originou o plano.
- `upstreamPlan`: ficheiro do plano anterior na cadeia (`none` quando plano inicial).
- `planType`: um dos tipos permitidos.
- `createdAt`: ISO-8601 UTC (`YYYY-MM-DDTHH:mm:ss.sssZ`).
- `updatedAt`: ISO-8601 UTC (`YYYY-MM-DDTHH:mm:ss.sssZ`).
- `todos`: nao vazio (ver `.github/instructions/planos-todos.instructions.md`).

## Regras de cadeia

1. `sot-GLOBAL` deve apontar para `upstreamPlan: none` (ou plano global anterior quando for evolucao da mesma demanda).
2. `sot-TESTES` deve apontar para um `sot-GLOBAL`.
3. `sot-CODIGO` deve apontar para um `sot-TESTES` e citar o `sot-GLOBAL` relacionado.
4. `sot-REFATORACAO` deve apontar para `none` (analise inicial) ou para o ultimo `sot-REFATORACAO` da mesma trilha.

Se um plano obrigatorio anterior nao for informado, o agente deve perguntar ao operador antes de continuar.

## Compatibilidade com legado

- Planos antigos podem manter nome legado.
- Ao evoluir um plano legado, usar o novo padrao de nome.
- Sempre incluir uma linha no corpo com `Legado relacionado:` quando houver ficheiro antigo equivalente.
