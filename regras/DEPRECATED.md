# ⚠️ DEPRECATED

Os arquivos `.mdc` nesta pasta estão **deprecados**.

## Migração

| Arquivo `.mdc` | Destino (fonte de verdade) |
|---|---|
| `angular-frontend.mdc` | `.github/instructions/angular-frontend.instructions.md` |
| `clean-architecture.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |
| `cqrs-command-query-responsibility-segregation.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |
| `ddd-domain-driven-design.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |
| `devsecops.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |
| `protecao-dados-lgpd-seguranca.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |
| `tdd-test-driven-development.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |
| `tecnicas-conhecidas.mdc` | `.github/skills/engenharia-de-software/SKILL.md` |

## Motivo

- O formato `.mdc` (Cursor rules) não é consumido pelo VS Code / GitHub Copilot.
- O conteúdo de `angular-frontend.mdc` é idêntico ao de `.github/instructions/angular-frontend.instructions.md`.
- Os demais arquivos contêm referências de engenharia de software já cobertas pela skill `engenharia-de-software`.
- Manter dois locais gera risco de divergência e carga de manutenção desnecessária.

**Não altere estes arquivos.** Faça futuras edições nos destinos indicados acima.
