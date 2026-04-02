# Tabela STRIDE — Ameaças e Mitigações

**Referência:** `overview.md` para escopo; `dfd-nivel-1.md` para DFDs por subsistema.
**Base:** Expandida do Apêndice C dos Padrões Técnicos v4.

## Tabela STRIDE

| ID | Categoria | Ativo em Risco | Ameaça | Mitigação Específica | Pentest Ref |
|---|---|---|---|---|---|
| T-01 | **S** Spoofing | Sessão de usuário | Replay de sessão entre reinicializações — atacante reutiliza token de sessão de execução anterior | Token de sessão invalidado ao fechar a aplicação; sessão reinicia com autenticação obrigatória; GUID de sessão não persistido em disco | Phase 11 — SEG-05: testar persistência de sessão após reinicialização |
| T-02 | **S** Spoofing | Identidade do usuário | Bypass de autenticação via manipulação direta de memória ou injeção de dependência | `IUsuarioAutenticadoProvider` verificado na camada de serviço (não apenas UI); sem caminho de código que aceite `usuarioId` não autenticado; sessão só existe em DI container, não em variável global mutável | Phase 11 — SEG-05: injeção de provider falso |
| T-03 | **T** Tampering | Banco de dados | Injeção SQL via campo de busca, filtro ou importação de dados | EF Core com queries parametrizadas exclusivamente; `BannedSymbols.txt` proíbe `string.Format`, concatenação de strings em queries e `FromSqlRaw` com interpolação | Phase 11 — SEG-05: SQLi em todos os campos de entrada |
| T-04 | **T** Tampering | Processo (memória/CPU) | Arquivo de importação malicioso — JSON bomb, tamanho excessivo, nesting infinito | Verificação de tamanho antes do parse (limite configurável, default 50MB); limite de profundidade de nesting JSON; checksum de integridade antes de processar; rejeitar arquivos sem estrutura esperada | Phase 11 — SEG-05: submeter JSON bomb e ZIP bomb |
| T-05 | **T** Tampering | Parser XML/RSS | XML External Entity (XXE) via feed RSS malicioso | `DtdProcessing.Prohibit` em **todo** parsing XML/RSS — sem exceção; `XmlReaderSettings.ProcessInlineSchema = false`; proibir `XmlResolver` remoto | Phase 11 — SEG-05: XXE via feed RSS local |
| T-06 | **T** Tampering | Rede interna / dados locais | SSRF via URL de fonte externa — atacante usa a aplicação para fazer requests a serviços internos | Validação **pós-resolução DNS** (não apenas hostname): rejeitar IPs privados (10.x, 172.16–31.x, 192.168.x, 127.x, ::1, 169.254.x); whitelist de protocolos (`https`, `http`, `file`); nunca confiar apenas no hostname sem resolver o IP | Phase 11 — SEG-05: URL apontando para localhost e ranges RFC 1918 |
| T-07 | **T** Tampering | OS (Process.Start) | Injeção de protocolo via `Process.Start` — URL com `javascript:`, `data:`, `vbscript:` executada como app padrão | Whitelist estrita de protocolos (`https`, `http`, `file`) antes de qualquer `Process.Start`; rejeitar e logar protocolos não listados; sem fallback silencioso | Phase 11 — SEG-05: testar URLs com protocolos proibidos |
| T-08 | **R** Repudiation | Auditoria de ações | Negação de ação do usuário — "não apaguei esse conteúdo", "não fiz essa marcação" | `HistoricoAcao` imutável por design (append-only, sem UPDATE/DELETE); cada entrada tem timestamp UTC, tipo de ação e `usuarioId`; logging auditável em nível de serviço | Phase 11 — SEG-05: verificar imutabilidade de histórico no banco |
| T-09 | **I** Information Disclosure | Dados de outros usuários | Cross-user data leak — usuário A acessa dados do usuário B por ausência de filtro | `WHERE usuario_id = @usuarioId` obrigatório em **toda** query que acessa dados do usuário; `BannedSymbols.txt` para detectar queries de listagem sem filtro de `usuarioId`; testes de isolamento entre usuários em CI | Phase 11 — SEG-05: criar dois usuários, testar acesso cruzado |
| T-10 | **I** Information Disclosure | Credenciais do banco de dados | Credenciais PostgreSQL em plaintext — em `appsettings.json`, logs ou variáveis de ambiente não cifradas | Credenciais somente no DPAPI (Windows) ou libsecret (Linux) — nunca em arquivos de configuração ou variáveis de ambiente; `BannedSymbols.txt` proíbe logging de strings contendo "password" ou "senha"; connection string montada em memória e zerada após uso | Phase 11 — SEG-05: verificar disco e logs após instalação |
| T-11 | **I** Information Disclosure | Dados sensíveis em logs | PII, senhas ou tokens aparecem em logs de erro — exposição em arquivos de log | Regras de logging: nenhum campo marcado `[Sensitive]` em logs; sem logging de objetos completos com senhas; `ILogger` configurado para filtrar dados sensíveis; review de logs em pentest | Phase 11 — SEG-05: inspecionar logs após operações de autenticação |
| T-12 | **I** Information Disclosure | Existência de área admin | Admin area discovery por não-admins — usuário sem role Admin descobre que área admin existe | Área admin **inexistente** para usuários sem role Admin em **todas** as camadas: UI não renderiza (sem "acesso negado" que confirme existência), service layer retorna `NotFound` (não `Forbidden`) para rotas admin acessadas por não-admins | Phase 11 — SEG-05: navegar como usuário comum, checar respostas |
| T-13 | **D** Denial of Service | Processo (rede) | Payload RSS gigante bloqueia processo ou esgota memória | Limite 5MB por payload (lido em stream, não buffer completo); timeout 10s por request; circuit breaker após 5 falhas consecutivas em uma fonte; sem retry automático ilimitado | Phase 11 — SEG-05: servidor local retornando resposta >5MB e resposta lenta |
| T-14 | **D** Denial of Service | Banco de dados / processo | Consulta sem paginação retorna dezenas de milhares de registros — congestão de memória e banco | `PaginatedList<T>` obrigatório em todas as listagens; testes falham se handler retornar lista não paginada; `BannedSymbols.txt` para `.ToList()` sem paginação em queries de listagem | Phase 11 — SEG-05: popular banco com 100k registros, checar consumo de memória |
| T-15 | **E** Elevation of Privilege | Controle de acesso (RBAC) | Consumidor acessa funcionalidades restritas a admin — role bypass via manipulação de UI ou request direto | RBAC verificado na **camada de serviço** (não apenas UI); `IUsuarioAutenticadoProvider.RolesAtuais` verificado antes de toda operação restrita; sem path de código que aceite role implícita | Phase 11 — SEG-05: testar acesso a operações de admin como consumidor |
| T-16 | **E** Elevation of Privilege | Processo do sistema | Escalada de privilégios via binário comprometido — atacante substitui executável por versão maliciosa | Code signing obrigatório em todos os binários distribuídos (Velopack); verificação de assinatura antes de aplicar atualização; SHA-256 do instalador publicado no canal de distribuição | Phase 11 — SEG-05: verificar cadeia de assinatura dos binários |

---

## Rastreabilidade de Mitigações

| Mitigação técnica | Implementada em | Verificada por |
|---|---|---|
| EF Core queries parametrizadas | `DbContext` de todos os módulos | Análise estática (BannedSymbols.txt) em CI a partir de Phase 2 |
| `DtdProcessing.Prohibit` | `IAdaptadorPlataforma` (Module.IntegracaoExterna) | Testes de integração do adaptador RSS (Phase 5) |
| Anti-SSRF DNS validation (IPs privados) | `IAdaptadorPlataforma` | Testes com URLs apontando para 127.x, 10.x, 192.168.x (Phase 5) |
| DPAPI/libsecret para credenciais | Módulo de configuração / startup da aplicação | Pentest local — verificar disco e processos (Phase 11) |
| `Argon2id` para hashing de senha | Handler de autenticação (Module.Identidade) | Testes de autenticação (Phase 9) |
| `CryptographicOperations.FixedTimeEquals()` | Handler de verificação de senha | Code review + pentest de timing (Phase 11) |
| `CryptographicOperations.ZeroMemory()` | Handlers de autenticação, qualquer código com credencial em memória | Code review + pentest de heap dump (Phase 11) |
| BannedSymbols.txt (Roslyn analyzer) | Arquivo de análise estática — src/DiarioDeBordo.Analyzers/ | CI pipeline — build falha se violado (Phase 2+) |
| `WHERE usuario_id = @usuarioId` em toda query | `IConteudoRepository` + todos os repositórios | Testes de isolamento cross-user (Phase 2+) |
| Whitelist de protocolos (Process.Start) | Module.Reproducao — handler de abertura externa | Testes com URLs maliciosas (Phase 8) |
| Limite 5MB + timeout 10s | `HttpClient` configurado em Module.IntegracaoExterna | Testes de carga/timeout (Phase 5) |
| Área admin invisível a não-admins (Information Disclosure) | Module.Identidade + todos os ViewModels com conteúdo admin | Testes de autorização (Phase 9) |
| Code signing de binários | Pipeline de build + Velopack | Verificação de assinatura em pentest (Phase 11) |

---

## Pentest checklist (Phase 11 — SEG-05)

Superfícies a cobrir no pentest full scope:

- [ ] **Banco de dados local** (PostgreSQL localhost:15432): acesso por outras aplicações do sistema, SQLi em todos os campos de entrada, isolamento cross-user (T-03, T-09)
- [ ] **Credenciais em memória**: análise de heap dump após autenticação (T-10)
- [ ] **Adaptadores de rede**: SSRF com IPs privados e loopback, XXE via feed RSS malicioso, payloads acima de 5MB, timeout >10s (T-05, T-06, T-13)
- [ ] **Importação de arquivos**: JSON bomb, nesting profundo, arquivo acima do limite, checksum inválido (T-04)
- [ ] **Process.Start**: URLs com `javascript:`, `data:`, `vbscript:`, `ftp:`, `smb:` (T-07)
- [ ] **Cross-user data isolation**: criar dois usuários, tentar acessar dados do outro (T-09)
- [ ] **Admin area**: acessar rotas admin como usuário consumidor, verificar respostas (T-12, T-15)
- [ ] **Sessão**: reinicializar aplicação, verificar se sessão anterior é rejeitada (T-01)
- [ ] **Logs**: verificar se senhas ou PII aparecem em arquivos de log (T-11)
- [ ] **Binários**: verificar assinatura e cadeia de confiança do instalador (T-16)
