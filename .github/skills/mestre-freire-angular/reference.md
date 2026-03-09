# Referência — Mestre Freire Angular

Extensão Angular do mapeamento achado → técnica. Formato orientado a consumo por agente IA (listas, checklists, paths explícitos).

---

## Regra de critério técnico (Angular)

- **Ficheiro:** `.github/instructions/angular-frontend.instructions.md`
- **Conteúdo relevante:** princípio "estrutura de pastas = navegação e pertencimento", estrutura modular (áreas/módulos/páginas/componentes/directives/serviços), camadas como regras conceituais de dependência (não como pastas obrigatórias na raiz), convenções (templateUrl, styleUrl), serviços por camada (DIP), regras duras, a11y, Signals/RxJS, lifecycle, typed forms, central de ações (Command).

---

## Estrutura de pastas ≠ Arquitetura

**Princípio central:** a árvore de pastas reflete o **produto** (navegação e pertencimento), não um diagrama de camadas. Clean Architecture é a fundação invisível — regras de dependência e SOLID se aplicam em qualquer nível, mas não ditam a hierarquia de pastas.

Detalhes completos: `.github/instructions/angular-frontend.instructions.md` §Estrutura de pastas = navegação e pertencimento.

### Árvore de decisão — onde colocar um novo artefato

```
1. A QUEM PERTENCE?
   ├─ A uma área específica? → areas/<area>/<modulo>/
   ├─ É transversal / genérico? → shared/ (módulo ou componente)
   ├─ É singleton global? → core/
   └─ É interface/tipo transversal de domínio? → domain/ (raiz)

2. QUAL O PAPEL? (dentro do local decidido acima)
   ├─ Página → <local>/pages/<nome>/
   ├─ Componente (exclusivo da página) → <local>/pages/<pagina>/components/
   ├─ Componente (do nível atual) → <local>/components/
   ├─ Directiva → <local>/directives/
   ├─ Interface de domínio → <local>/domain/
   ├─ Caso de uso → <local>/application/ ou services/
   └─ Implementação HTTP → <local>/infrastructure/ ou services/
```

**Compartilhamento entre módulos/áreas:** via abstrações no `shared/` (interfaces, componentes base). Nunca por cópia de código nem acoplamento direto entre áreas. SOLID guia o **como**; estrutura guia o **onde**.

---

## Fluxo spec-first (para agente IA)

- **Quando há contrato OpenAPI:** gerar ou validar tipos e serviços de infraestrutura a partir do contrato; DTOs e interfaces alinhados à spec.
- **Quando há relatório de alterações ou demanda:** garantir critérios verificáveis (ex.: "página X faz Y", "serviço Z implementa interface W") antes de implementar; validar resultado contra esses critérios.
- **Ordem:** ler spec → planejar (onde criar ficheiros, que camada/conceito) → implementar → validar (testes, build).

---

## DIP e camadas no Angular

Ao tratar achados que mencionem **DIP**, **Clean Architecture** ou **serviços por camada** em código Angular:

- **Serviços por camada:** domain (interfaces/abstrações), application (casos de uso, sem HttpClient), infrastructure (implementações HTTP, repositórios), core (singletons, config). Componentes em features usam serviços de application ou infrastructure, não HttpClient direto.
- Consultar `.github/instructions/angular-frontend.instructions.md` para o mapeamento completo e dependências permitidas.

---

## Estrutura modular (conceitos → ficheiros/pastas)

Mapeamento dos conceitos do produto para a árvore de pastas. Ver §Estrutura de pastas ≠ Arquitetura acima para o princípio orientador.

- **Áreas:** conjuntos de módulos por roles/permissões; pasta por área (ex.: `app/areas/admin/`, `app/areas/creator/`).
- **Módulos:** menor agrupamento coeso de páginas e ações desenvolvível de forma independente; pertence a uma **área** (ex.: `areas/admin/gerenciamento-acessos/`) ou ao **shared** (ex.: `shared/gerenciamento-acessos/`). Cada módulo tem `pages/`, `components/`, rotas e eventualmente `domain/`, `services/` internos.
- **Páginas:** storytelling UX; um componente de página por rota dentro do módulo (ex.: `areas/admin/ger-acessos/pages/lista/`).
- **Componentes:** menor unidade implantável do Angular; reutilizáveis para montar páginas. `shared/components/` (globais), `<modulo>/components/` (do módulo), `<pagina>/components/` (exclusivos da página). Subpastas `components/` podem existir em qualquer nível (área, módulo, página) para artefatos exclusivos daquele nível — se o componente servir a outros consumidores, deve subir para o nível adequado respeitando SOLID e regras de exclusividade/uso-compartilhado.
- **Directives:** extensão de HTML (comportamento sem template); `shared/directives/` ou `<modulo>/directives/`.
- **Serviços:** vivem dentro do módulo que os possui, ou em `core/`/`shared/`/`domain/` (raiz) quando transversais. A camada Clean Arch (domain, application, infrastructure) determina o papel, não necessariamente a pasta na raiz.

Uso pelo agente: usar a árvore de decisão (§Estrutura ≠ Arquitetura) para decidir **onde** criar cada artefato e **quando** abrir novo módulo em Greenfield e Evolução.

---

## Testes no Angular

- **Ficheiros de teste:** não alterar `*.spec.ts`. A skill mestre-freire (e esta) mantém testes intocáveis.
- **Comando:** `npm run test` ou `ng test`. Executar após cada passo (ou lote coerente) de refatoração; a suíte deve permanecer verde.
- Se um teste falhar após refatoração, corrigir o **código de produção**, não o spec.

---

## Acessibilidade (a11y)

- **Checklist mínimo:** semântica HTML (`<button>`, `<nav>`, `<label>`, etc.); ARIA onde HTML não basta; teclado (interactivos acessíveis, foco visível); feedback para leitores de tela.
- **LiveAnnouncer** (Angular CDK) para anunciar conteúdo dinâmico.
- **Referência:** Angular a11y (angular.dev/best-practices/a11y), CDK a11y (FocusTrap, FocusMonitor).

---

## Signals vs RxJS

- **Signals:** estado síncrono e fino na UI (flags de componente, computed); preferir para estado local e derived state.
- **RxJS:** fluxos assíncronos (HTTP, WebSockets, eventos, operadores); manter para tudo o que é async.
- **Interop:** `toSignal()` para consumir Observable no template; `toObservable()` para aplicar operadores RxJS a Signal; `takeUntilDestroyed(this.destroyRef)` para subscriptions em componentes (Angular 16+).

---

## Ciclo de vida

- **Limpeza em `ngOnDestroy`:** subscriptions, listeners, timers para evitar memory leaks.
- **Preferir `takeUntilDestroyed(this.destroyRef)`** em vez de unsubscribe manual quando possível (Angular 16+).
- Directives e serviços: respeitar destruição; não reter referências a componentes destruídos.

---

## Typed forms

- Preferir **FormGroup/FormControl tipados** (Angular 14+); inferência ou genéricos.
- Evitar `UntypedFormGroup`/`UntypedFormControl` exceto em migração.

---

## Central de ações / Command

- Todas as requisições ao backend passam por um **módulo/serviço central** (design pattern Command).
- **Falha de rede/backend:** notificar utilizador, persistir comando (fila), permitir retry; **nunca** quebrar a página.
- UX: central acessível (ex.: menu superior) onde o utilizador vê processos solicitados e obtém feedback quando o comando terminar.

---

## Testes assíncronos (Angular)

| Técnica | Quando usar | Exemplo |
|---------|-------------|---------|
| **`fakeAsync` + `tick`** | Testar código com timers, debounce, `setTimeout`, animações. | `fakeAsync(() => { component.search('x'); tick(300); expect(...) })` |
| **`waitForAsync`** | Testar código com Promises ou micro-tasks. | `waitForAsync(() => { fixture.detectChanges(); fixture.whenStable().then(() => expect(...)) })` |
| **Marble testing (RxJS)** | Testar operadores complexos, combinações de Observables, timing. | `cold('--a--b|', { a: 1, b: 2 })` com `expectObservable`. |
| **`TestScheduler`** | Controle fino de tempo virtual em streams RxJS. | `testScheduler.run(({ cold, expectObservable }) => { ... })` |

**Regra:** preferir `fakeAsync` para a maioria dos cenários; marble testing apenas para operadores RxJS complexos.

---

## Gestão de estado

| Abordagem | Quando usar | Padrão |
|-----------|-------------|--------|
| **Signals (local)** | Estado de componente: flags, contadores, computed values. | `signal()`, `computed()`, `effect()`. Sem boilerplate. |
| **Signals (compartilhado)** | Estado partilhado entre poucos componentes próximos. | Signal em serviço `providedIn: 'root'` ou no módulo. |
| **NgRx / store (global)** | Estado complexo, muitas ações, efeitos colaterais, debugging com devtools. | Actions → Reducers → Selectors → Effects. Quando Signals não bastam. |
| **Service com BehaviorSubject** | Legado; migrar para Signals quando refatorar. | `BehaviorSubject` em serviço; componentes subscrevem. |

**Princípio:** estado local com Signals; estado global complexo com store; evitar estado global para dados que pertencem a um único módulo.

---

## Tratamento de erros e resiliência

| Cenário | Estratégia | No código |
|---------|-----------|-----------|
| **Erro HTTP** | Interceptor global para erros 4xx/5xx; notificação ao utilizador. | `HttpInterceptorFn` que captura erro, notifica e rethrows para o chamador decidir. |
| **Erro de validação (400)** | Exibir mensagens específicas nos campos do formulário. | Interceptor extrai `errors` do response; serviço distribui para `FormControl.setErrors()`. |
| **Timeout/rede** | Retry com backoff exponencial; fallback offline se aplicável. | `retry({ count: 3, delay: (err, n) => timer(n * 1000) })` no Observable. |
| **Erro inesperado** | `ErrorHandler` global; log; tela amigável. | `class GlobalErrorHandler implements ErrorHandler { handleError(err) { ... } }` |
| **Componente isolado** | Error boundary no template; componente falha sem derrubar a página. | `@defer` com `@error { ... }` (Angular 17+) ou try/catch em lifecycle. |

---

## Lazy loading e performance

| Técnica | Quando usar | Como |
|---------|-------------|------|
| **Lazy routes** | Módulos/features carregados sob demanda. | `loadComponent: () => import('./feature/page.component')` nas rotas. |
| **`@defer`** | Componentes pesados carregados quando visíveis ou sob condição. | `@defer (on viewport) { <heavy-component /> }` (Angular 17+). |
| **Preloading strategy** | Carregar módulos em background após initial load. | `PreloadAllModules` ou custom strategy nas rotas. |
| **TrackBy em loops** | Evitar re-render de listas inteiras. | `@for (item of items; track item.id) { ... }` (Angular 17+ control flow). |
| **OnPush** | Reduzir detecção de mudanças desnecessária. | `changeDetection: ChangeDetectionStrategy.OnPush` em componentes puros. |

---

## Domain Services no Angular

Quando a lógica de domínio no frontend envolve mais de uma entidade ou cálculos complexos:

| Camada | O que | Onde |
|--------|-------|------|
| **domain** | Interfaces (ports), Value Objects, regras puras. | `<modulo>/domain/` (específico) ou `app/domain/` (transversal) — sem dependências Angular. |
| **application** | Domain Services (stateless), orquestração de casos de uso. | `<modulo>/application/` ou `<modulo>/services/` — injeta interfaces do domain. |
| **infrastructure** | Implementações concretas (HTTP, storage, adapters). | `<modulo>/infrastructure/` ou `<modulo>/services/` — implementa interfaces do domain. |

**Regra DIP:** componentes e serviços de application dependem de interfaces em domain; implementações concretas em infrastructure são injetadas via `providers` no módulo ou rota. Estas camadas vivem **dentro do módulo** quando específicas, ou na raiz quando transversais.
