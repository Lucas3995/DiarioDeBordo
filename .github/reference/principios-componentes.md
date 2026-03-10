# Princípios de Componentes e Padrões Avançados de Arquitetura Limpa

Referência centralizada para as skills `batedor-de-codigos`, `mestre-freire` e `mestre-freire-angular`. Sintetiza conteúdo de Clean Architecture (Uncle Bob, caps. 12–14, 22–27) sobre princípios de componentes, Humble Object, limites parciais e componente Main.

---

## 1. O que é um componente

- Menor parte do sistema que pode ser implantada e versionada de forma independente.
- Em .NET: um `.csproj` / DLL. Em Angular: um módulo lazy-loadable ou uma library.
- Componentes bem feitos podem ser desenvolvidos e implantados independentemente uns dos outros.

---

## 2. Princípios de coesão de componentes

### 2.1 REP – Equivalência de Reúso/Publicação

- Componentes devem agrupar código que é **reutilizado e versionado junto**.
- Módulos e classes dentro de um componente compartilham o mesmo controle de publicação (versão, data de lançamento, changelog).
- Agrupamento deve fazer sentido tanto para a equipe quanto para os consumidores.

### 2.2 CCP – Fechamento Comum

- Reunir no mesmo componente as classes que **mudam pelas mesmas razões e nos mesmos momentos**.
- É o SRP aplicado em nível de componentes.
- Quando a aplicação precisa mudar, é preferível que todas as mudanças ocorram em um mesmo componente, evitando revalidação e reimplantação dos demais.
- Separar o que muda em tempos diferentes ou por razões diferentes.

### 2.3 CRP – Reúso Comum

- Não forçar consumidores a dependerem de coisas que não usam.
- É o ISP aplicado a componentes.
- Se componente A depende de B mas só usa parte de B, isso indica que B precisa ser dividido ou A/B precisam ser reorganizados conforme REP e CCP.

### 2.4 Dialética REP × CCP × CRP

A relação entre os três princípios é **dialética**:

- **REP + CCP** (tese): tendem a **aumentar** o tamanho dos componentes (agrupar para reutilização e manutenção).
- **CRP** (antítese): tende a **diminuir** o tamanho (dividir para evitar publicações desnecessárias).
- Boa arquitetura = trabalhar esse tensionamento conforme o contexto do projeto.

| Foco | Efeito | Quando usar |
|------|--------|-------------|
| REP + CRP | Difícil reutilizar; muitas publicações em mudanças simples. | Projetos estáveis, em sustentação. |
| REP + CCP | Muitas publicações desnecessárias; vários controles de versão. | Projetos maduros e ativos; mudanças frequentes. |
| CCP + CRP | Mudanças tendem a ser grandes (muitas linhas no mesmo componente). | Início de projeto; prioridade em manutenção. |

---

## 3. Princípios de acoplamento de componentes

### 3.1 DA – Dependências Acíclicas

- **Não permitir ciclos de dependência** entre componentes.
- Se componente A depende de B, B está proibido de depender de A ou de qualquer componente que dependa de A.
- Quebrar ciclos com **DIP**: introduzir interface no componente mais interno, implementar no externo.
- Design de componentes é emergente: não projetar a estrutura a priori, mas seguir os princípios conforme as demandas obrigam a criar classes e módulos.

### 3.2 DE (SDP) – Dependências Estáveis

- Componentes fáceis de alterar **não devem depender** de componentes difíceis de alterar.
- Estabilidade ≠ frequência de alteração. É a **dificuldade** de mudar, determinada pela quantidade de dependentes.
- Componentes abstratos (só interfaces) são permitidos e ajudam a quebrar dependências usando DIP.

### 3.3 AE (SAP) – Abstrações Estáveis

- Um componente deve ser **tão abstrato quanto ele for estável**.
- Componente estável → deve consistir majoritariamente de interfaces e classes abstratas, para que sua estabilidade não impeça extensão (correlação com OCP).
- Componente instável → pode ser concreto, já que a instabilidade permite que seja facilmente alterado.
- Métricas de abstração/instabilidade permitem identificar:
  - Classes/módulos que podem ser removidos sem impacto.
  - Classes/módulos que devem ter partes movidas (ex.: aplicar OCP).
  - Se alterações recentes pioraram a arquitetura.

---

## 4. Camadas da Arquitetura Limpa (Cap. 22)

Ordem das camadas (exterior → interior):

1. **Frameworks e Drivers**: Web, UI, DB, dispositivos, interfaces externas. Contém o mínimo possível; apenas ligação com a próxima camada.
2. **Adaptadores de Interface**: Apresentadores, controladores, gateways. Normalização/sanitização dos dados entre camadas. Símbolos de medida, formatação, SQL, DTOs.
3. **Regras de negócio da aplicação** (Casos de Uso): Processos específicos da aplicação. Usam conteúdo da camada de Entidades para definir fluxos. Mudanças aqui não devem afetar Entidades.
4. **Regras de negócio da empresa** (Entidades): Conceitos intrínsecos à empresa. Só mudam se o modo da empresa funcionar mudar. Podem ser usadas por várias aplicações.

**Regra de dependência:** Um driver pode depender de tudo; uma Entidade só pode depender do que está na camada de Entidades.

**Sobre as camadas:** é um esquema proposto e aberto, não uma regra rígida — uma orientação e referência para construir a estrutura que atende a realidade do projeto.

---

## 5. Padrão Humble Object (Cap. 23)

**Quando usar:** quando uma classe é difícil ou "impossível de testar".

**Como funciona:** separar a classe em duas:
- A parte **testável** (apresentador, repositório) com toda a lógica verificável.
- A parte **humble** (visualizador, DbSet, listener) com estritamente o necessário não-testável.

### Exemplos

**Apresentadores e Visualizadores (Angular):**
- `.ts` = apresentador — contém toda a lógica: tratamento de strings, formatação, regras de exibição.
- `.html` = visualizador (humble) — apenas encaixa dados já tratados pelo apresentador.
- Permite testar o funcionamento da tela sem limitações do HTML.

**Gateways de Banco de Dados:**
- Repositório = toda a lógica de quais consultas fazer, quais campos retornar/enviar.
- DbSet/ORM = humble — gera SQL e executa. ORMs devem ficar contidos apenas na camada de persistência.

**Service Listeners:**
- Classes de entrada/saída de serviço = humble — apenas convertem dados para o formato do serviço (envio) ou da aplicação (recepção).

---

## 6. Limites Parciais (Cap. 24)

- Separação via interfaces e inversão de dependência **sem** separar em módulos distintos (build e publicação vinculados).
- Podem ser usados como **preparação** para separação real no futuro.
- **Risco:** tendem a enfraquecer com o tempo sem disciplina da equipe.

---

## 7. Componente Main (Cap. 26)

- Componente de **configuração e ligações externas**; "vê" todos os componentes externos e liga aos de política de alto nível.
- Deve poder haver **vários Mains**: dev, testes, produção, por país/cliente.
- É um componente "plug-in" — o mais externo do sistema.

---

## 8. Serviços (Cap. 27)

- Ter serviços diferentes **não significa** ter módulos desacoplados.
- Se alteração no serviço A obriga alteração no B → há dependência e acoplamento.
- Um serviço sem limites bem definidos é apenas um conjunto de classes com chamadas mais caras que usam outra URL.
- **Limites DENTRO, não ENTRE:** A fronteira arquitetural relevante está **dentro** de cada serviço (separação de políticas, camadas, componentes internos), não na fronteira de rede entre serviços. Microsserviços sem boa estrutura interna são monolitos distribuídos.
- **Serviços não são arquitetura:** São mecanismo de entrega/deploy. A arquitetura é a organização de políticas e regras dentro de cada unidade.

---

## 9. Conceitos complementares

### Independência (Cap. 16)

- **Lei de Conway:** organizações produzem designs que copiam sua estrutura de comunicação.
- Dimensões de independência: desenvolvimento (equipes paralelas), implantação (módulos publicáveis independentemente), serviço (operações independentes).
- **Duplicação aparente vs real:** antes de acoplar dois códigos semelhantes, avaliar se de fato compartilham razão de mudança ou se vão divergir no futuro.

### Fronteiras e Limites (Cap. 17)

- Limites existem para que partes do código não precisem conhecer outras, mas possam trabalhar juntas.
- Módulos de regra de negócio não devem saber qual banco, ORM ou framework será usado.
- Começar por bibliotecas de classes (regras de negócio), não por projetos de API/Console/Web.

### Política e Nível (Cap. 19)

- Políticas = passos do processo automatizado.
- Nível = distância das entradas/saídas do sistema (quanto mais longe, maior o nível).
- Níveis maiores não devem depender de níveis menores.

### Arquitetura Gritante (Cap. 21)

- A arquitetura deve expressar o **tipo de sistema** (gestão de pátio, imobiliária, etc.), não o framework usado.
- Frameworks não devem interferir na arquitetura.
- **Teste do grito:** ao olhar a estrutura de pastas, o leitor deve dizer "isso é um sistema de gestão de contratos" — não "isso é um projeto Angular" ou "isso é ASP.NET".
- **Organização por Componente > por Recurso > por Camada:**
  1. **Por Camada** (pior): `controllers/`, `services/`, `repositories/` — espalha domínio; não grita.
  2. **Por Recurso/Feature** (melhor): `pedidos/`, `usuarios/`, `contratos/` — agrupamento vertical por conceito.
  3. **Por Componente** (melhor): cada recurso como componente coeso com camadas internas — API pública estreita; internação das camadas.

### Paradigmas como Disciplinas Negativas (Cap. 3-4-5)

Paradigmas de programação não são capacidades adicionadas — são **restrições retiradas**:

| Paradigma | O que REMOVE | Implicação |
|-----------|-------------|------------|
| **Programação Estruturada** | `goto` — remove transferência direta de controle | Lógica organizada em sequência/seleção/iteração |
| **OO** | Ponteiros de função — remove transferência indireta | Polimorfismo seguro; DIP possível; inversão de dependência |
| **Funcional** | Atribuição — remove mutação de estado | Imutabilidade; sem race conditions; raciocínio mais fácil |

### Testes como Componentes Arquiteturais (Cap. 28)

- Testes são **parte da arquitetura**, não um artefato secundário.
- Design de testabilidade é decisão arquitetural: se o sistema é difícil de testar, a arquitetura tem problemas.
- Testes acoplados a detalhes (BD, framework, UI) são frágeis — aplicar Humble Object e DIP para isolar.
- **API estrutural de testes:** criar API específica para testes que desacopla de detalhes, permitindo evolução independente.

### BD, Web e Frameworks como Detalhes (Caps. 30-34)

| Detalhe | Por que é detalhe | Consequência |
|---------|-------------------|-------------|
| **Banco de Dados** | Modelo relacional/NoSQL é detalhe de armazenamento; dados em memória são entidades | Não deixar SQL/ORM vazar para regras de negócio; repositório abstrai |
| **Web** | Mecanismo de entrega (HTTP, REST) | Controllers são adaptadores; regras de negócio não sabem que estão na web |
| **Frameworks** | Ferramentas, não arquitetura | Manter framework na camada mais externa; não herdar de classes do framework em domínio |

### App-tidão (Cap. 28 – Kent Beck)

Etapas de escrita de código:
1. **Faça funcionar** — garantir as funcionalidades.
2. **Arrume** — facilitar/reduzir a manutenção.
3. **Otimize** — consumir menos e melhor os recursos.

---

## 10. Checklists

### Ao criar nova funcionalidade

1. **Localização:** Regra de domínio → Entidades. Processo da aplicação → Casos de Uso. Adaptação/apresentação → Adaptadores. Configuração → Frameworks & Drivers.
2. **Dependências:** Classe interna depende de UI/banco/framework concreto? → Reprojetar com interfaces e DIP. Risco de ciclo? → Quebrar com DIP.
3. **SOLID:** SRP (mais de uma razão para mudar? → dividir), OCP (precisa modificar estável? → extrair abstrações), LSP (subclasses funcionam sem ifs? → reprojetar), ISP (métodos não usados? → segregar), DIP (depende de concreto? → introduzir interface).
4. **Componentes:** CCP (classes que mudam juntas no mesmo componente?), CRP (consumidores dependem só do que usam?), REP (faz sentido como unidade de reúso/publicação?).

### Ao refatorar código existente

- Separar responsabilidades por atores/roles e por camadas.
- Remover dependências de regras de negócio para UI/banco/frameworks concretos.
- Quebrar ciclos entre componentes usando DA + DIP.
- Verificar: SOLID respeitado, direção de dependência correta, camadas internas independentes de detalhes externos.
