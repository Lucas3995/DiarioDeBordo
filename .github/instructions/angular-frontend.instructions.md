---
applyTo: "frontend/**"
---

# Angular Frontend — Regra de critério técnico

## Spec antes de código

- **Entrada obrigatória antes de gerar ou alterar código Angular:** contrato de API (ex.: OpenAPI), relatório de alterações (ex.: maestro) ou demanda com critérios verificáveis.
- Fluxo: **ler spec → planejar** (onde criar ficheiros, que conceito, a quem pertence) **→ implementar → validar** (testes, build).
- Nunca gerar código Angular sem referência a um artefato de spec.

---

## Estrutura de pastas = navegação e pertencimento

**Estrutura de pastas não é arquitetura.** A árvore de pastas é um mapa do produto: ao navegá-la, deve-se ver **áreas → módulos → páginas → componentes**. Isso reflete **navegação** (como o utilizador chega lá) e **pertencimento** (a quem aquilo serve).

Clean Architecture **não é negada** — é a fundação invisível. Pastas como `domain/`, `services/`, `application/` podem existir em **qualquer nível** (raiz, dentro de uma área, dentro de um módulo) como suporte organizacional. Mas a hierarquia primária de pastas reflete o **frontend como produto**, não um diagrama de camadas.

**Duas dimensões que coexistem:**

| Dimensão | Responde a | Exemplos |
|----------|-----------|----------|
| **Estrutura** (onde mora) | Navegação e pertencimento | `areas/admin/gerenciamento-acessos/`, `shared/notificacoes/` |
| **SOLID / Clean Arch** (como se comporta) | Dependências, abstrações, responsabilidades | DIP entre serviços, SRP dentro do módulo, OCP nos componentes |

- **Decisão primária:** "A quem pertence?" (qual área, qual módulo, é transversal?) — não "que camada é?"
- Estrutura de pertencimento **não é desculpa** para violar SOLID. Compartilhamento entre módulos/áreas: via abstrações no `shared/` (interfaces, componentes base), nunca por cópia de código nem acoplamento direto entre áreas.

### Árvore de referência

```
app/
  areas/
    admin/
      components/                   ← componentes exclusivos da área admin
      gerenciamento-acessos/        ← módulo pertence à área admin
        pages/
          lista/
            components/             ← componentes exclusivos desta página
            lista.component.ts
            lista.component.html
        components/                 ← componentes exclusivos deste módulo
        domain/                     ← clean arch DENTRO do módulo
        services/
      dashboard/                    ← outro módulo da área admin
        pages/
        components/
    creator/                        ← outra área (outra visão/UI/permissões)
      ...
  shared/                           ← módulos/componentes transversais
    gerenciamento-acessos/          ← versão genérica; compartilha com admin via SOLID
      pages/
      components/
    components/                     ← componentes reutilizáveis globais
    directives/                     ← directivas reutilizáveis globais
  core/                             ← singletons globais (auth, config, logging)
  domain/                           ← interfaces/tipos transversais (opcional)
```

> **Regra de subpastas `components/`:** subpastas `components/` podem existir em **qualquer nível** (área, módulo, página) para artefatos **estritamente exclusivos** daquele nível. Se o componente fizer sentido para outros consumidores, deve subir para o nível adequado (`<modulo>/components/`, `shared/components/`, etc.) respeitando SOLID.

---

## Estrutura modular — conceitos e definições operativas

Estes conceitos definem **o que cada nível da árvore representa** e como se organiza.

- **Áreas:** Conjuntos de módulos ligados a roles/permissões de utilizador; geram uma visão de sistema; podem ter UI/layout completamente diferente entre si. **Onde:** pasta por área (ex.: `app/areas/admin/`, `app/areas/creator/`).
- **Módulos:** Menor agrupamento coeso de páginas e ações desenvolvível de forma independente. Um módulo pertence a uma **área** (ex.: `areas/admin/gerenciamento-acessos/`) ou ao **shared** (ex.: `shared/gerenciamento-acessos/`). Cada módulo tem `pages/`, `components/`, rotas e eventualmente `domain/`, `services/` internos.
- **Páginas:** Conjuntos de componentes agrupados para uma ação ou conjunto de informações; storytelling em UX/UI guiando o utilizador de forma assertiva e informativa. **Onde:** um componente de página por rota dentro do módulo (ex.: `areas/admin/gerenciamento-acessos/pages/lista/`).
- **Componentes:** Menor unidade de software implantável num projeto Angular; partes de código reutilizáveis para montar páginas (conjuntos de elementos, abstrações reutilizáveis). **Onde:** `shared/components/` para globais; `<modulo>/components/` para específicos do módulo; `<pagina>/components/` para exclusivos de uma página. A mesma lógica de subpastas `components/` aplica-se em **qualquer nível** (área, módulo, página): partes menores podem ser contidas em qualquer nível das partes maiores, desde que sigam as regras de SOLID e de exclusividade/uso-compartilhado.
- **Directives:** Estender HTML com comportamento customizado ou transformar elementos (sem template próprio). **Onde:** `shared/directives/` ou `<modulo>/directives/`.
- **Serviços:** Lógica reutilizável. Vivem **dentro do módulo** que os possui, ou em `core/` / `shared/` / `domain/` (raiz) quando transversais. A camada Clean Arch (domain, application, infrastructure) determina o papel do serviço, não necessariamente sua pasta na raiz.

---

## Camadas e dependências — regras conceituais

As camadas de Clean Architecture são **regras de dependência**, não uma estrutura de pastas obrigatória na raiz. Aplicam-se em **qualquer nível** (raiz, área, módulo).

| Camada | Responsabilidade | Pode depender de |
|--------|------------------|------------------|
| **domain** | Interfaces, tipos, entidades de domínio | — |
| **application** | Casos de uso, orquestração (sem I/O direto) | domain |
| **infrastructure** | HTTP, repositórios, implementações de interfaces | domain, application |
| **core** | Singletons, config global, guards | domain (evitar application/infrastructure) |
| **shared** | Componentes, directives, pipes reutilizáveis | domain, core |

- **Regra:** dependências apenas para dentro (camada externa depende da interna; domain não depende de ninguém).
- Estas camadas podem existir como pastas na **raiz** (para tipos/serviços transversais) ou **dentro de um módulo** (para lógica específica daquele módulo). A presença de `domain/` dentro de `areas/admin/gerenciamento-acessos/domain/` é válida e preferível quando o domínio é específico daquele módulo.

### Checklist — onde colocar um novo artefato

Duas perguntas em ordem:

**1. A quem pertence?**
- [ ] Pertence a uma área específica → `areas/<area>/<modulo>/`
- [ ] É transversal / genérico → `shared/` (como módulo em `shared/<modulo>/` ou artefato em `shared/components/`, `shared/directives/`)
- [ ] É singleton global (auth, config) → `core/`
- [ ] É interface/tipo transversal de domínio → `domain/` (raiz)

**2. Qual o papel?**
- [ ] Página (rota, storytelling UX) → `<local>/pages/<nome>/`
- [ ] Componente reutilizável → `<local>/components/` ou `shared/components/`
- [ ] Componente exclusivo de uma página → `<local>/pages/<pagina>/components/`
- [ ] Directiva → `<local>/directives/` ou `shared/directives/`
- [ ] Interface/tipo de domínio → `<local>/domain/` ou `domain/` (raiz)
- [ ] Caso de uso / orquestração → `<local>/application/` ou `<local>/services/`
- [ ] Implementação HTTP/repositório → `<local>/infrastructure/` ou `<local>/services/`
- [ ] Novo módulo coeso → nova pasta em `areas/<area>/<modulo>/` ou `shared/<modulo>/` com `pages/`, `components/`, rotas; considerar lazy loading.

---

## Convenções de componentes e módulos

- **Componentes:** usar `templateUrl` e `styleUrl` (não template inline em páginas com mais de 5 linhas).
- **Standalone:** preferir componentes standalone; módulos quando necessário para lazy loading ou agrupamento.
- **Lazy loading:** por feature/módulo nas rotas (`loadChildren`).
- **Trio de ficheiros:** `.component.ts`, `.component.html`, `.component.scss` para cada componente (exceto directivas).

---

## Serviços por camada (DIP)

Independentemente de onde vivem na árvore, os serviços seguem regras de camada:

- **domain:** apenas interfaces/abstrações (ex.: `IReportRepository`).
- **application:** serviços que orquestram casos de uso; **não** injetar `HttpClient`; dependem de interfaces do domain.
- **infrastructure:** implementações que chamam HTTP, repositórios concretos; implementam interfaces do domain/application.
- **core:** singletons (auth, config, logging); configuração de app.
- Componentes de página **não** injetam `HttpClient` diretamente; usam serviços de application ou infrastructure.

---

## Regras duras (violações no formato para agente)

Ao analisar código, violações devem ser reportadas assim: `**[PRINCÍPIO]** \`path:linha\` — descrição`.

- **[PERTENCIMENTO]** `src/app/` — artefato fora de qualquer área/módulo/shared sem justificativa; deve pertencer a uma área, módulo ou shared.
- **[CAMADA]** `src/app/areas/admin/foo/page.component.ts` — componente de página injeta HttpClient diretamente; deve usar serviço de application/infrastructure.
- **[CONVENÇÃO]** `src/app/areas/admin/foo/page.component.ts` — template inline com mais de 5 linhas; usar templateUrl.
- **[CAMADA]** `src/app/domain/` — camada domain contém implementação concreta (ex.: chamada HTTP); manter apenas interfaces/tipos.
- **[ESTRUTURA]** `src/app/areas/admin/foo/` — módulo sem pasta `pages/` ou `components/` quando há páginas/componentes soltos; agrupar conforme conceitos (módulo/página/componente).

---

## Acessibilidade (a11y)

- Semântica HTML: usar `<button>`, `<nav>`, `<header>`, `<main>`, `<label>` em vez de `<div>` com click.
- ARIA: quando HTML não basta, usar `[attr.aria-label]`, `role`, `aria-live` conforme necessidade.
- Teclado: todos os interactivos acessíveis por teclado; focos visíveis.
- Leitores de tela: conteúdo dinâmico anunciado (ex.: Angular CDK `LiveAnnouncer`); evitar só feedback visual.
- Referência: Angular a11y (angular.dev/best-practices/a11y), CDK a11y (FocusTrap, FocusMonitor).

---

## Signals vs RxJS

- **Signals:** estado síncrono e fino na UI (flags, computed); preferir para estado local de componente e derived state.
- **RxJS:** fluxos assíncronos (chamadas HTTP, WebSockets, eventos, operadores); manter para tudo o que é async.
- Interop: `toSignal()` para consumir Observable no template; `toObservable()` para aplicar operadores RxJS a um Signal; `takeUntilDestroyed(this.destroyRef)` para subscriptions em componentes (Angular 16+).

---

## Ciclo de vida

- Limpeza em `ngOnDestroy`: subscriptions, listeners, timers para evitar memory leaks.
- Preferir `takeUntilDestroyed(this.destroyRef)` em vez de unsubscribe manual quando possível.
- Directives e serviços com ciclo de vida: respeitar destruição e não reter referências a componentes destruídos.

---

## Typed forms

- Preferir reactive forms tipados (Angular 14+): `FormGroup<T>`, `FormControl<T>` com inferência ou genéricos.
- Evitar `UntypedFormGroup`/`UntypedFormControl`.

---

## Frontend independente do backend — central de ações (Command)

- O frontend deve funcionar sem o backend exceto no momento das requisições.
- **Central de ações de servidor:** um módulo/serviço central gere **todas** as requisições aos backends (design pattern Command: intenção serializável, fila, retry).
- Requisições ao backend são **não garantidas** (rede, indisponibilidade); tratar como ambiente fora do controle do frontend.
- Se o backend falhar ou estiver indisponível: **avisar o utilizador**, **persistir o comando** (ex.: fila local) e permitir **retry** mais tarde; **nunca** quebrar a página (ex.: erro não tratado que impeça navegação ou uso da UI).
- Se o backend concluir com sucesso a requisição: Permitir com click um hook para tela e/ou modal relevante da ação. (ex: terminou de inserir um registro? encaminhar para a tela de edição dele. terminou de carregar um relatio? encaminhar para a pagina de apresentação dele.)
- UX: central acessível (ex.: menu superior) onde o utilizador vê processos solicitados e pode continuar com o feedback quando o comando terminar por via de uma caixa de notificações.

---

## Checklist geral (agente IA)

- [ ] Spec/contrato/relatório/demanda disponível antes de implementar.
- [ ] **Pertencimento:** novos artefatos pertencem a uma área/módulo ou ao shared/core — nunca soltos na raiz sem justificativa.
- [ ] **Estrutura:** árvore reflete o produto (áreas → módulos → páginas → componentes); camadas Clean Arch como suporte dentro dessa estrutura.
- [ ] Componentes com templateUrl e styleUrl; sem HttpClient em componentes de página.
- [ ] Serviços por camada (DIP); interfaces no domain, implementações em infrastructure — regras válidas em qualquer nível da árvore.
- [ ] Compartilhamento entre módulos/áreas via abstrações no shared; sem cópia de código nem acoplamento direto; SOLID respeitado.
- [ ] A11y: semântica, ARIA quando necessário, teclado, feedback para leitores de tela.
- [ ] Estado: Signals para estado síncrono de UI; RxJS para async (HTTP, WebSockets).
- [ ] Subscriptions com `takeUntilDestroyed` ou limpeza em ngOnDestroy.
- [ ] Formulários tipados (typed forms).
- [ ] Requisições ao backend via central de ações; falha tratada (notificar, persistir comando, retry); páginas não quebradas por falha de rede.
