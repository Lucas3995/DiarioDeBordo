# ADR-003: Arquitetura — Monolito Modular com Bounded Contexts

**Data:** 2026-04-02
**Status:** Aceito

## Contexto

O sistema possui 8 bounded contexts (Acervo, Agregação, Reprodução, Integração Externa, Busca, Portabilidade, Identidade, Preferências). A arquitetura precisa garantir separação de responsabilidades e manutenibilidade sem impor overhead desnecessário para uma aplicação desktop single-user.

Forças em tensão:
- Separação por contexto para evitar acoplamento inadvertido entre módulos
- Aplicação desktop single-process — comunicação via rede seria overhead gratuito
- Desenvolvimento incremental por fases — módulos precisam ser desenvolvidos de forma independente
- Testabilidade: cada módulo deve ser testável em isolamento
- Futuro: não impedir extração de módulos caso o sistema evolua

**Referência:** Padrões Técnicos v4, seções 1.2 e 6 — Arquitetura e Estrutura de Solução.

## Decisão

Adotamos arquitetura de monolito modular onde cada bounded context é um projeto/namespace separado (`DiarioDeBordo.Module.Acervo`, `DiarioDeBordo.Module.Agregacao`, etc.) com interfaces explícitas definidas em `DiarioDeBordo.Core`. Comunicação entre módulos exclusivamente via MediatR (Commands, Notifications) ou interfaces em Core — sem referência direta entre módulos.

## Consequências

### Positivas
- Deploy único — sem orquestração de serviços para uma aplicação desktop
- Comunicação in-process via MediatR — sem overhead de rede, serialização ou service discovery
- Separação explícita de responsabilidades — cada módulo tem seu próprio namespace e projeto
- Facilidade de teste por módulo — cada contexto pode ser testado com mocks das interfaces em Core
- Evolução incremental — módulos novos são adicionados sem modificar os existentes
- Anti-corruption layers naturais via interfaces em Core

### Negativas / Trade-offs
- Acoplamento de deploy — todos os módulos sobem juntos no mesmo processo
- Migração para microserviços no futuro exigiria extração e conversão de módulos para serviços com API
- MediatR pode esconder dependências implícitas se o pipeline não for bem documentado

## Alternativas Consideradas

| Alternativa | Por que não adotada |
|---|---|
| Microserviços | Overhead de rede, Docker, service mesh desnecessário para aplicação desktop single-user |
| Monolito sem modularização | Dificulta evolução por fase; acoplamento inadvertido entre contextos torna refatorações caras |
| Plugins via MEF/Assembly | Complexidade desnecessária para contextos sempre presentes na aplicação |
