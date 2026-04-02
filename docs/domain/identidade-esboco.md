# BC Identidade — Esboço

**Classificação:** Genérico
**Projeto .NET:** `DiarioDeBordo.Module.Identidade`
**Status de modelagem:** Esboço — modelagem tática completa será feita na Phase de implementação do BC Identidade

## Responsabilidade

Autenticação local multi-usuário com roles (consumidor/admin), e fornecer a identidade do usuário atual para todos os módulos via `IUsuarioAutenticadoProvider`. É o único BC que conhece credenciais — todos os demais módulos recebem apenas o `UsuarioId` e os roles via interface.

## O que este BC NÃO faz

- Sem OAuth, SSO ou autenticação remota de qualquer tipo
- Sem perfis públicos de usuário
- Sem autenticação de dois fatores (na fase inicial)
- Sem gerenciamento de sessão distribuída (aplicação desktop local)
- Não decide autorização de recursos — apenas provê identidade e roles

## Interfaces publicadas (definidas em Core, implementadas aqui)

```csharp
// Definida em DiarioDeBordo.Core — consumida por TODOS os módulos que precisam do usuário atual:
public interface IUsuarioAutenticadoProvider
{
    Guid? UsuarioIdAtual { get; }
    IReadOnlySet<Role> RolesAtuais { get; }
    bool EstaAutenticado { get; }
}

public enum Role
{
    Consumidor,
    Admin
}
```

## Interfaces consumidas (definidas em Core, implementadas em outro BC)

Nenhuma — este BC não depende de interfaces de outros BCs.

## Eventos de domínio relevantes

| Evento | Publicado por | Consumido por | Quando |
|---|---|---|---|
| `UsuarioAutenticadoNotification(UsuarioId, Roles)` | Module.Identidade | Todos os módulos com estado por sessão | Quando login bem-sucedido |
| `UsuarioDesautenticadoNotification(UsuarioId)` | Module.Identidade | Todos os módulos com estado por sessão | Quando logout ou sessão expirada |

## Invariante de Segurança Crítica — Área Admin Invisível

> **Esta invariante é verificada em TODAS as camadas — não apenas na UI.**

A área de administração **não existe** para usuários sem o role `Admin`:

1. **Camada de UI:** Nenhum link, menu, botão ou indicação visual da área admin é renderizado para usuários com role `Consumidor`. A área admin é literalmente ausente do DOM/visual tree.

2. **Camada de serviço (Application):** Qualquer command ou query de administração que chegue sem o role `Admin` é recusado. O handler retorna um resultado genérico — sem revelar que a operação ou rota existe.

3. **Camada de apresentação (ViewModel):** Rotas e commands de admin não são registrados no sistema de navegação para sessões não-admin.

**Comportamento em acesso não autorizado:** retorna comportamento genérico — sem indicação de que a área admin existe. O usuário é redirecionado à home com uma mensagem de erro genérica (ex: "Página não encontrada" ou "Recurso indisponível") — nunca "Acesso negado", pois isso revelaria a existência da área.

Esta regra protege contra enumeração de rotas privilegiadas e é um requisito de segurança, não apenas de UX.

## Segurança de credenciais

- **Hash de senhas:** Argon2id (conforme ADR-005)
  - Parâmetros mínimos: memória 64MB, iterações 3, paralelismo 1
  - Salt único por senha, gerado com `RandomNumberGenerator`
- **Armazenamento de sessão:** token local não persistido além da sessão da aplicação

## O que é adiado para a fase de implementação

- Modelo concreto da entidade `Usuario` (atributos, histórico de logins)
- Grupos de roles e permissões granulares por recurso
- Tela de administração de usuários (criação, desativação, troca de role)
- Fluxo de criação do primeiro usuário admin (bootstrapping)
- Recuperação de senha (mecanismo local — sem email)
- Política de bloqueio de conta após tentativas falhas
