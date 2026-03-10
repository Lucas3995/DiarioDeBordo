---
sourceArtifact: none
upstreamPlan: none
planType: sot-refatoracao
createdAt: 2026-03-10T00:00:00Z
updatedAt: 2026-03-10T00:00:00Z
---

# Fonte de Verdade para Refactoring

## 1. Visão da Arquitetura e Estrutura Atual

### Backend (.NET 9 / C# 13)
- Clean Architecture, CQRS, DDD, TDD, DevSecOps, LGPD.
- Camadas: Domain (entidades, value objects), Application (casos de uso), Persistence (DbContext, repositórios), Infrastructure (serviços externos), Api (controllers, DI, JWT), Tests.
- Direção de dependência: Api → Application → Domain; Api → Persistence → Domain; Api → Infrastructure → Domain; Tests → Application, Domain, Api.
- Camadas internas não referenciam frameworks, banco ou HTTP.

### Frontend (Angular 21)
- Clean Architecture, DDD, modularização (feature modules, core, shared).
- Camadas ativas: core (configuração, guards, interceptors), shared (componentes reutilizáveis), features (subdiretórios por feature).
- Camadas futuras: domain, application, infrastructure (criadas conforme evolução do domínio).
- Convenções: templateUrl + styleUrl, SCSS, routing com lazy loading, testes unitários (Karma/Jasmine), e2e (Playwright), CI (GitHub Actions).

## 2. Relatório de Inadequações

- Referenciar skills batedor-de-codigos e mestre-freire para análise detalhada.
- Pontos críticos a monitorar:
  - Backend: entidades anêmicas, God Services, repositórios com lógica de negócio, dependências acíclicas, SRP, DIP, CCP, REP, uso de Value Objects, testes cobrindo invariantes.
  - Frontend: modularização progressiva, convenções Angular, evitar injeção direta de HttpClient em componentes, garantir a11y, signals, typed forms, central de ações, lazy loading, consistência de nomenclatura e estrutura.
- Para análise detalhada, consultar catálogos de smells e técnicas em .github/skills/batedor-de-codigos/reference.md e .github/skills/mestre-freire/reference.md.

## 3. Prioridades e Critérios para Refatorações

- Priorizar:
  1. Violações de arquitetura (ciclos, acoplamento, God Object, dependências rígidas).
  2. Smells de domínio: entidades anêmicas, God Services, repositórios com lógica de negócio.
  3. Modularização e separação de responsabilidades (SRP, CCP, REP).
  4. Consistência de nomenclatura, abstração e estrutura.
  5. Testes cobrindo invariantes e regras de negócio.
  6. Convenções Angular e boas práticas de frontend.
- Refatorar sempre com testes verdes; não alterar regras de negócio.
- Usar técnicas de refactoring conforme mapeamento de smells (Fowler, Clean Code, DDD).

## 4. Restrições

- Não alterar regras de negócio do sistema.
- Testes e funcionalidades devem permanecer intactos.
- Refatorações guiadas por esta fonte de verdade não devem quebrar testes nem funcionalidades.
- Alterações devem respeitar Clean Architecture, DDD, CQRS, TDD, DevSecOps, LGPD.

---

Este documento orienta ciclos de refactoring e deve ser atualizado conforme novas análises ou evolução incremental.
