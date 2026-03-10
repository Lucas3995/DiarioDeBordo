---
name: Fonte de verdade para refactoring
overview: >
  Spec-as-source para ciclo de refatoração (descartável ao fim do ciclo).
  28 achados de inadequação (backend + frontend + transversal) com prioridades e plano de execução.
  Consumido por skills batedor-de-codigos (análise) e mestre-freire (execução).
sourceArtifact: Evolução das diretrizes em .github/ — verificar aderência do código
upstreamPlan: none
planType: sot-REFATORACAO
createdAt: "2026-03-09T00:00:00.000Z"
updatedAt: "2026-03-09T21:00:00.000Z"
todos:
  - id: back-repo-interfaces-domain
    content: "Mover interfaces de repositório (IObraLeituraRepository, IObraEscritaRepository, IUsuarioRepository) de Application para Domain"
  - id: back-ipasswordhasher-domain
    content: "Mover IPasswordHasher de Application para Domain"
  - id: back-itokenservice-domain
    content: "Mover ITokenService de Application para Domain"
  - id: back-authcontroller-primary-ctor
    content: "Padronizar AuthController para primary constructor (consistência com ObrasController)"
  - id: front-atualizar-posicao-port
    content: "Criar port abstrato para AtualizarPosicaoService (remover HttpClient direto)"
  - id: front-features-para-areas
    content: "Reestruturar features/ para areas/ com hierarquia áreas → módulos → pages → components"
  - id: front-central-acoes
    content: "Implementar Central de Ações (Command pattern) para requisições ao servidor — fila, retry, notificações"
  - id: front-typed-forms
    content: "Migrar de FormsModule/ngModel para Typed Reactive Forms (FormGroup<T>, FormControl<T>)"
  - id: front-signals-estado-local
    content: "Adotar Signals para estado local síncrono de componentes (flags, computed)"
  - id: back-command-bloat-split
    content: "Separar AtualizarPosicaoObraCommand em dois commands (Update + Create) para pureza CQRS"
  - id: front-memory-leaks
    content: "Corrigir subscriptions sem cleanup em AtualizarPosicaoComponent e LoginFormComponent"
  - id: front-onpush
    content: "Adotar ChangeDetectionStrategy.OnPush em todos os componentes"
  - id: front-legacy-ngif
    content: "Migrar *ngIf restante em login-form.component.html para sintaxe @if"
  - id: front-prompt-template-externo
    content: "Extrair template inline do PromptObraNovaComponent para ficheiro .html externo"
  - id: front-dialog-esc-backdrop
    content: "Adicionar ESC e backdrop click ao DialogService para dismiss"
  - id: front-missing-tests
    content: "Criar testes unitários para ConfigComponent, HomeComponent, AtualizarPosicaoComponent, PromptObraNovaComponent"
  - id: front-form-semantics
    content: "Envolver formulários em <form> para semântica HTML e a11y (submit via Enter)"
  - id: back-auditable-entity-ctor
    content: "Unificar construtores de AuditableEntity para padrão único com IClock"
  - id: back-request-command-duplication
    content: "Eliminar duplicação entre AtualizarPosicaoObraRequest e AtualizarPosicaoObraCommand"
  - id: back-rate-limiting
    content: "(Segurança) Adicionar rate limiting ao endpoint /api/auth/login"
  - id: back-validation-strings
    content: "Extrair mensagens de validação repetidas para constantes compartilhadas"
  - id: transversal-dockerfile-healthcheck
    content: "Adicionar instrução HEALTHCHECK ao Dockerfile"
  - id: back-obra-auditable
    content: "(Opcional) Avaliar se Obra deve herdar de AuditableEntity para rastreabilidade temporal"
  - id: back-data-protection
    content: "(Opcional — on-premise pessoal) Implementar DataProtectionService real para LGPD"
  - id: back-bounded-contexts
    content: "(Opcional) Formalizar Bounded Contexts (Auth vs Obras) com separação explícita"
  - id: transversal-regras-mdc
    content: "(Opcional) Avaliar se regras/*.mdc devem ser deprecadas em favor de .github/instructions/"
---

<!-- SPEC-AS-SOURCE: documento descartável ao fim do ciclo de refatoração.
     Consumidores: skills batedor-de-codigos e mestre-freire (agentes IA).
     Formato otimizado para parsing: campos fixos por achado, tabelas para lookup rápido. -->

# Achados de Inadequação

Total: **28** · SRP ✅ OCP ✅ LSP ✅ ISP ✅ DIP ❌ DA ✅ DDD ⚠️

| Categoria | Qt |
|-----------|----|
| Arch and Struct | 6 |
| Couplers | 1 |
| Anti-patterns DDD | 2 |
| Component Smells | 3 |
| Dispensables | 2 |
| Change Preventers | 4 |
| Bloaters | 1 |
| Memory/Performance | 3 |
| Security | 1 |
| Test Smells | 1 |
| Consistency | 3 |

---

## Backend

### [1] Interfaces de repositório em camada errada
- **Cat:** Arch and Struct · **Violação:** DIP, Clean Architecture
- **Loc:** `backend/src/DiarioDeBordo.Application/Obras/Listar/IObraLeituraRepository.cs:10`, `backend/src/DiarioDeBordo.Application/Obras/IObraEscritaRepository.cs:10`, `backend/src/DiarioDeBordo.Application/Auth/IUsuarioRepository.cs:10`
- **Evidência:** Interfaces de repositório definidas em Application; pertencem ao Domain (camada mais interna).
- **Todo:** `back-repo-interfaces-domain`

### [2] IPasswordHasher em camada errada
- **Cat:** Arch and Struct · **Violação:** DIP, Clean Architecture
- **Loc:** `backend/src/DiarioDeBordo.Application/Auth/IPasswordHasher.cs:8`
- **Evidência:** Contrato de hashing em Application; deveria estar em Domain (política de senha é regra de domínio). Implementação `BcryptPasswordHasher` já está em Infrastructure.
- **Todo:** `back-ipasswordhasher-domain`

### [3] ITokenService em camada errada
- **Cat:** Arch and Struct · **Violação:** DIP, Clean Architecture
- **Loc:** `backend/src/DiarioDeBordo.Application/Auth/ITokenService.cs:10`
- **Evidência:** Port de geração de credencial em Application; pertence ao Domain.
- **Todo:** `back-itokenservice-domain`

### [11] AuthController — constructor clássico
- **Cat:** Dispensables · **Violação:** Consistência
- **Loc:** `backend/src/DiarioDeBordo.Api/Controllers/AuthController.cs:15-19`
- **Evidência:** Usa constructor explícito + campo `_mediator`; demais controllers usam primary constructor (C# 12).
- **Todo:** `back-authcontroller-primary-ctor`

### [12] Obra sem AuditableEntity
- **Cat:** Anti-patterns DDD · **Violação:** Rastreabilidade de entidades
- **Loc:** `backend/src/DiarioDeBordo.Domain/Obras/Obra.cs:16`
- **Evidência:** Herda de `Entity` (só Id). Sem `CriadoEm`/`AtualizadoEm`. `AuditableEntity` existe mas não é usada.
- **Nota:** Pode ser intencional. Validar com operador.
- **Todo:** `back-obra-auditable`

### [13] DataProtectionService placeholder
- **Cat:** Change Preventers · **Violação:** LGPD Art. 46
- **Loc:** `backend/src/DiarioDeBordo.Infrastructure/Security/DataProtectionService.cs:16`
- **Evidência:** Retorna plaintext. Aceitável para on-premise pessoal; incompleto para produção.
- **Todo:** `back-data-protection`

### [14] AtualizarPosicaoObraCommand — Long Parameter List
- **Cat:** Bloaters · **Violação:** SRP, CQRS (um command = uma intenção)
- **Loc:** `backend/src/DiarioDeBordo.Application/Obras/AtualizarPosicao/AtualizarPosicaoObraCommand.cs:10-18`
- **Evidência:** 8 parâmetros misturando "atualizar posição" e "criar obra nova". Últimos 3 (`NomeParaCriar`, `TipoParaCriar`, `OrdemPreferenciaParaCriar`) só aplicam quando `CriarSeNaoExistir = true`.
- **Todo:** `back-command-bloat-split`

### [15] Bounded Contexts implícitos
- **Cat:** Anti-patterns DDD · **Violação:** DDD — Bounded Contexts
- **Loc:** `backend/src/DiarioDeBordo.Domain/` (nível de projeto)
- **Evidência:** Auth e Obras sem delimitação formal. Organização por pasta mas sem separação semântica no Domain.
- **Nota:** Suficiente para tamanho atual. Prioridade baixa.
- **Todo:** `back-bounded-contexts`

### [17] AuditableEntity — construtores mistos
- **Cat:** Change Preventers · **Violação:** Consistência, testabilidade
- **Loc:** `backend/src/DiarioDeBordo.Domain/Common/AuditableEntity.cs:14-32`
- **Evidência:** 4 construtores: 2 com `DateTime.UtcNow` direto, 2 com `IClock`. Dependência oculta do relógio dificulta testes.
- **Todo:** `back-auditable-entity-ctor`

### [18] Request duplica Command
- **Cat:** Dispensables · **Violação:** DRY
- **Loc:** `backend/src/DiarioDeBordo.Api/Models/AtualizarPosicaoObraRequest.cs`, `backend/src/DiarioDeBordo.Application/Obras/AtualizarPosicao/AtualizarPosicaoObraCommand.cs`
- **Evidência:** Mesmos 8 campos; controller mapeia campo-a-campo. Ligado a #14.
- **Todo:** `back-request-command-duplication`

### [19] Sem rate limiting no login
- **Cat:** Security · **Violação:** OWASP — Rate Limiting
- **Loc:** `backend/src/DiarioDeBordo.Api/Controllers/AuthController.cs` — `POST /api/auth/login`
- **Evidência:** `Microsoft.AspNetCore.RateLimiting` disponível no runtime mas não configurado. BCrypt work factor 12 atenua mas não substitui.
- **Nota:** On-premise pessoal; risco atenuado.
- **Todo:** `back-rate-limiting`

### [20] Mensagens de validação repetidas
- **Cat:** Dispensables · **Violação:** DRY
- **Loc:** Validators em `backend/src/DiarioDeBordo.Application/` ("não pode ser vazio", "deve ter no máximo X caracteres" etc.)
- **Evidência:** Strings idênticas dispersas sem constantes compartilhadas.
- **Todo:** `back-validation-strings`

---

## Frontend

### [4] AtualizarPosicaoService injeta HttpClient direto
- **Cat:** Couplers · **Violação:** DIP, `angular-frontend.instructions.md` §Serviços por camada
- **Loc:** `frontend/src/app/application/atualizar-posicao.service.ts:49`
- **Evidência:** Injeta `HttpClient` no construtor; demais serviços (AuthService, ListaObrasService) usam ports abstratos.
- **Todo:** `front-atualizar-posicao-port`

### [5] `features/` ao invés de `areas/`
- **Cat:** Arch and Struct · **Violação:** `angular-frontend.instructions.md` §Estrutura de pastas
- **Loc:** `frontend/src/app/features/` (pasta inteira)
- **Evidência:** Diretriz exige `areas/ → módulos → pages/ → components/`. Card dedicado.
- **Todo:** `front-features-para-areas`

### [6] Sem Central de Ações (Command pattern)
- **Cat:** Component Smells · **Violação:** `angular-frontend.instructions.md` §Frontend independente do backend
- **Loc:** `frontend/src/app/` (ausência de módulo)
- **Evidência:** Diretriz exige "Central de ações de servidor: Command serializável, fila, retry". Não existe. Card dedicado.
- **Todo:** `front-central-acoes`

### [7] Sem notificação centralizada de erros
- **Cat:** Component Smells · **Violação:** `angular-frontend.instructions.md` §Frontend independente do backend
- **Loc:** `frontend/src/app/` (ausência de serviço)
- **Evidência:** Erros tratados localmente em cada componente; sem toast/snackbar central. Complemento de #6.
- **Todo:** `front-central-acoes`

### [8] FormsModule/ngModel em vez de Typed Reactive Forms
- **Cat:** Change Preventers · **Violação:** `angular-frontend.instructions.md` §Typed forms
- **Loc:** `frontend/src/app/features/config/config.component.ts:2`, `frontend/src/app/features/auth/login-form.component.ts:3`, `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:3`, `frontend/src/app/features/obras/atualizar-posicao/prompt-obra-nova.component.ts:3`
- **Evidência:** Todos os formulários usam `FormsModule` + `[(ngModel)]`. Diretriz exige `FormGroup<T>`, `FormControl<T>`.
- **Todo:** `front-typed-forms`

### [9] Sem Signals para estado local
- **Cat:** Change Preventers · **Violação:** `angular-frontend.instructions.md` §Signals vs RxJS
- **Loc:** `frontend/src/app/features/` — todos os componentes
- **Evidência:** Zero uso de `signal()`, `computed()`, `effect()` (verificado por grep). Propriedades simples (`carregando`, `erro`, `obras`) são candidatas naturais.
- **Todo:** `front-signals-estado-local`

### [10] Sem `pages/` nos módulos
- **Cat:** Arch and Struct · **Violação:** `angular-frontend.instructions.md` §Estrutura modular — Páginas
- **Loc:** `frontend/src/app/features/obras/obra-lista.component.ts`, `features/home/home.component.ts`, `features/config/config.component.ts`
- **Evidência:** Componentes de página soltos na raiz de cada feature. Tratar junto com #5.
- **Todo:** `front-features-para-areas`

### [21] Memory leaks — subscriptions sem cleanup
- **Cat:** Memory/Performance · **Violação:** Angular lifecycle management
- **Loc:**
  - `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:82` (onSearchTerm)
  - `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:104` (verPreview)
  - `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:167` (salvar)
  - `frontend/src/app/features/obras/atualizar-posicao/atualizar-posicao.component.ts:223` (executarSalvamentoObraNova)
  - `frontend/src/app/features/auth/login-form.component.ts:42` (submeter)
- **Evidência:** 5 `subscribe()` sem `takeUntilDestroyed()` nem `unsubscribe()`. Apenas `ObraListaComponent` faz cleanup corretamente.
- **Todo:** `front-memory-leaks`

### [22] Zero componentes com OnPush
- **Cat:** Memory/Performance · **Violação:** Angular OnPush strategy
- **Loc:** `frontend/src/app/` — todos os componentes
- **Evidência:** Grep por `ChangeDetectionStrategy` retorna 0. Adotar junto com #9 (Signals + OnPush é idiomático Angular 21).
- **Todo:** `front-onpush`

### [23] `*ngIf` legado em login
- **Cat:** Consistency · **Violação:** Angular 17+ control flow
- **Loc:** `frontend/src/app/features/auth/login-form.component.html:35`, `:39`
- **Evidência:** Usa `*ngIf`; demais templates já usam `@if`.
- **Todo:** `front-legacy-ngif`

### [24] Template inline em PromptObraNovaComponent
- **Cat:** Consistency · **Violação:** `angular-frontend.instructions.md` §templateUrl/styleUrl
- **Loc:** `frontend/src/app/features/obras/atualizar-posicao/prompt-obra-nova.component.ts:27-93`
- **Evidência:** ~65 linhas de HTML inline; convenção do projeto é `templateUrl`.
- **Todo:** `front-prompt-template-externo`

### [25] DialogService sem ESC / backdrop click
- **Cat:** Component Smells · **Violação:** WCAG 2.1, WAI-ARIA dialog pattern
- **Loc:** `frontend/src/app/shared/dialog/dialog.service.ts`
- **Evidência:** Sem Escape para fechar, sem dismiss por clique no backdrop. Só fecha via botões internos.
- **Todo:** `front-dialog-esc-backdrop`

### [26] 4 componentes sem testes unitários
- **Cat:** Test Smells · **Violação:** Cobertura de testes
- **Loc:** `frontend/src/app/features/config/config.component.ts`, `features/home/home.component.ts`, `features/obras/atualizar-posicao/atualizar-posicao.component.ts`, `features/obras/atualizar-posicao/prompt-obra-nova.component.ts`
- **Evidência:** Sem `.spec.ts`. `AtualizarPosicaoComponent` é o mais complexo (~230 linhas) e não tem nenhum teste.
- **Dep:** Pré-requisito para refatorações #8, #9, #21, #22.
- **Todo:** `front-missing-tests`

### [27] Formulários sem `<form>`
- **Cat:** Consistency (a11y) · **Violação:** HTML5 semantics, WCAG
- **Loc:** `frontend/src/app/features/auth/login-form.component.html`, `features/config/config.component.html`, `features/obras/atualizar-posicao/atualizar-posicao.component.html`, `features/obras/atualizar-posicao/prompt-obra-nova.component.ts` (inline)
- **Evidência:** Nenhum formulário envolvido por `<form>`. Impede Enter-to-submit, `(ngSubmit)`, associação semântica.
- **Todo:** `front-form-semantics`

---

## Transversal

### [16] `regras/*.mdc` duplica `.github/instructions/`
- **Cat:** Dispensables · **Violação:** DRY
- **Loc:** `regras/` — 8 ficheiros `.mdc`
- **Evidência:** Legado Cursor cobrindo mesmos tópicos que `.github/instructions/` e `.github/skills/`. Validar com operador.
- **Todo:** `transversal-regras-mdc`

### [28] Dockerfile sem HEALTHCHECK
- **Cat:** Arch and Struct · **Violação:** Docker best practices
- **Loc:** `backend/Dockerfile`
- **Evidência:** Endpoints `/health` e `/status` existem mas Dockerfile não declara `HEALTHCHECK`.
- **Todo:** `transversal-dockerfile-healthcheck`

---

# Plano de Execução

## Prioridades

| Faixa | Critério | Gatilho |
|-------|----------|---------|
| **P0** | Bugs / degradação em produção | Corrigir antes de qualquer refatoração |
| **P1** | Dependências de camada, DIP, estrutura | Pré-requisito para demais |
| **P2** | SOLID, convenções, aderência, DRY | Qualidade e padrões |
| **P3** | Consistência, opcionais | Avaliar com operador |

## Achados por prioridade

### P0 — Crítico

| # | Todo ID | Ação |
|---|---------|------|
| 21 | `front-memory-leaks` | Adicionar `takeUntilDestroyed()` / `DestroyRef` |
| 26 | `front-missing-tests` | Criar specs (pré-req para #8, #9, #21, #22) |

### P1 — Alta

| # | Todo ID | Ação |
|---|---------|------|
| 1 | `back-repo-interfaces-domain` | Mover interfaces para Domain |
| 2 | `back-ipasswordhasher-domain` | Mover para Domain/Auth/ |
| 3 | `back-itokenservice-domain` | Mover para Domain/Auth/ |
| 4 | `front-atualizar-posicao-port` | Criar port abstrato + adapter |
| 14 | `back-command-bloat-split` | Separar em 2 commands |
| 5+10 | `front-features-para-areas` | Reestruturar pastas (card dedicado) |
| 6+7 | `front-central-acoes` | Command queue + toasts (card dedicado) |

### P2 — Média

| # | Todo ID | Ação |
|---|---------|------|
| 8 | `front-typed-forms` | Migrar para ReactiveFormsModule |
| 9+22 | `front-signals-estado-local` + `front-onpush` | Signals + OnPush |
| 17 | `back-auditable-entity-ctor` | Unificar construtores com IClock |
| 18 | `back-request-command-duplication` | Eliminar duplicação |
| 19 | `back-rate-limiting` | Rate limiter no login |
| 20 | `back-validation-strings` | Extrair constantes |
| 25 | `front-dialog-esc-backdrop` | ESC + backdrop click |
| 27 | `front-form-semantics` | Envolver em `<form>` |
| 13 | `back-data-protection` | Criptografia real (LGPD) |

### P3 — Baixa

| # | Todo ID | Ação |
|---|---------|------|
| 11 | `back-authcontroller-primary-ctor` | Primary constructor |
| 23 | `front-legacy-ngif` | `*ngIf` → `@if` |
| 24 | `front-prompt-template-externo` | Extrair template |
| 28 | `transversal-dockerfile-healthcheck` | HEALTHCHECK |
| 12 | `back-obra-auditable` | Avaliar com operador |
| 15 | `back-bounded-contexts` | Formalizar quando crescer |
| 16 | `transversal-regras-mdc` | Deprecar .mdc |

## Fases de execução

| Fase | Achados | Escopo |
|------|---------|--------|
| **0 — Estabilização** | #21, #26 | Corrigir leaks; criar testes faltantes |
| **1 — Camadas back** | #1+#2+#3, #11, #14+#18 | Mover interfaces; split command |
| **2 — Camadas front** | #4, #27+#23+#24, #25 | Port DIP; quick wins a11y/consistência |
| **3 — Modernização Angular** | #8+#9+#22 | Typed Forms + Signals + OnPush (por módulo) |
| **4 — Reestruturações grandes** | #5+#10, #6+#7 | Cards dedicados (areas/, Central de Ações) |
| **5 — Polimento** | #17+#20+#19, #28, opcionais | DRY, segurança, Dockerfile, opcionais |

---

# Restrições

1. **Não alterar regras de negócio.** Refatoração ≠ funcionalidade.
2. **Testes devem continuar passando.** Se falhar, corrigir o código de produção (ou imports/paths se foi apenas mover ficheiro).
3. **Funcionalidades intactas.** Sem adição nem remoção sem demanda.
4. **Validar testes após cada refatoração.** Back: `dotnet test backend/src/DiarioDeBordo.Tests/DiarioDeBordo.Tests.csproj -c Release`. Front: `cd frontend && npm run test`.
5. **Reestruturações grandes = cards dedicados.** #5+#10 (areas/) e #6+#7 (Central de Ações).
6. **Testes antes de refatorar.** #26 é pré-requisito para #8, #9, #21, #22.

---

# Conformidade (não alterar)

| Aspecto | OK? |
|---------|-----|
| CQRS via MediatR | ✅ |
| Entidades ricas | ✅ |
| Pipeline validação desacoplado | ✅ |
| Repos leitura/escrita separados | ✅ |
| BCrypt + timing equalization + JWT | ✅ |
| ExceptionMiddleware (sem stack trace prod) | ✅ |
| DIP ports front (Auth, ListaObras) | ✅ |
| Standalone components | ✅ |
| Lazy loading rotas | ✅ |
| templateUrl + styleUrl | ✅ (exceção #24) |
| takeUntilDestroyed | ⚠️ só ObraListaComponent (#21) |
| a11y autocomplete | ✅ |
| Domain types/enums/funções puras | ✅ |
| TS strict + strictTemplates | ✅ |
| Testes back (xUnit+FA+Moq) | ✅ |
| Testes front (serviços+componentes chave) | ⚠️ 4 sem teste (#26) |
| E2E Playwright | ✅ |
| Dockerfile multi-stage | ✅ |
| NuGet deps atualizadas | ✅ |
| npm deps atualizadas | ✅ |
