# DFD Nível 1 — Detalhamento por Subsistema

**Referência:** Veja `overview.md` para DFD nível 0 e escopo geral.

---

## Subsistema 1: Autenticação

```mermaid
graph TD
    U[Usuário] -->|"credenciais (byte[], não string)"| AUTH[Handler de Autenticação\nModule.Identidade]
    AUTH -->|"Argon2id.Verify(senha, hash)"| DB[(PostgreSQL\nusuario_id, password_hash)]
    DB -->|resultado da verificação| AUTH
    AUTH -->|"CryptographicOperations.ZeroMemory(senha)"| MEM[Memória limpa]
    AUTH -->|"IUsuarioAutenticadoProvider.UsuarioIdAtual = userId"| SESSION[Sessão em memória\nDI container]
    AUTH -->|"Result<bool>"| U
```

**Trust boundary:** Processo da aplicação → processo do PostgreSQL (mesmo host, porta 15432)
**Entradas:** Credenciais do usuário (byte[])
**Saídas:** Token de sessão em memória (Guid do usuário)
**Dados sensíveis em trânsito:** Senha em memória — nunca serializada, zerada após uso com `CryptographicOperations.ZeroMemory()`

**Proteções:**
- Argon2id para hashing de senhas (resistente a GPU e ASIC)
- `CryptographicOperations.FixedTimeEquals()` para comparação de hashes (previne timing attacks)
- Senha manipulada como `byte[]`, nunca `string` (evita fixação no heap do GC)
- `CryptographicOperations.ZeroMemory()` após uso

---

## Subsistema 2: Banco de Dados

```mermaid
graph TD
    APP[Módulos da Aplicação\nModule.Acervo, etc.] -->|"EF Core - queries parametrizadas"| EFCORE[EF Core\nDbContext]
    EFCORE -->|"SQL com parameters - sem concatenação"| DB[(PostgreSQL\nlocalhost:15432)]
    DB -->|"Resultados"| EFCORE
    EFCORE -->|"Entidades de domínio"| APP
    SS[Secure Storage\nDPAPI/libsecret] -->|"ConnectionString com senha"| EFCORE
```

**Trust boundary:** Processo da aplicação → PostgreSQL (localhost)
**Invariante de segurança:** Todo acesso inclui `WHERE usuario_id = @usuarioId` — sem exceções
**Proteções:**
- EF Core usa queries parametrizadas — BannedSymbols.txt proíbe concatenação de SQL
- ConnectionString nunca em arquivos de configuração — somente no Secure Storage do OS (DPAPI no Windows, libsecret no Linux)
- Porta 15432 (não-padrão) + credenciais fortes geradas na instalação
- `PaginatedList<T>` obrigatório em todas as listagens — previne DoS por consultas sem paginação

---

## Subsistema 3: Adaptadores de Rede

```mermaid
graph TD
    AGG[Module.Agregacao] -->|"FonteSubscricao (url validada)"| ADAPT[IAdaptadorPlataforma\nModule.IntegracaoExterna]
    ADAPT -->|"Validar URL: protocolo whitelist + anti-SSRF"| VAL[Validação de URL]
    VAL -->|"URL segura"| HTTP[HttpClient\ntimeout 10s, max 5MB]
    HTTP -->|"HTTPS request"| EXT[Plataforma Externa\nRSS/YouTube]
    EXT -->|"Payload de resposta"| HTTP
    HTTP -->|"payload <= 5MB"| PARSE[Parser\nDtdProcessing.Prohibit para XML]
    PARSE -->|"List<ItemFeedDto>"| AGG
    HTTP -->|"timeout / erro"| ERR[Result.Failure\nsem crash]
```

**Trust boundary:** Processo da aplicação → Internet (untrusted)
**Proteções obrigatórias (todas invioláveis):**
- `DtdProcessing.Prohibit` em todo XML/RSS — previne XXE (XML External Entity attacks)
- Limite 5MB por payload — previne DoS por resposta gigante
- Timeout 10s — previne DoS por resposta lenta
- Validação pós-resolução DNS: rejeitar IPs privados (10.x, 192.168.x, 127.x, 172.16–31.x, ::1, 169.254.x) — previne SSRF
- Whitelist de protocolos: `https`, `http`, `file` apenas — rejeitar tudo mais
- Circuit breaker após N falhas consecutivas — previne hammering de fontes indisponíveis

**Anti-SSRF — validação após DNS resolution (obrigatório):**
```
1. Resolver hostname → endereço IP
2. Verificar IP contra lista de ranges privados:
   - 10.0.0.0/8
   - 172.16.0.0/12
   - 192.168.0.0/16
   - 127.0.0.0/8
   - 169.254.0.0/16 (link-local)
   - ::1/128 (IPv6 loopback)
3. Se IP privado → rejeitar com Result.Failure("URL aponta para rede privada")
4. NUNCA usar apenas validação de hostname (bypass por DNS rebinding)
```

---

## Subsistema 4: Reprodutor Externo

```mermaid
graph TD
    REPRO[Module.Reproducao] -->|"FonteOrdenada.Valor (url ou caminho)"| VAL[Validação de protocolo]
    VAL -->|"Whitelist: https, http, file"| PS[Process.Start\napp padrão do OS]
    VAL -->|"Protocolo proibido (javascript:, data:, etc.)"| ERR[Rejeição - log de aviso]
```

**Trust boundary:** Processo da aplicação → OS (Process.Start)
**Proteção:** Whitelist estrita de protocolos antes de qualquer `Process.Start`

**Lógica de validação:**
```
Protocolo permitido: ["https", "http", "file"]
Se protocolo não está na whitelist → rejeitar, logar aviso
Nunca: javascript:, data:, vbscript:, ftp:, smb:
```

---

## Subsistema 5: Importação de Arquivos

```mermaid
graph TD
    U[Usuário] -->|"Arquivo de importação"| IMP[Module.Portabilidade\nImportador]
    IMP -->|"Verificar tamanho (limite configurável)"| SIZE[Verificação de tamanho]
    SIZE -->|"Arquivo dentro do limite"| CHECK[Verificação de checksum]
    CHECK -->|"Checksum válido"| PARSE[Parse do formato\nlimite de profundidade]
    PARSE -->|"Dados validados"| DB[(PostgreSQL)]
    SIZE -->|"Arquivo acima do limite"| ERR1[Rejeição com mensagem]
    CHECK -->|"Checksum inválido"| ERR2[Rejeição com mensagem]
```

**Trust boundary:** Sistema de arquivos (potencialmente manipulado) → Processo da aplicação
**Proteções:**
- Verificação de tamanho antes do parse (previne DoS por arquivo gigante)
- Checksum de integridade antes de processar (previne arquivos corrompidos/adulterados)
- Limite de profundidade de nesting no JSON (previne JSON bomb)
- Path traversal prevention: canonicalizar caminhos antes de abrir arquivos

---

## Referências

- `overview.md` — DFD nível 0 e escopo do threat model
- `stride-table.md` — Ameaças mapeadas por subsistema
- Padrões Técnicos v4, seção 4 (segurança)
- ADR-005: Abordagem de Segurança
