# ADR-002: Banco de Dados — PostgreSQL bundled na porta 15432

**Data:** 2026-04-02
**Status:** Aceito

## Contexto

O sistema DiarioDeBordo é offline-first: o Pilar 1 (gestão de acervo) deve funcionar integralmente sem internet. A aplicação é desktop — o usuário não deve precisar instalar ou configurar um servidor de banco de dados manualmente, pois isso criaria uma barreira de instalação inaceitável para um aplicativo pessoal.

Forças em tensão:
- Necessidade de banco de dados relacional robusto com full-text search nativo
- Instalação zero-config para o usuário final
- Segurança das credenciais do banco sem dependência de arquivos de configuração em plaintext
- Evitar conflito com instalações existentes do PostgreSQL na máquina do usuário

**Referência:** Padrões Técnicos v4, seção 3.1 — Banco de Dados e Persistência.

## Decisão

Adotamos PostgreSQL como banco de dados, distribuído bundled com o instalador da aplicação, rodando na porta 15432 (não-padrão para evitar conflito com instalações existentes do usuário). A senha é gerada na instalação e armazenada no Secure Storage do OS (DPAPI no Windows, libsecret no Linux).

## Consequências

### Positivas
- Zero configuração manual pelo usuário — banco inicia automaticamente com a aplicação
- Senha forte gerada por instalação, nunca hardcoded ou em arquivos de configuração
- Porta 15432 (não-padrão) reduz probabilidade de colisão com PostgreSQL já instalado
- Full-text search nativo do PostgreSQL (tsvector/tsquery) sem extensões externas
- ACID completo — consistência transacional para invariantes de domínio complexos
- EF Core tem provider PostgreSQL maduro (Npgsql)

### Negativas / Trade-offs
- Aumenta tamanho do instalador em ~80MB (runtime PostgreSQL por plataforma)
- Requer bundling separado do runtime PostgreSQL para Linux e Windows
- Processo PostgreSQL consome memória mesmo quando a aplicação não está em uso intenso

## Alternativas Consideradas

| Alternativa | Por que não adotada |
|---|---|
| SQLite | Full-text search inadequado para buscas complexas (sem tsvector); sem suporte multi-user local futuro |
| SQL Server Express | Somente Windows — viola requisito cross-platform; licença mais restritiva |
| MySQL/MariaDB | Full-text search menos maduro que PostgreSQL; menos suporte no ecossistema .NET |
| PostgreSQL instalado pelo usuário | Cria barreira de instalação inaceitável para aplicativo pessoal desktop |
