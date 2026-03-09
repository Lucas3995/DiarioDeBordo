# Referência — Tradutor

Material de apoio para os passos 8, 9 e 10 do workflow do tradutor. Consumo por agente IA.

---

## Linguagem Ubíqua

### Princípios

| Princípio | Aplicação |
|-----------|-----------|
| **Um termo por conceito** | Não alternar entre sinônimos ("Medição" e "Aferição" para o mesmo conceito). |
| **Termos do domínio, não técnicos** | "Obra", "Contrato", "Medição" — não "registro", "item", "dado". |
| **Alinhamento código ↔ documentação** | Se o código usa `Medicao`, a documentação diz "Medição". |
| **Glossário vivo** | Atualizado a cada demanda; termos novos são adicionados, obsoletos removidos. |

### Checklist para o tradutor

- [ ] Listar todos os termos-chave que aparecem na demanda.
- [ ] Verificar se cada termo já existe no glossário do projeto.
- [ ] Para termos novos: definir com precisão, validar com o operador.
- [ ] Para termos ambíguos: resolver com o operador antes de prosseguir.
- [ ] Incluir seção "Glossário de domínio" no entregável (termo + definição + nota de uso).

### Template de glossário

| Termo | Definição | Uso no sistema | Observações |
|-------|-----------|----------------|-------------|
| _Ex.: Medição_ | _Aferição do progresso físico de uma etapa da obra._ | _Entidade, grid, relatório_ | _Não confundir com "Apontamento" (registro de horas)._ |

---

## Proteção de Dados (LGPD)

### Quando sinalizar

Sinalizar sempre que a demanda envolver **qualquer** dado que identifique ou possa identificar uma pessoa natural:
- Dados diretos: nome, CPF, RG, email, telefone, endereço, foto.
- Dados sensíveis: saúde, biometria, orientação, religião, opinião política, filiação sindical.
- Dados indiretos: IP, geolocalização, cookies, identificadores de dispositivo.

### Checklist de proteção de dados

| # | Aspecto | Pergunta-chave | Ação se "sim" |
|---|---------|----------------|---------------|
| 1 | **Finalidade** | Para que esses dados serão usados? | Documentar base legal (consentimento, legítimo interesse, obrigação legal, etc.). |
| 2 | **Minimização** | Todos os campos são necessários para a finalidade? | Remover campos desnecessários; justificar cada campo com finalidade. |
| 3 | **Consentimento** | O titular precisa consentir? | Prever tela de consentimento com checkbox, texto e timestamp do aceite. |
| 4 | **Acesso do titular** | O titular pode ver/exportar seus dados? | Prever endpoint/tela "Meus dados" com download. |
| 5 | **Correção** | O titular pode corrigir seus dados? | Prever edição de perfil/dados pessoais. |
| 6 | **Exclusão** | O titular pode pedir exclusão? | Prever soft-delete ou anonimização com fluxo de solicitação. |
| 7 | **Retenção** | Por quanto tempo os dados ficam armazenados? | Definir período; prever rotina de expurgo automatizada. |
| 8 | **Anonimização** | Dados são usados em relatórios/analytics? | Anonimizar antes de alimentar dashboards. |
| 9 | **Segurança** | Dados em trânsito e em repouso protegidos? | HTTPS obrigatório; criptografia em repouso (AES-256); controle de acesso. |
| 10 | **Auditoria** | Quem acessou/alterou dados pessoais? | Log de acesso auditável com timestamp, ator e ação. |

### Template de sinalização no entregável

```markdown
## Dados Pessoais Identificados

| Dado | Finalidade | Base Legal | Retenção | Mitigação |
|------|-----------|------------|----------|-----------|
| CPF | Identificação do titular do contrato | Obrigação legal | Vigência do contrato + 5 anos | Criptografia em repouso; acesso restrito a perfil GESTOR |
```

---

## Mitigação de Riscos ao Usar IA

### Princípio central

> "Ao usar IA para X, aplique Y como mitigação."

A IA é ferramenta válida; o foco é **mitigar riscos**, não evitar o uso.

### Catálogo de estratégias por área de risco

| Área de risco | Exemplo | Estratégia de mitigação |
|--------------|---------|------------------------|
| **Geração de texto/conteúdo** | IA gera descrições, resumos, e-mails para o usuário. | Revisão humana obrigatória antes de envio; flag "gerado por IA" visível. |
| **Geração de dados** | IA sugere valores, preenche campos automáticos. | Validação contra fonte autoritativa; highlight visual de dados sugeridos vs confirmados. |
| **Classificação de pessoas** | IA classifica perfis, scores, prioridades. | Testes de viés (fairness); explicabilidade (por que essa classificação?); revisão periódica. |
| **Recomendação** | IA sugere ações, produtos, próximos passos. | Transparência ("sugerido por IA"); opção de ignorar; métricas de qualidade da recomendação. |
| **Decisão automatizada** | IA aprova/rejeita sem intervenção humana. | Sempre ter caminho de revisão humana; log de decisão com justificativa; alerta para edge cases. |
| **Geração de código** | IA gera trechos de código, queries, scripts. | Code review obrigatório; testes automatizados; não confiar em segurança do código gerado sem revisão. |
| **Análise de requisitos** | IA interpreta demandas, gera specs. | Validação com stakeholder; não usar como fonte única; contrastar com documentação existente. |
| **Testes** | IA gera casos de teste. | Revisão de cobertura por humano; não assumir que IA cobriu edge cases; complementar com técnicas formais. |

### Template de sinalização no entregável

```markdown
## Mitigações IA

| Risco identificado | Área | Estratégia de mitigação |
|-------------------|------|------------------------|
| IA gera resumo de medição para relatório | Geração de texto | Revisão humana obrigatória antes de inclusão no relatório oficial |
```

### Checklist rápido

- [ ] A demanda envolve uso de IA em alguma funcionalidade?
- [ ] Para cada uso identificado: mapeado risco + estratégia de mitigação?
- [ ] Mitigações registradas no entregável para que maestro e quadro as tratem?
- [ ] Transparência ao usuário final prevista (flag "gerado por IA" quando aplicável)?
