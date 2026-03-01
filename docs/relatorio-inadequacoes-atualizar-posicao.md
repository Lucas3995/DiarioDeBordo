# Relatório de Inadequações — Módulo Atualizar Posição (escopo novo e alterado)

## Resumo
- Total de achados: 2
- Por categoria: Arch and Struct (2)

## Achados

### 1. Request DTO na mesma unidade do Controller — ObrasController
**Categoria:** Arch and Struct
**Localização:** `backend/src/DiarioDeBordo.Api/Controllers/ObrasController.cs` (classe `AtualizarPosicaoObraRequest` no final do ficheiro)
**Evidência:** O DTO de request `AtualizarPosicaoObraRequest` está definido no mesmo ficheiro que o controller. Em projetos que separam contratos da API (DTOs de entrada/saída) da apresentação, os request DTOs costumam ficar em pastas dedicadas (ex.: Api/Models, Api/Requests) para facilitar reutilização e manutenção.
**Princípio/Referência violada:** Separação de responsabilidades na camada de apresentação; convenção de organização do projeto.
**Contexto adicional:** Outros endpoints do mesmo controller (ex.: Listar) usam tipos da Application diretamente; este é o único que define um DTO de request no controller. Mover para um ficheiro dedicado mantém consistência futura.

---

### 2. Interface de repositório em pasta de um caso de uso — IObraEscritaRepository
**Categoria:** Arch and Struct
**Localização:** `backend/src/DiarioDeBordo.Application/Obras/AtualizarPosicao/IObraEscritaRepository.cs`
**Evidência:** A interface `IObraEscritaRepository` está na pasta do caso de uso AtualizarPosicao, mas é consumida também pelo caso de uso ObterPorIdOuNome (GetObraPorIdOuNomeQueryHandler), que fica em `Application/Obras/ObterPorIdOuNome/`. Isso cria uma dependência do módulo ObterPorIdOuNome em relação à pasta AtualizarPosicao, sugerindo que o contrato é compartilhado e deveria residir em um local neutro (ex.: Application/Obras ou Application/Obras/Common).
**Princípio/Referência violada:** Organização por feature e baixo acoplamento; a interface é um contrato compartilhado entre dois casos de uso.
**Contexto adicional:** A implementação em Persistence já está correta; apenas a localização da interface na Application pode ser ajustada para refletir que é um recurso compartilhado de obras.

---
