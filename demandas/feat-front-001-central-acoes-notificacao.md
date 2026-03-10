# Central de Ações (Command Pattern) e Notificação Centralizada

## Metadados

| Campo | Valor |
|-------|-------|
| **ID** | FEAT-FRONT-001 |
| **Tipo** | Feature nova |
| **Prioridade** | Should (diretriz exige; sistema funciona sem) |
| **sourceArtifact** | `.github/reference/fonte-de-verdade-refactoring.md` — achados #6 e #7 |
| **Diretriz violada** | `.github/instructions/angular-frontend.instructions.md` §Frontend independente do backend |

---

## 1. Contexto

Cada componente faz requisições HTTP via ports e trata erros localmente de forma inconsistente. Não há fila central, retry automático nem feedback visual padronizado. A diretriz exige "Central de ações de servidor: Command serializável, fila, retry" e notificação centralizada.

**Problema atual:**
- `LoginFormComponent`: exibe `erro()` signal inline
- `AtualizarPosicaoComponent`: mensagem de erro inline por operação
- `ObraListaComponent`: erro ao listar tratado localmente
- `ConfigComponent`: trata erro de conexão localmente
- Zero padronização: cada tela decide como/se mostra erros

---

## 2. User Stories

**US-1**: Como operador, quero que erros de rede ou do servidor sejam exibidos numa notificação visual padronizada (toast) para saber imediatamente o que falhou.

**US-2**: Como operador, quero que ações falhadas possam ser reenviadas (retry) sem repetir manualmente a operação.

**Critérios de aceitação:**
1. Erros de qualquer requisição ao backend exibem toast no canto superior direito.
2. Toast de erro persiste até dismiss manual; toast de sucesso auto-dismiss em 5s.
3. Toast de erro oferece botão de retry.
4. Componentes não tratam erros inline — delegam para a central.
5. UI permanece navegável mesmo com backend indisponível.
6. Testes existentes continuam passando após migração.

---

## 3. Alterações necessárias

### Domain (types)
1. **Criar** `ServerCommand<TPayload, TResult>` — interface: id, type, payload, status, retryCount.
2. **Criar** `CommandHandler<T>` — interface abstrata: `handle(command): Observable<TResult>`.
3. **Criar** `AppNotification` — tipo: id, type (success/error/info), message, action?, dismissed.

### Core (services)
4. **Criar** `ActionQueueService` — singleton: dispatch(command), executa via handler, retry com backoff (max 3, delay 1s/2s/4s), expõe signal `queue()`.
5. **Criar** `NotificationService` — singleton: `success(msg)`, `error(msg, retryFn?)`, `info(msg)`. Signal `notifications()`. Auto-dismiss sucesso 5s.

### Shared (UI)
6. **Criar** `ToastComponent` — standalone, OnPush. Consome `NotificationService.notifications()`. Renderiza toasts empilhados. `role="alert"`, `aria-live="assertive"` para erros.

### Application (handlers)
7. **Criar** command handlers concretos: `login.command.ts`, `lista-obras.command.ts`, `atualizar-posicao.command.ts` — delegam para ports existentes.

### Integração
8. **Alterar** `AppComponent` — adicionar `<app-toast />` ao template.
9. **Alterar** componentes de feature — substituir calls diretas a ports por `ActionQueueService.dispatch()`. Remover tratamento de erro inline.

---

## 4. Requisitos técnicos/metodológicos aplicáveis

- **TDD**: testes escritos antes da implementação (quadro-de-recompensas → mercenario).
- **DDD**: types no domain layer; services no core; UI no shared.
- **SOLID**:
  - SRP: `ActionQueueService` = fila/execução; `NotificationService` = feedback visual.
  - OCP: novos handlers adicionados sem alterar ActionQueueService.
  - DIP: componentes dependem de abstrações (CommandHandler), não de HTTP.
- **Pirâmide de testes**: unitários (services, component), integração (command → handler → notification), E2E (toast aparece em erro/sucesso).
- **Angular**: standalone, OnPush, signals para estado, RxJS para async, `takeUntilDestroyed`, a11y (ARIA).
- **Gestão de memória**: fila limitada (últimos 50 comandos); subscriptions limpas via `DestroyRef`.

## 5. Dependências e riscos

| Risco | Severidade | Mitigação |
|-------|------------|-----------|
| Migração dos componentes pode quebrar testes existentes | Média | Migração incremental por componente; rodar suíte após cada um |
| Retry em mutações pode causar duplicação (ex.: criar obra 2x) | Média | Retry apenas para comandos idempotentes; para mutações, confirmar com utilizador |
| Fila perde-se ao recarregar página | Baixa | Aceitável para app on-premise pessoal; evolução futura com localStorage |

## 6. Definição de pronto

- [ ] `ActionQueueService` e `NotificationService` com testes unitários.
- [ ] `ToastComponent` renderiza e faz dismiss.
- [ ] Todos os componentes migrados para usar central de ações.
- [ ] Zero tratamento de erro inline nos componentes.
- [ ] Testes unitários, integração e E2E passam.
- [ ] `ng build --configuration production` sem erros.
- [ ] PR aprovado e CI verde.
