# ADR-005: Abordagem de Segurança — Argon2id, DPAPI/libsecret, BannedSymbols

**Data:** 2026-04-02
**Status:** Aceito

## Contexto

O sistema lida com dados pessoais do usuário (conteúdo, anotações, progresso) e requer autenticação local sem OAuth. As credenciais do banco PostgreSQL precisam ser protegidas sem depender de arquivos de configuração em plaintext. A natureza desktop da aplicação cria vetores de ataque específicos (acesso físico à máquina, leitura de memória do processo).

Forças em tensão:
- Senhas de usuário nunca em plaintext em banco ou memória por tempo prolongado
- Credenciais do banco PostgreSQL protegidas pelo OS, não por arquivos de configuração
- Prevenção de regressões de segurança via análise estática — não depender apenas de revisão manual
- Minimizar janela de exposição de dados sensíveis em memória do processo

**Referência:** Padrões Técnicos v4, seção 4 — Segurança.

## Decisão

Adotamos as seguintes medidas de segurança em profundidade:

1. **Argon2id** para hash de senhas de usuário — resistente a ataques de força bruta com GPU e ASIC
2. **DPAPI** (Windows) e **libsecret** (Linux) para armazenar credenciais do banco PostgreSQL — nunca em arquivos de configuração ou código-fonte
3. **BannedSymbols.txt** no analisador Roslyn para impedir uso de APIs perigosas: `string` para senhas, SQL concatenado, DTD processing, e outros padrões inseguros identificados na seção 4 dos Padrões Técnicos v4
4. **`CryptographicOperations.ZeroMemory()`** após uso de dados sensíveis em memória para reduzir janela de exposição
5. **`usuarioId` obrigatório** em toda query ao banco — multi-tenant local por design, sem vazamento de dados entre usuários da mesma instância

## Consequências

### Positivas
- Senhas de usuário nunca armazenadas em plaintext em nenhuma camada
- Credenciais do banco protegidas pelo OS — chave de encriptação não precisa ficar em arquivo
- Análise estática via BannedSymbols impede introdução de padrões inseguros sem revisão de código dedicada
- ZeroMemory reduz janela de exposição de segredos em dumps de memória
- `usuarioId` obrigatório garante isolamento de dados entre usuários na mesma instância local

### Negativas / Trade-offs
- Argon2id é mais lento que bcrypt e SHA por design — essa lentidão é uma feature de segurança, não um bug
- libsecret requer D-Bus no Linux — disponível em todos os desktops Linux modernos (GNOME, KDE, etc.)
- BannedSymbols.txt requer manutenção ativa à medida que novas APIs inseguras são identificadas
- ZeroMemory exige disciplina no código — dados sensíveis precisam ser rastreados até o ponto de descarte

## Alternativas Consideradas

| Alternativa | Por que não adotada |
|---|---|
| bcrypt | Menos resistente a ataques com GPU e ASIC que Argon2id — Argon2id é o estado da arte atual para hash de senhas |
| Arquivos de configuração encriptados | A chave de encriptação precisa ficar em algum lugar — problema circular que DPAPI/libsecret resolve nativamente |
| Sem BannedSymbols | Depende exclusivamente de revisão de código manual para prevenir APIs inseguras — não escalável |
| SHA-256 para senhas | Hash criptográfico rápido é inadequado para senhas — vulnerável a ataques de força bruta com hardware moderno |
