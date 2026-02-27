# Retrospectiva: Módulo Acompanhamento de Obras + Autenticação

**Período:** Fevereiro de 2026  
**Branches:** `feature/backend-modulo-acompanhamento-obras`  
**PR:** [#7 — feat: módulo backend de acompanhamento de obras](https://github.com/Lucas3995/DiarioDeBordo/pull/7)

---

## 1. O que foi construído

Ao longo de duas grandes tarefas, foram implementados:

### Módulo 1 — Acompanhamento de Obras (Frontend)
- Camadas `domain/`, `application/`, `infrastructure/` e `features/obras/` no Angular
- `IListaObrasPort`, `ListaObrasService`, `ListaObrasMock` e posteriormente `ListaObrasHttp`
- Tela de listagem paginada (`/obras`) com campos: Nome, Tipo, Última atualização (relativa + tooltip `dd/MM/yyyy`), Posição (com unidade do tipo: capítulo/episódio/minuto), Previsão
- Paginação com pageSize 10/25/50/100
- TDD aplicado: testes escritos antes da implementação

### Módulo 1 — Acompanhamento de Obras (Backend)
- Entidade `Obra` no Domain com `TipoObra` (enum)
- Query CQRS `GetObrasAcompanhamentoQuery` com handler e validator
- `IObraLeituraRepository` na camada Application (Clean Architecture)
- `ObraLeituraRepository` na Persistence com EF Core
- Migration `CriarTabela Obras` + seed de 12 obras (equivalentes ao mock do frontend)
- `GET /api/obras` com autenticação JWT Bearer
- Containerização: Dockerfile multi-stage + entrada no `docker-compose.yml`

### Autenticação
- Entidade `Usuario` no Domain com `Login`, `SenhaHash`, `Ativo`, `Requer2FA`, `Perfil`
- `LoginCommand` / `LoginResponse` / `LoginCommandHandler` (CQRS)
- `ITokenService`, `IPasswordHasher`, `IUsuarioRepository` na Application
- `TokenService` (JWT HS256, 8h) e `BcryptPasswordHasher` (BCrypt wf12) na Infrastructure
- Migration `CriarTabelaUsuarios` + seed do admin (`admin`/`camaradinha@123`, `Requer2FA=false`)
- `POST /api/auth/login` (público) retorna 200/401/400
- Frontend: `IAuthPort`, `AuthHttp`, `AuthService`, `LoginFormComponent` integrado em Configurações

---

## 2. Correções do usuário e aprendizados da IA

### 2.1 "Qual arquivo você alterou no seu ajuste?"

**Contexto:** Ao resolver o erro `no pg_hba.conf entry for host "172.19.0.2"`, descrevi a correção verbalmente mas não havia criado/modificado nenhum arquivo no repositório Git. O usuário notou que nenhum arquivo aparecia como modificado no `git status`.

**Correção:** Alterar `COMO_RODAR_BACK_E_FRONT.txt` efetivamente — com `StrReplace` — para refletir o novo range de IP (`172.0.0.0/8` em vez de `172.17.0.0/16`). Apenas descrever a mudança não é suficiente; o arquivo deve ser editado de fato.

**Aprendizado:** Quando uma correção exige mudança em arquivo, executar a ferramenta de edição imediatamente. Não relatar a correção como feita sem evidência de ferramentas chamadas.

---

### 2.2 Subnet Docker dinâmica: `172.17.0.0/16` vs `172.0.0.0/8`

**Contexto:** A instrução inicial no `COMO_RODAR_BACK_E_FRONT.txt` usava `172.17.0.0/16` como range permitido no `pg_hba.conf`. Na prática, o Docker Compose aloca subnets dinamicamente (`172.17.x`, `172.18.x`, `172.19.x`, etc.), e o container do backend recebia IP `172.19.0.2`, que caía fora do range declarado.

**Erro original:**
```
Npgsql.PostgresException: 28000: no pg_hba.conf entry for host "172.19.0.2"
```

**Correção:** Usar `172.0.0.0/8` para cobrir todos os ranges que o Docker Compose possa alocar.

**Orientação do usuário:** A regra deve cobrir a realidade do ambiente, não apenas o caso mais comum.

**Aprendizado:** Ao configurar `pg_hba.conf` para acesso de containers Docker, sempre usar `172.0.0.0/8` (ou equivalente amplo) em vez de um range restrito de uma única subnet. O Docker Compose não garante a mesma subnet entre execuções.

---

### 2.3 Paths do Dockerfile relativos ao contexto do build

**Contexto:** O `docker-compose.yml` usava `context: ..` (raiz do repositório) ao referenciar o backend. O Dockerfile original usava `COPY src/ src/` assumindo que o contexto seria `backend/`, o que causava:

```
ERROR: failed to solve: ... "/src": not found
```

**Correção:** Todos os caminhos no `COPY` do Dockerfile devem ser relativos ao contexto de build declarado no `docker-compose.yml`. Como o contexto é a raiz, os caminhos ficaram `backend/src/DiarioDeBordo.X/DiarioDeBordo.X.csproj`.

**Aprendizado:** Sempre verificar `context:` no `docker-compose.yml` antes de escrever um Dockerfile. O contexto define o "diretório de trabalho" do builder — não necessariamente onde o Dockerfile está.

---

### 2.4 `dotnet publish --no-restore` quebrando após `dotnet restore` por projeto (não por solution)

**Contexto:** O Dockerfile rodava `dotnet restore DiarioDeBordo.Api.csproj` (projeto único) e depois `dotnet publish --no-restore`. Isso causava falha porque `--no-restore` impedia a resolução de pacotes de outros projetos da solution (ex.: analyzers).

**Erro:**
```
error NETSDK1064: Package Microsoft.CodeAnalysis.Analyzers, version 3.3.4 was not found.
```

**Correção:** Trocar para `dotnet restore DiarioDeBordo.sln` (restaura toda a solution) e remover `--no-restore` do `dotnet publish`.

**Aprendizado:** O restore por projeto não garante que todos os pacotes transitivos e de analyzers estejam presentes para o publish. O restore por solution é mais seguro em ambientes multi-projeto.

---

### 2.5 `dotnet restore` por solution exige que todos os projetos referenciados existam no contexto

**Contexto:** Após trocar para `dotnet restore DiarioDeBordo.sln`, o build falhava porque o `.sln` referencia `DiarioDeBordo.Tests.csproj`, que não havia sido copiado para o estágio de build do Docker.

**Erro:**
```
error MSB3202: The project file ".../DiarioDeBordo.Tests.csproj" was not found
```

**Correção:** Adicionar `COPY backend/src/DiarioDeBordo.Tests/DiarioDeBordo.Tests.csproj` no Dockerfile (o projeto de testes não entra na imagem final, mas o `.csproj` precisa estar presente para o restore da solution passar).

**Aprendizado:** Ao usar restore por solution, todos os arquivos `.csproj` referenciados no `.sln` precisam estar no contexto do builder — mesmo que o projeto não vá para a imagem de runtime.

---

### 2.6 Race condition em testes de integração com banco InMemory

**Contexto:** O teste `GET_obras_ComToken_DeveRetornarJsonComItemsETotalCount` falhava intermitentemente com `TotalCount = 1` esperando `>= 3`. A causa era que dois testes compartilhavam o mesmo banco InMemory e `PopularObrasAsync` só populava se a tabela estivesse vazia — mas às vezes outro teste já havia inserido 1 registro.

**Correção:** Implementar `IAsyncLifetime` no `ObrasControllerTests` e chamar `PopularObrasAsync(3)` no `InitializeAsync()`, garantindo estado limpo e populado antes de cada teste, não apenas uma vez ao criar o fixture.

**Aprendizado:** Testes de integração com `IClassFixture` compartilham estado. Quando o estado precisa ser resetado entre testes, usar `IAsyncLifetime` com `InitializeAsync` e `DisposeAsync` para garantir isolamento.

---

### 2.7 Downgrade de pacotes NuGet ao adicionar referência de projeto

**Contexto:** Ao adicionar `DiarioDeBordo.Application` como referência em `DiarioDeBordo.Infrastructure`, o Application já dependia de `MediatR 14` que requer `Microsoft.Extensions.DependencyInjection.Abstractions >= 10.0.0`. O Infrastructure tinha declarado explicitamente a versão `9.0.3`, causando conflito `NU1605`.

**Correção:** Atualizar no Infrastructure as versões de `Microsoft.Extensions.DependencyInjection.Abstractions` e `Microsoft.Extensions.Logging.Abstractions` para `10.0.0` para alinhar com a versão exigida transitivamente pelo MediatR.

**Aprendizado:** Ao adicionar referências entre projetos, verificar se há conflitos de versão de pacotes compartilhados. A regra `NU1605` tratada como erro bloqueia o build corretamente — não suprimir, resolver.

---

### 2.8 Regra de 2FA: "não implementar autenticação que dependa apenas de senha"

**Contexto:** A workspace rule `protecao-dados-lgpd-seguranca.mdc` proíbe explicitamente fluxos de login que dependam apenas de senha. No início do design da autenticação, a solução mais simples seria `login + senha → token`. O usuário apontou que isso violaria a regra.

**Orientação do usuário:** A arquitetura deve ser **2FA-ready** desde o início: campo `Requer2FA` na entidade `Usuario`, e a resposta do login deve indicar se o segundo fator é necessário. O admin pode começar com `Requer2FA = false` para desenvolvimento, mas a estrutura deve existir.

**Aprendizado:** Regras hard das workspace rules têm precedência sobre conveniência de implementação. "2FA-ready" não significa "2FA implementado agora", mas significa que a arquitetura não fecha a porta para ele — entidade com flag, resposta com discriminador, handler que respeita o flag.

---

### 2.9 Anti-enumeração de usuários no login

**Contexto:** Uma implementação ingênua de login retorna mensagens diferentes para "login não encontrado" e "senha incorreta", permitindo que um atacante enumere quais logins existem no sistema.

**Orientação implícita via regras:** Os testes validam explicitamente que a mensagem retornada para "login inexistente" é **idêntica** à mensagem de "senha incorreta" (`"Credenciais inválidas."`).

**Aprendizado complementar (timing attack):** Para impedir que o tempo de resposta revele a ausência do login (timing attack), o handler executa `_passwordHasher.Verificar()` mesmo quando o usuário não existe, passando um hash fictício. Isso iguala o tempo de processamento dos dois casos.

---

### 2.10 Qualidade dos testes: "de nada adianta os testes passarem se não forem abrangentes"

**Contexto:** Após solicitar a auto-checagem, o usuário enfatizou que testes que passam mas são genéricos demais (apenas `toBeTruthy`, apenas status code, sem verificar conteúdo concreto) não têm valor.

**Orientação do usuário:** Cada assertion deve validar um comportamento concreto e necessário. Exemplos:
- Não apenas `status 200` — verificar também `token` não vazio, `expiresAt` no futuro, `sucesso = true`
- Não apenas `toBeTruthy()` — verificar que o DOM exibe a mensagem correta
- O teste de "mesma mensagem" entre senha errada e login inexistente protege a regra de anti-enumeração

**Aprendizado:** A qualidade do teste é tão importante quanto a cobertura. Um teste que verifica apenas que algo "não quebrou" não protege regras de negócio. Cada teste deve ter um cenário claro e assertions que falhariam se a regra de negócio fosse violada.

---

### 2.11 Seed de dados equivalente ao mock do frontend

**Contexto:** O plano do backend especificava que o seed de Development deveria ser equivalente ao mock do frontend, para que a validação visual fosse consistente.

**Orientação:** Ao criar `ObrasSeed.cs`, os 12 registros deveriam corresponder exatamente às obras usadas no `ListaObrasMock`, com os mesmos nomes, tipos, posições e dados de previsão — não dados genéricos de teste.

**Aprendizado:** Seed de Development não é apenas para "ter dados" — é para permitir validação visual e comparação entre a versão mockada e a versão real. Cópias divergentes geram confusão na validação.

---

## 3. Orientações que estavam distantes dos READMEs

### 3.1 Formato de data: `dd/MM/yyyy` e fuso UTC-3

O `README.md` raiz mencionava genericamente "formato de data" sem especificar. O usuário orientou explicitamente:
- **Formato:** `dd/MM/yyyy` (padrão brasileiro)
- **Fuso:** GMT-3 / UTC-3 (America/Sao_Paulo) para exibição no frontend
- **Armazenamento:** UTC no banco (boas práticas)
- **Conversão:** feita no frontend na exibição, não no backend

Isso foi incorporado ao `ObraAcompanhamentoListItemDto` (armazena UTC) e à lógica de exibição do frontend.

### 3.2 PostgreSQL na máquina hospedeira (não no Docker Compose)

O `README.md` original mencionava "PostgreSQL local ou via Docker" de forma genérica. O usuário orientou:
- O PostgreSQL está **instalado na máquina hospedeira**, não em container
- O backend em Docker acessa via `host.docker.internal`
- O `docker-compose.yml` **não deve incluir serviço de banco**
- Requer configuração de `listen_addresses = '*'` e regra em `pg_hba.conf`

O `COMO_RODAR_BACK_E_FRONT.txt` foi criado especificamente para documentar esses passos que não estavam em nenhum README.

### 3.3 Credenciais do PostgreSQL

O usuário informou fora do README: usuário `postgres`, senha `39953995`. Isso foi usado nas variáveis de ambiente do `docker-compose.yml` e nas instruções do `COMO_RODAR_BACK_E_FRONT.txt` — mas a senha **não deve ser commitada** em arquivos de configuração de produção.

**Aprendizado:** Credenciais de desenvolvimento ficam em `.env` (não commitado) ou são passadas por variável de ambiente. O `docker-compose.yml` deve usar `${POSTGRES_PASSWORD}` ou equivalente.

### 3.4 Endpoint de auth como pré-requisito para `/api/obras`

O `README.md` original (antes das atualizações) não mencionava que `GET /api/obras` exige autenticação — estava documentado apenas como "Lista obras paginadas". O usuário criou o fluxo completo:

1. `POST /api/auth/login` → obtém token
2. Configurar token no frontend (via formulário de login ou manualmente)
3. Acessar `/obras`

O `COMO_RODAR_BACK_E_FRONT.txt` foi atualizado para incluir o Passo 4 de autenticação antes do acesso às obras.

---

## 4. Padrões confirmados que funcionaram bem

- **Clean Architecture estrita:** `Application` depende de `IUsuarioRepository` (interface), não de `DiarioDeBordoDbContext`. Funcionou corretamente para TDD (mock nos testes unitários, implementação EF nos de integração).
- **CQRS isolado:** `LoginCommand` separado de `GetObrasAcompanhamentoQuery`. Sem "save genérico".
- **TDD com `IAsyncLifetime`:** Pattern correto para testes de integração com estado mutável.
- **Anti-enumeração por design:** mensagem genérica + timing equalization no handler.
- **Dockerfile multi-stage:** build SDK + runtime ASP.NET — imagem final sem SDK.

---

## 5. Itens para backlog / próximas iterações

| Item | Origem |
|------|--------|
| Habilitar `Requer2FA = true` para o admin antes de produção | Regra de segurança / orientação do usuário |
| Implementar TOTP como segundo fator | Arquitetura 2FA-ready aguarda implementação |
| Credenciais do banco via `.env` (não hardcoded no compose) | DevSecOps |
| Senha do PostgreSQL movida para variável de ambiente no CI/CD | DevSecOps |
| Modal de detalhe da obra (query `ObterDetalhesObra`) | Planejado desde o início, fora do escopo atual |
| Commands de escrita (criar obra, atualizar posição) | Fora do escopo do módulo 1 |
| i18n e acessibilidade (tooltip, aria-labels) | Registrado como lacuna na Fase 3 |

---

*Documento gerado em 27/02/2026 como registro de aprendizados e orientações desta iteração de desenvolvimento.*
