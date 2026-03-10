# Separar AtualizarPosicaoObraCommand em dois Commands

## Metadados

| Campo | Valor |
|-------|-------|
| **ID** | REFACT-BACK-001 |
| **Tipo** | Refatoração (pureza CQRS) |
| **Prioridade** | Média (P1) |
| **sourceArtifact** | `.github/reference/fonte-de-verdade-refactoring.md` — achado #14 |
| **Diretriz violada** | CQRS — um command = uma intenção; SRP |

---

## 1. Contexto

`AtualizarPosicaoObraCommand` tem 8 parâmetros: 5 para update de posição + 3 opcionais para criação de obra nova. O handler já separa os dois caminhos (`if obra exists → update`, `else if criar → create`), mas o record único viola a pureza CQRS (um command = uma intenção).

**Estrutura atual:**
```csharp
public sealed record AtualizarPosicaoObraCommand(
    Guid? IdObra,
    string? NomeObra,
    int NovaPosicao,
    DateTime? DataUltimaAtualizacao,
    bool CriarSeNaoExistir,
    string? NomeParaCriar = null,
    TipoObra? TipoParaCriar = null,
    int? OrdemPreferenciaParaCriar = null) : IRequest<AtualizarPosicaoObraResponse>;
```

---

## 2. User Story

Como desenvolvedor, quero que cada command represente exatamente uma intenção de negócio para facilitar validação, testes e manutenção.

---

## 3. Critérios de aceitação

1. `AtualizarPosicaoObraCommand` contém somente parâmetros de update (IdObra, NomeObra, NovaPosicao, DataUltimaAtualizacao).
2. Novo command `CriarObraComPosicaoCommand` contém parâmetros de criação (Nome, Tipo, OrdemPreferencia, PosicaoInicial).
3. Cada command tem seu próprio handler e validator.
4. Controller decide qual command enviar com base no payload (ou frontend passa separado).
5. Response mantém compatibilidade (`AtualizarPosicaoObraResponse` com `Criada: bool`).
6. Todos os testes existentes (103 backend) passam.
7. Frontend ajusta requests se necessário.

---

## 4. Alterações necessárias

### Backend
1. **Criar** `CriarObraComPosicaoCommand` record com: Nome, Tipo, OrdemPreferencia, PosicaoInicial.
2. **Criar** `CriarObraComPosicaoCommandHandler` com a lógica de criação extraída do handler atual.
3. **Criar** `CriarObraComPosicaoCommandValidator` com validações específicas de criação.
4. **Simplificar** `AtualizarPosicaoObraCommand` removendo parâmetros de criação e `CriarSeNaoExistir`.
5. **Simplificar** handler existente removendo branch de criação.
6. **Controller**: novo endpoint ou decisão no controller sobre qual command enviar.

### Frontend
7. **Ajustar** `AtualizarPosicaoComponent` e `PromptObraNovaComponent` para chamar endpoints separados conforme a intenção.
8. **Ajustar** port/service se a interface mudar.

---

## 5. Dependências e riscos

- Frontend `AtualizarPosicaoComponent` e `PromptObraNovaComponent` precisarão ajustar os requests.
- Validar se o fluxo do frontend (prompt → criação) deve chamar endpoint diferente.

---

## 6. Requisitos técnicos/metodológicos aplicáveis

- CQRS: um command = uma intenção de negócio clara.
- SRP: cada handler resolve exatamente um caso de uso.
- TDD: testes existentes adaptados; novos testes para o command de criação.
- DDD: commands expressam linguagem ubíqua do domínio.
