# Referência — Mestre Freire Angular

Extensão Angular do mapeamento achado → técnica.

---

## Regra de critério técnico (Angular)

- **Ficheiro:** `regras/angular-frontend.mdc`
- **Conteúdo relevante para refatoração:** camadas (domain, application, infrastructure, core, shared, features), dependências entre camadas, convenções de componentes (templateUrl, styleUrl), serviços por tipo (domínio, caso de uso, infraestrutura, core), regras duras (não HttpClient em componentes de página, não template inline em páginas com >5 linhas, não camadas vazias).

---

## DIP e camadas no Angular

Ao tratar achados que mencionem **DIP**, **Clean Architecture** ou **serviços por camada** em código Angular:

- **angular-frontend (serviços por camada):** domain (interfaces/abstrações), application (casos de uso, sem HttpClient), infrastructure (implementações HTTP, repositórios), core (singletons, config). Componentes em features usam serviços de application ou infrastructure, não HttpClient direto.
- Consultar `regras/angular-frontend.mdc` para o mapeamento completo e dependências permitidas.

---

## Testes no Angular

- **Ficheiros de teste:** não alterar `*.spec.ts`. A skill mestre-freire (e esta) mantém testes intocáveis.
- **Comando:** `npm run test` ou `ng test`. Executar após cada passo (ou lote coerente) de refatoração; a suíte deve permanecer verde.
- Se um teste falhar após refatoração, corrigir o **código de produção**, não o spec.
