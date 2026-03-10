# Reestruturar `features/` → `areas/` com hierarquia `pages/components`

## Metadados

| Campo | Valor |
|-------|-------|
| **ID** | REFACT-STRUCT-001 |
| **Tipo** | Refatoração estrutural (zero mudança de comportamento) |
| **Prioridade** | Média |
| **sourceArtifact** | `.github/reference/fonte-de-verdade-refactoring.md` — achados #5 e #10 |
| **Diretriz violada** | `.github/instructions/angular-frontend.instructions.md` §Estrutura de pastas, §Estrutura modular — Páginas |

---

## 1. Contexto

A estrutura de pastas do frontend usa `features/` com componentes soltos na raiz de cada feature. A diretriz exige `areas/ → módulos → pages/ → components/`. Divergência impacta manutenibilidade, consistência com documentação e capacidade do agente IA de localizar ficheiros.

**User Story:**
Como desenvolvedor, quero que a estrutura de pastas do frontend siga a convenção `areas/ → módulos → pages/ → components/` para localizar componentes de forma previsível e consistente com a documentação.

**Critérios de aceitação:**
1. Pasta `features/` não existe; substituída por `areas/`.
2. Componentes roteáveis ficam em `pages/<nome>/`.
3. Componentes auxiliares ficam em `components/` dentro da página que os utiliza.
4. Todas as rotas, imports e lazy loading funcionam identicamente.
5. Todos os testes unitários e E2E passam sem alteração de lógica.
6. `grep -rn "features/" frontend/src/` retorna zero resultados.

---

## 2. Mapa de movimentação

| De (atual) | Para (esperado) |
|------------|-----------------|
| `features/auth/login-form.*` | `areas/auth/pages/login/login-form.*` |
| `features/config/config.*` | `areas/config/pages/config/config.*` |
| `features/home/home.component.ts` | `areas/home/pages/home/home.component.ts` |
| `features/obras/obra-lista.*` | `areas/obras/pages/lista/obra-lista.*` |
| `features/obras/obras.routes.*` | `areas/obras/obras.routes.*` |
| `features/obras/atualizar-posicao/atualizar-posicao.*` | `areas/obras/pages/atualizar-posicao/atualizar-posicao.*` |
| `features/obras/atualizar-posicao/prompt-obra-nova.*` | `areas/obras/pages/atualizar-posicao/components/prompt-obra-nova.*` |

---

## 3. Alterações necessárias

1. **Renomear** `features/` → `areas/`.
2. **Criar hierarquia** `pages/` em cada módulo (auth, config, home, obras).
3. **Mover** `prompt-obra-nova` para `components/` dentro da página que o utiliza.
4. **Atualizar** `app.routes.ts` — lazy loading (`./features/...` → `./areas/...`).
5. **Atualizar** `obras.routes.ts` — import do componente (`./obra-lista...` → `./pages/lista/obra-lista...`).
6. **Atualizar** imports relativos entre componentes/specs (nova profundidade de pastas).
7. **Verificar** que testes E2E não referenciam paths de ficheiros (usam DOM/URLs — impacto improvável).
8. **Remover** pasta `features/` vazia.

---

## 4. Requisitos técnicos/metodológicos aplicáveis

- **TDD**: testes existentes são safety net — devem passar antes e depois sem alteração de lógica.
- **DDD**: `areas/` mapeia áreas de domínio; `pages/` são entry points do utilizador.
- **SOLID**: refatoração não altera responsabilidades.
- **Pirâmide de testes**: validação em unitários (specs), integração (rotas carregam componentes), E2E (Playwright).
- **Angular**: lazy loading via `loadChildren`/`loadComponent` mantido; `templateUrl`/`styleUrl` relativo (movem junto).

## 5. Dependências e riscos

| Risco | Severidade | Mitigação |
|-------|------------|-----------|
| Rename massivo quebra imports não mapeados | Alta | IDE refactoring tools + `grep` pós-movimentação + `ng build --configuration production` |
| Imports relativos com profundidade errada | Média | Testes como safety net — se passam, imports corretos |

## 6. Definição de pronto

- [ ] `ng build --configuration production` sem erros.
- [ ] Todos os testes unitários passam.
- [ ] Todos os testes E2E passam.
- [ ] `grep -rn "features/" frontend/src/` retorna zero.
- [ ] PR aprovado e CI verde.
