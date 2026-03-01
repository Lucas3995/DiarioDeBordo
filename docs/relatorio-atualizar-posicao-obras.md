# Relatório de Alterações para Demanda — Atualizar progressão e posição nas obras

## Resumo da demanda

Permitir ao usuário atualizar a progressão nas obras: informar a obra por nome ou código (id), informar a nova posição (capítulo/episódio/parte) e opcionalmente a data da última atualização; exibir prévia (valor antigo → valor novo) antes de salvar; criar a obra se não existir quando nome for informado. As alterações devem refletir nos campos PosicaoAtual e DataUltimaAtualizacaoPosicao, já exibidos na listagem (coluna "Última Atualização" e posição atual).

## Âmbito da análise

- Backend: Domain (Obra), Application (Commands, Queries, Handlers, Validators, repositório de escrita), Persistence (implementação do repositório de escrita), Api (ObrasController).
- Frontend: feature obras (rotas, componente de atualização de posição, serviço HTTP).
- Premissas: foco em posição e data da última atualização; link e comentários por parte ficam para demanda futura; prévia via GET da obra atual + exibição no frontend; "código" = Id (Guid) da obra.

## Alterações necessárias

### 1. Domínio — método AtualizarPosicao na entidade Obra

**Onde:** backend, DiarioDeBordo.Domain, Obras/Obra.cs  
**Tipo:** Alterar  
**Descrição:** Adicionar método público `AtualizarPosicao(int novaPosicao, DateTime? dataUltimaAtualizacao = null)` que valida novaPosicao (não negativa, reutilizando regra existente), atualiza a propriedade PosicaoAtual e atualiza DataUltimaAtualizacaoPosicao com o valor informado ou DateTime.UtcNow quando dataUltimaAtualizacao for nulo. Garantir que a data seja armazenada em UTC (SpecifyKind).  
**Requisito atendido:** Atualizar posição e última atualização na entidade.

---

### 2. Application — interface de repositório de escrita para obras

**Onde:** backend, DiarioDeBordo.Application, Obras (nova pasta ou subpasta para escrita)  
**Tipo:** Criar  
**Descrição:** Criar interface (ex.: IObraEscritaRepository) com métodos: ObterPorIdAsync(Guid id, CancellationToken), ObterPorNomeAsync(string nome, CancellationToken), AdicionarAsync(Obra, CancellationToken), AtualizarAsync(Obra, CancellationToken). A interface deve ser consumida pelos handlers de Command e Query de prévia.  
**Requisito atendido:** Persistência de obra por id/nome e atualização.

---

### 3. Persistence — implementação do repositório de escrita de obras

**Onde:** backend, DiarioDeBordo.Persistence, Obras/  
**Tipo:** Criar  
**Descrição:** Implementar a interface do item 2 usando DbContext (com tracking para update). ObterPorIdAsync e ObterPorNomeAsync devem usar o DbSet<Obra> existente; AdicionarAsync e AtualizarAsync devem persistir no mesmo DbContext. Registrar a implementação em Persistence/DependencyInjection.cs.  
**Requisito atendido:** Resolver e persistir obra por id ou nome.

---

### 4. Application — Command AtualizarPosicaoObra

**Onde:** backend, DiarioDeBordo.Application, Obras/AtualizarPosicao/ (ou equivalente)  
**Tipo:** Criar  
**Descrição:** Criar AtualizarPosicaoObraCommand com: IdObra (Guid?), NomeObra (string?), NovaPosicao (int), DataUltimaAtualizacao (DateTime?), CriarSeNaoExistir (bool); e para criação: Nome (string), TipoObra (TipoObra), OrdemPreferencia (int). Regra: pelo menos um de IdObra ou NomeObra deve ser informado. Handler: obter obra por id (se IdObra informado) ou por nome (se NomeObra informado); se não existir e CriarSeNaoExistir com Nome preenchido, criar nova Obra e AdicionarAsync; senão chamar obra.AtualizarPosicao(NovaPosicao, DataUltimaAtualizacao) e AtualizarAsync. Response com Id da obra e indicador Criada (bool).  
**Requisito atendido:** Atualizar posição por nome ou código; criar obra se não existir.

---

### 5. Application — Validator do AtualizarPosicaoObraCommand

**Onde:** backend, DiarioDeBordo.Application, Obras/AtualizarPosicao/  
**Tipo:** Criar  
**Descrição:** FluentValidation para o command: pelo menos um de IdObra ou NomeObra obrigatório; NovaPosicao >= 0; quando CriarSeNaoExistir for true e for caso de criação, Nome não vazio, TipoObra informado, OrdemPreferencia >= 0. Registrar no DependencyInjection da Application.  
**Requisito atendido:** Validação do comando de atualização.

---

### 6. Application — Query GetObraPorIdOuNome (prévia)

**Onde:** backend, DiarioDeBordo.Application, Obras/ObterPorIdOuNome/ (ou equivalente)  
**Tipo:** Criar  
**Descrição:** Criar GetObraPorIdOuNomeQuery com Id (Guid?) e Nome (string?). Handler: obter obra por id se Id informado, senão por nome se Nome informado; retornar DTO com dados atuais (Id, Nome, Tipo, PosicaoAtual, DataUltimaAtualizacaoPosicao, OrdemPreferencia). Se não encontrar, retornar null (API retornará 404). Validator: pelo menos um de Id ou Nome obrigatório.  
**Requisito atendido:** Prévia: exibir estado atual da obra antes de salvar.

---

### 7. API — Endpoints GET obra (prévia) e PATCH/PUT posição

**Onde:** backend, DiarioDeBordo.Api, Controllers/ObrasController.cs  
**Tipo:** Alterar  
**Descrição:** Adicionar GET /api/obras/{id} que envia GetObraPorIdOuNomeQuery com Id e retorna 200 com DTO ou 404. Adicionar GET /api/obras/por-nome?nome=... para busca por nome (ou incluir nome no query do GET por id). Adicionar PATCH /api/obras/posicao (ou PUT) que recebe no body o payload do AtualizarPosicaoObraCommand e envia o command; retornar 200 com response (id, criada) ou 400/404 conforme validação e existência. Manter [Authorize] em todas as rotas.  
**Requisito atendido:** API para prévia e para atualizar posição.

---

### 8. Frontend — Serviço HTTP para prévia e atualização de posição

**Onde:** frontend, app (infrastructure ou application), obras  
**Tipo:** Criar / Alterar  
**Descrição:** Criar ou estender serviço que chama GET /api/obras/{id} e GET por nome (para prévia) e PATCH/PUT /api/obras/posicao com body (nova posição, data opcional, criar-se-não-existir, dados para criação). Usar token já configurado (interceptor ou config).  
**Requisito atendido:** Frontend poder obter prévia e enviar atualização.

---

### 9. Frontend — Componente e rota "Atualizar posição"

**Onde:** frontend, app/features/obras/  
**Tipo:** Criar  
**Descrição:** Criar componente (tela ou modal) com: campo para código (id) ou nome da obra; campo nova posição; campo opcional data da última atualização (default hoje); opção "criar se não existir" com nome, tipo e ordem quando aplicável; botão "Ver prévia" que chama GET e exibe antigo → novo (posição e data); botão "Salvar" que envia PATCH/PUT e em sucesso atualiza lista ou redireciona. Adicionar rota em obras.routes.ts (ex.: atualizar-posicao) e link/botão na lista de obras (ex.: "Atualizar posição" ou uso do "Ver mais").  
**Requisito atendido:** Fluxo completo de atualizar posição com prévia.

---

### 10. Application — Registro de validators e handlers

**Onde:** backend, DiarioDeBordo.Application, DependencyInjection.cs  
**Tipo:** Alterar  
**Descrição:** Registrar IRequestValidator para AtualizarPosicaoObraCommand e GetObraPorIdOuNomeQuery no FluentValidationRequestValidatorAdapter (ou equivalente) para que o ValidationBehavior execute as validações.  
**Requisito atendido:** Pipeline de validação dos novos casos de uso.

---

## Resumo executivo

- Total de itens de alteração: 10  
- Por tipo: Criar (6), Alterar (4), Remover (0), Integrar (0)  
- Dependências: 1 (Domínio) não depende de outros; 2 e 3 (repositório) permitem 4 e 6; 4 e 5 dependem de 1 e 2; 6 depende de 2; 7 depende de 4 e 6; 8 e 9 dependem de 7; 10 integra 4 e 6 ao pipeline.
