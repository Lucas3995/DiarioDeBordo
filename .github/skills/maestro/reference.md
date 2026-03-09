# Referência — Engenharia de Requisitos (para relatório de alterações)

Resumo de conceitos úteis para redigir o relatório de alterações de forma alinhada à Engenharia de Requisitos. Fonte: [ENGENHARIA_DE_REQUISITOS.md](../../../ENGENHARIA_DE_REQUISITOS.md) (norma IEEE/ISO/IEC 29148).

---

## Conceitos centrais

| Conceito | Definição |
|----------|-----------|
| **Requisito funcional** | O que o sistema deve fazer; comportamentos, funções e capacidades esperadas. |
| **Requisito não funcional** | Como o sistema deve se comportar; desempenho, segurança, usabilidade, disponibilidade. |
| **Requisito de negócio** | Objetivos estratégicos que motivam a existência da funcionalidade; valor entregue. |
| **Requisito de sistema/software** | Especificação em nível de sistema ou software; pode derivar de requisitos de negócio. |

Ao descrever a demanda e as alterações, usar estes termos quando ajudar a evitar ambiguidade (ex.: "Requisito funcional: o utilizador deve poder X"; "Requisito não funcional: resposta em menos de 2s").

---

## Atributos de requisito (qualidade documental)

Requisitos bem escritos são: **completo**, **consistente**, **não ambíguo**, **verificável**, **rastreável**, **relevante**. No relatório, cada alteração pode ser ligada à parte da demanda que atende (rastreabilidade demanda → alteração).

---

## Atividades relevantes para o Maestro

- **Revisão das demandas**: Analisar a demanda do utilizador; identificar o que está claro e o que é ambíguo.
- **Esclarecimento das demandas**: Refinar e esclarecer; remover ambiguidades na medida do possível; assinalar premissas quando não for possível.
- **Distribuição das tarefas**: Atribuir alterações a componentes do sistema (onde cada mudança deve ocorrer) — isto é o núcleo do relatório do Maestro.

Não é papel do Maestro: elicitação com stakeholders, validação formal, baseline ou gestão de mudanças em projeto.

---

## Boas práticas para o texto do relatório

- **Não ambíguo**: Uma única interpretação possível; evitar "melhorar", "tornar mais rápido" sem critério objectivo.
- **Verificável**: Cada alteração deve permitir verificar se foi feita (ex.: "Criar endpoint POST /api/foo" em vez de "adicionar suporte a foo").
- **Rastreável**: Ligar cada item de alteração à parte da demanda que atende (campo "Requisito atendido" no template).

---

## Especificação em dois níveis

| Nível | Escopo | Conteúdo | Exemplo |
|-------|--------|----------|---------|
| **Requisitos de usuário** | O que o utilizador precisa; linguagem de negócio. | User stories, critérios de aceite, cenários de uso, personas. | "O gestor deve poder aprovar medições pendentes." |
| **Requisitos de sistema** | O que o sistema deve fazer para atender o requisito de usuário; linguagem técnica. | Endpoints, entidades, validações, estados, integrações, permissões. | "POST /api/medicoes/{id}/aprovar — altera status para Aprovada; valida permissão GESTOR." |

**Regra para o Maestro:** o relatório de alterações deve conter ambos os níveis — o requisito de usuário (contexto) e o(s) requisito(s) de sistema derivado(s) (ações concretas). Referência: ISO/IEC/IEEE 29148.

---

## Técnicas de elicitação

| Técnica | Quando usar | O que extrair |
|---------|-------------|---------------|
| **Entrevista / conversa com stakeholder** | Demanda ambígua ou incompleta; múltiplas interpretações. | Intenção real, prioridade, critérios de aceite, restrições. |
| **Análise de documentos** | Há documentação existente (specs, manuais, contratos). | Requisitos implícitos, regras de negócio não mencionadas na demanda. |
| **Prototipação / wireframe** | Demanda envolve UI/UX e o fluxo não está claro. | Fluxo de telas, campos, ações, feedbacks esperados. |
| **Cenários / user stories** | Detalhar comportamento esperado por persona. | Fluxo feliz + exceções; critério de aceite por cenário. |
| **Brainstorming** | Explorar alternativas e ideias. | Opções de solução, riscos, dependências. |

**Uso pelo Maestro:** ao analisar a demanda, verificar se as informações são suficientes; se não, sugerir ao operador quais técnicas de elicitação aplicar antes de produzir o relatório.

---

## Requisitos não funcionais (NFRs) verificáveis

| Atributo | Como tornar verificável | Exemplo |
|----------|------------------------|---------|
| **Desempenho** | Tempo de resposta, throughput, carga. | "Lista de obras carrega em < 2s para 1000 registros." |
| **Segurança** | Autenticação, autorização, criptografia, auditoria. | "Endpoint protegido por JWT; apenas role GESTOR acessa." |
| **Usabilidade** | Passos para completar tarefa, taxa de erro, a11y. | "Aprovação em no máximo 3 cliques; WCAG AA." |
| **Disponibilidade** | Uptime, recovery, failover. | "99.5% uptime mensal; recovery < 4h." |
| **Manutenibilidade** | Cobertura de testes, complexidade ciclomática. | "Cobertura > 80%; CC < 10 por método." |

**Regra:** NFRs sem valor mensurável não são verificáveis e devem ser refinados antes de entrar no relatório.

---

## Requisitos de proteção de dados (LGPD)

Quando o tradutor sinalizar dados pessoais, o Maestro deve especificar requisitos concretos:

| Aspecto LGPD | Requisito a gerar | Exemplo de item no relatório |
|--------------|-------------------|------------------------------|
| **Finalidade e consentimento** | Registrar base legal e consentimento para cada tipo de dado coletado. | "Tela de cadastro deve exibir termo de consentimento; armazenar aceite com timestamp." |
| **Minimização de dados** | Coletar apenas dados necessários para a finalidade. | "Remover campo 'religiao' do cadastro — sem finalidade identificada." |
| **Direitos do titular** | Fornecer acesso, correção, exclusão, portabilidade. | "Endpoint GET /api/meus-dados retorna todos os dados pessoais do usuário logado." |
| **Retenção e descarte** | Definir período de retenção e processo de descarte. | "Logs com dados pessoais retidos por 6 meses; rotina de expurgo automatizada." |
| **Anonimização / pseudonimização** | Quando dados são usados para analytics ou relatórios. | "Relatório de produtividade usa dados anonimizados (sem CPF, nome)." |
| **Segurança do tratamento** | Criptografia, controle de acesso, auditoria. | "Dados pessoais criptografados em repouso (AES-256); log de acesso auditável." |

**Fluxo:** Tradutor identifica dados pessoais → Maestro gera requisitos LGPD concretos → Quadro cria testes que verificam conformidade.

---

## Rastreabilidade bidirecional

| Direção | De → Para | Uso |
|---------|-----------|-----|
| **Forward** | Demanda → Requisito de usuário → Requisito de sistema → Alteração no código | Garantir que toda demanda tem implementação correspondente. |
| **Backward** | Alteração no código → Requisito de sistema → Requisito de usuário → Demanda | Garantir que toda alteração tem justificativa na demanda. |

**No relatório do Maestro:** cada item de alteração deve referenciar o requisito de usuário e a demanda que o originou. Formato: campo "Origem" no template do relatório.

---

## Linguagem Ubíqua nos requisitos

| Regra | Aplicação no relatório |
|-------|------------------------|
| **Usar termos do domínio** | "Medição", "Obra", "Contrato" — não "registro", "item", "objeto". |
| **Consistência com o glossário** | Se o glossário do projeto define "Etapa" como subdivisão de Obra, usar "Etapa" no relatório. |
| **Evitar sinônimos** | Não alternar entre "Medição" e "Aferição" para o mesmo conceito. |
| **Alinhar com código existente** | Se o código já usa `Medicao`, usar "Medição" no relatório (não traduzir). |

Ver [engenharia-de-software reference.md](../engenharia-de-software/reference.md) §Glossário para termos unificados.
