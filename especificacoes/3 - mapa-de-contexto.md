# Mapa de Contexto — Sistema de Gestão e Consumo de Conteúdo com Agência do Usuário

**Versão:** 1.0
**Documentos base:** Definição de Domínio v3, Mapa de Domínio v1

---

## 1. Bounded Contexts

### 1.1. Acervo (Principal)

**Subdomínios agrupados:** Gestão de Conteúdo + Curadoria e Coletâneas.

**Justificativa da fusão:** Compartilham a mesma linguagem ubíqua e operam sobre as mesmas entidades. Conteúdo, Coletânea, Progresso, Categoria, Relação, Anotação — todos esses termos significam a mesma coisa para ambos. Separá-los criaria uma fronteira artificial com tradução de conceitos idênticos. Dentro do monolito modular, podem ser módulos distintos mas dentro do mesmo contexto semântico.

**Linguagem ubíqua:**
- Conteúdo: qualquer coisa sobre a qual o usuário registra informações.
- Formato de mídia: natureza material do conteúdo (áudio, texto, vídeo, imagem, interativo, misto, nenhum).
- Papel estrutural: item ou coletânea.
- Coletânea: conteúdo que agrupa outros conteúdos, com tipo (guiada, miscelânea, subscrição) e configurações comportamentais (ordenação, acompanhamento sequencial).
- Progresso: estado global de consumo do conteúdo.
- Categoria: tag livre atribuída pelo usuário.
- Relação: vínculo bidirecional entre conteúdos com tipo textual.
- Anotação contextual: anotação na relação conteúdo-coletânea.
- Fonte: origem do conteúdo com prioridade e fallback.
- Gancho: marcação em parte específica de um conteúdo.

**Responsabilidades:**
- Ciclo de vida completo da entidade Conteúdo (criação, edição, classificação, progresso, histórico de ações, deduplicação).
- Estrutura e comportamento de coletâneas (tipos, composição, proteção contra ciclos, anotação contextual).
- Categorias, relações entre conteúdos, imagens.
- Hierarquia de autoridade de metadados.
- Define a estrutura da coletânea Subscrição (quais fontes, configurações comportamentais), mas não o comportamento de montagem de feed (isso pertence à Agregação).

---

### 1.2. Agregação (Principal)

**Subdomínio:** Agregação e Subscrição.

**Linguagem ubíqua:**
- Item de feed: referência efêmera a um conteúdo descoberto em uma fonte externa. Existe enquanto a visão está montada. Pode nunca virar um registro.
- Visão: dados montados sob demanda, não persistidos.
- Feed: visão de itens de uma subscrição, ordenados cronologicamente.
- Agregador: visão consolidada de múltiplos feeds.
- Persistência seletiva: apenas itens com interação do usuário geram registros no Acervo.
- Montagem de feed: operação de consultar fontes externas para descobrir quais itens existem.

**Responsabilidades:**
- Montagem de feeds sob demanda a partir de fontes configuradas nas subscrições.
- Agregador como visão consolidada de múltiplas subscrições.
- Decisão de o que é visão (efêmero) vs. o que vira registro (quando o usuário interage).
- Filtros do agregador (por criador, palavras-chave, esconder consumidos).
- Comportamento offline de subscrições (apresentar apenas itens com registro, sinalizar incompletude).
- Apresentação de cards (metadados quando possível, links quando não).
- Regras de transição: quando um item de feed se torna um conteúdo no Acervo.

**Tradução conceitual com o Acervo:** Um "item de feed" na Agregação é um conceito diferente de "conteúdo" no Acervo. O item de feed é efêmero, incompleto, sem progresso nem anotações. Quando o usuário interage com um item de feed, a Agregação solicita ao Acervo a criação de um conteúdo a partir desse item — esse é o momento de tradução.

---

### 1.3. Reprodução (Suporte)

**Subdomínio:** Reprodução de Conteúdo.

**Linguagem ubíqua:**
- Reprodução interna: consumo do conteúdo dentro do sistema.
- Abertura externa: delegação da reprodução ao OS ou aplicativo escolhido.
- Fallback: tentativa da próxima fonte quando a atual está indisponível.
- Embed: incorporação de conteúdo de plataforma externa no reprodutor interno.
- Posição: ponto específico dentro de um conteúdo sendo reproduzido (timestamp, página, marcação).

**Responsabilidades:**
- Reprodutor interno (leitor de texto, player de áudio, embed de vídeo).
- Abertura externa (app padrão do OS ou escolhido pelo usuário).
- Navegação entre fontes por prioridade (fallback).
- Ganchos (criação e navegação de marcações dentro do conteúdo).
- Oferta de marcação automática de progresso ao Acervo.
- Configuração de comportamento padrão por subtipo de formato.

**O que este contexto recebe:** Um conteúdo com suas fontes ordenadas por prioridade e seu formato de mídia. Não precisa saber sobre coletâneas, categorias ou relações.

---

### 1.4. Integração Externa (Suporte)

**Subdomínio:** Integração com Plataformas Externas.

**Linguagem ubíqua:**
- Adaptador: componente que se conecta a uma plataforma específica e traduz seus dados para o formato interno.
- Plataforma: fonte externa de conteúdo (YouTube, Instagram, RSS, Amazon, etc.).
- Metadados externos: título, descrição, thumbnail e outros dados obtidos da plataforma de origem.
- Parser de importação: componente que transforma dados exportados de uma plataforma em registros do modelo de domínio.

**Responsabilidades:**
- Adaptadores por plataforma (YouTube, Instagram, RSS, Amazon, etc.).
- Consulta a fontes para montagem de feed (a pedido da Agregação).
- Obtenção de metadados de conteúdos específicos (a pedido do Acervo).
- Busca de conteúdo em plataformas externas.
- Tratamento de indisponibilidade (plataformas offline, APIs alteradas, bloqueios).
- Parsers de importação de dados de plataformas existentes (futuro).

**Papel de Anti-Corruption Layer:** Este contexto protege o domínio interno da instabilidade e diversidade das APIs externas. Internamente, as plataformas mudam APIs, alteram formatos, bloqueiam acessos. Externamente ao domínio, isso é invisível — a Integração traduz tudo para uma representação padronizada que Agregação e Acervo consomem.

---

### 1.5. Busca (Suporte)

**Subdomínio:** Busca e Navegação.

**Linguagem ubíqua:**
- Resultado: projeção de um conteúdo otimizada para apresentação em lista filtrada.
- Filtro: critério de seleção sobre o acervo (formato, categoria, nota, progresso, etc.).
- Busca textual: pesquisa por correspondência em título, descrição e anotações.
- Operação em lote: ação aplicada a múltiplos conteúdos simultaneamente.

**Responsabilidades:**
- Busca textual interna sobre o acervo.
- Filtros combinados sobre todos os atributos relevantes.
- Busca em plataformas externas (delegando à Integração Externa).
- Operações em lote.

**Tradução conceitual com o Acervo:** "Conteúdo" na Busca é um "resultado" — uma projeção otimizada para filtragem e apresentação, não a entidade completa com todo o grafo de relações e histórico. A Busca consome dados do Acervo mas pode manter sua própria estrutura de indexação.

---

### 1.6. Portabilidade (Suporte)

**Subdomínio:** Portabilidade de Dados.

**Linguagem ubíqua:**
- Pacote: arquivo ou conjunto de arquivos contendo dados exportados.
- Exportação: serialização dos dados do sistema em um pacote.
- Importação: deserialização de um pacote nos registros do sistema.
- Parser: componente que interpreta um formato de pacote específico.

**Responsabilidades:**
- Exportação de dados do usuário (conteúdos, coletâneas, progresso, anotações, configurações pessoais).
- Exportação de configurações globais (admin).
- Importação de pacotes em outra instância.
- Formato agnóstico de plataforma, legível e documentado.

---

### 1.7. Identidade (Genérico)

**Subdomínio:** Identidade e Acesso.

**Linguagem ubíqua:**
- Usuário: pessoa autenticada no sistema.
- Role: papel fixo (consumidor, admin).
- Grupo: combinação de roles criada pelo admin.
- Sessão: período autenticado de uso.
- Área: região do sistema acessível conforme a role (consumidor ou admin).

**Responsabilidades:**
- Autenticação.
- Autorização baseada em roles e grupos.
- Gerenciamento de usuários (criar, ativar, inativar, resetar senha).
- Ocultação da área admin para não-admins.

---

### 1.8. Preferências (Genérico)

**Subdomínio:** Personalização de Interface.

**Linguagem ubíqua:**
- Tema: conjunto de cores e estilos visuais aplicados à interface.
- Preferência: configuração individual do usuário sobre a aparência ou comportamento da interface.
- Default global: valor padrão definido pelo admin, sobrescrevível pelo usuário.

**Responsabilidades:**
- Configurações de fonte, cores, temas.
- Configurações de acessibilidade.
- Mecanismo de defaults globais e sobrescrita por usuário.
- Disclosure progressivo configurável.

---

## 2. Relações entre Contextos

### 2.1. Acervo ↔ Agregação — Partnership

**Padrão:** Partnership.

**Justificativa:** A Subscrição é um tipo de coletânea definida no Acervo, mas seu comportamento de montagem de feed é responsabilidade da Agregação. Os dois contextos precisam evoluir juntos no que toca à Subscrição: se o Acervo mudar a estrutura de uma coletânea Subscrição, a Agregação precisa acompanhar, e vice-versa.

**Fronteira:** A estrutura da coletânea Subscrição (quais fontes, configurações comportamentais, proteção contra ciclos) vive no Acervo. O comportamento de montar o feed, consultar fontes e decidir o que é visão vs. registro vive na Agregação.

**Momento de tradução:** Quando o usuário interage com um item de feed (anota, dá nota, registra progresso), a Agregação solicita ao Acervo a criação de um conteúdo. O item de feed (efêmero, incompleto) é traduzido em um conteúdo (registro persistente com todos os atributos).

---

### 2.2. Integração Externa → Agregação — Customer/Supplier

**Padrão:** Customer/Supplier.

**Supplier (fornecedor):** Integração Externa.
**Customer (consumidor):** Agregação.

**Justificativa:** A Agregação define o contrato do que precisa — um formato padronizado de item de feed (título, descrição, URL, thumbnail, data de publicação, identificador da fonte). A Integração adapta cada plataforma externa para entregar nesse formato. A Agregação não sabe e não precisa saber como o YouTube ou Instagram funcionam.

**Anti-Corruption Layer:** A Integração Externa funciona como ACL, protegendo a Agregação da instabilidade das APIs externas. Quando uma API muda, apenas o adaptador correspondente na Integração é alterado.

---

### 2.3. Acervo → Reprodução — Customer/Supplier

**Padrão:** Customer/Supplier.

**Supplier (fornecedor):** Acervo.
**Customer (consumidor):** Reprodução.

**Justificativa:** A Reprodução define o que precisa — um conteúdo com formato de mídia, fontes ordenadas por prioridade, e posição atual de progresso. O Acervo entrega essa representação. A Reprodução não precisa saber sobre coletâneas, categorias, relações ou anotações.

**Retorno:** Quando a reprodução avança, a Reprodução informa o Acervo da nova posição de progresso (se o usuário aceitar a marcação automática).

---

### 2.4. Integração Externa → Acervo — Customer/Supplier

**Padrão:** Customer/Supplier.

**Supplier (fornecedor):** Integração Externa.
**Customer (consumidor):** Acervo.

**Justificativa:** Quando o Acervo precisa enriquecer um conteúdo com metadados (thumbnail, descrição, título) buscados da fonte de origem ou de fontes de metadados externas, ele solicita à Integração. O Acervo define o contrato (quais metadados precisa), a Integração adapta cada plataforma para entregar.

---

### 2.5. Acervo → Busca — Customer/Supplier

**Padrão:** Customer/Supplier.

**Supplier (fornecedor):** Acervo.
**Customer (consumidor):** Busca.

**Justificativa:** A Busca opera sobre uma projeção indexada dos dados do Acervo. O Acervo é a fonte de verdade; a Busca consome os dados e pode manter índices próprios otimizados para consultas rápidas.

---

### 2.6. Busca → Integração Externa — Customer/Supplier

**Padrão:** Customer/Supplier.

**Supplier (fornecedor):** Integração Externa.
**Customer (consumidor):** Busca.

**Justificativa:** Quando o usuário busca conteúdo em plataformas externas, a Busca delega à Integração Externa a consulta e recebe resultados no formato padronizado.

---

### 2.7. Acervo + Preferências → Portabilidade — Customer/Supplier

**Padrão:** Customer/Supplier.

**Suppliers (fornecedores):** Acervo, Preferências.
**Customer (consumidor):** Portabilidade.

**Justificativa:** A Portabilidade precisa ler e escrever dados do Acervo (conteúdos, coletâneas, progresso) e das Preferências (configurações do usuário) para serializar/deserializar. Define o formato do pacote de exportação.

---

### 2.8. Identidade → Todos — Open Host Service

**Padrão:** Open Host Service.

**Justificativa:** A Identidade expõe um serviço padronizado que todos os contextos consomem: quem é o usuário autenticado e qual role possui. Nenhum contexto precisa conhecer os detalhes internos da autenticação — apenas consultar se o usuário tem permissão.

---

### 2.9. Preferências → Todos que renderizam — Open Host Service

**Padrão:** Open Host Service.

**Justificativa:** As Preferências expõem as configurações do usuário (tema, fonte, contraste, disclosure progressivo) para qualquer contexto que renderize interface. Cada contexto consome as preferências sem precisar saber como são armazenadas ou gerenciadas.

---

## 3. Mapa Visual

```
                    ┌──────────────────────────────────────────────┐
                    │          USO SAUDÁVEL (transversal)          │
                    │  Restrições sobre o design de todos os BCs   │
                    └──────────────────────────────────────────────┘

    ┌───── PRINCIPAL ──────────────────────────────────────────────────┐
    │                                                                  │
    │  ┌─────────────────────┐  Partnership  ┌──────────────────────┐ │
    │  │       ACERVO        │◄─────────────►│     AGREGAÇÃO        │ │
    │  │    (Prioridade 1)   │               │    (Prioridade 2)    │ │
    │  │                     │               │                      │ │
    │  │ Conteúdo, Coletânea │               │ Item de feed, Visão  │ │
    │  │ Progresso, Categoria│               │ Feed, Agregador      │ │
    │  │ Relação, Fonte      │               │ Persistência seletiva│ │
    │  └──┬──────┬───────────┘               └──────┬───────────────┘ │
    │     │      │                                   │                 │
    └─────┼──────┼───────────────────────────────────┼─────────────────┘
          │      │                                   │
          │      │  ┌────────────────────────────────┘
          │      │  │
    ┌─────┼──────┼──┼──────────────────────────────────────────────────┐
    │     │      │  │           SUPORTE                                │
    │     │      │  │                                                  │
    │     │  C/S │  │ C/S                                              │
    │     │      │  │                                                  │
    │     │  ┌───▼──▼────────────────┐    ┌──────────────────────────┐│
    │     │  │  INTEGRAÇÃO EXTERNA   │    │        BUSCA             ││
    │     │  │                       │    │                          ││
    │     │  │  Adaptadores (ACL)    │    │  Resultados, Filtros     ││
    │     │  │  Metadados externos   │    │  Busca textual           ││
    │     │  │  Parsers              │    │  Operações em lote       ││
    │     │  └───────────────────────┘    └──────────────────────────┘│
    │     │                                                            │
    │  C/S│  ┌───────────────────────┐    ┌──────────────────────────┐│
    │     │  │     REPRODUÇÃO        │    │     PORTABILIDADE        ││
    │     │  │                       │    │                          ││
    │     └─►│  Reprodutor interno   │    │  Pacote, Exportação      ││
    │        │  Abertura externa     │    │  Importação              ││
    │        │  Fallback, Ganchos    │    │                          ││
    │        └───────────────────────┘    └──────────────────────────┘│
    │                                                                  │
    └──────────────────────────────────────────────────────────────────┘

    ┌─── GENÉRICO ─────────────────────────────────────────────────────┐
    │                                                                  │
    │  ┌─────────────────────┐         ┌───────────────────────────┐  │
    │  │    IDENTIDADE       │         │     PREFERÊNCIAS          │  │
    │  │  (Open Host Service)│         │  (Open Host Service)      │  │
    │  │                     │         │                           │  │
    │  │  Usuário, Role      │         │  Tema, Fonte, Contraste   │  │
    │  │  Grupo, Sessão      │         │  Defaults globais         │  │
    │  └─────────────────────┘         └───────────────────────────┘  │
    │         ▲ consumido por todos          ▲ consumido por todos    │
    │                                          que renderizam         │
    └──────────────────────────────────────────────────────────────────┘

    Legenda:
    ◄──────► Partnership (evolução conjunta)
    C/S      Customer/Supplier (quem define o contrato / quem entrega)
    ACL      Anti-Corruption Layer (protege domínio interno)
    OHS      Open Host Service (serviço padronizado para todos)
```

---

## 4. Notas de Decisão

### 4.1. Fusão de Gestão de Conteúdo e Curadoria no contexto Acervo

Gestão de Conteúdo e Curadoria são subdomínios com preocupações distintas (o que um conteúdo é vs. como ele é organizado) mas compartilham a mesma linguagem ubíqua. Dentro de um monolito modular, podem ser módulos separados (ex: módulo de conteúdo e módulo de coletânea), mas pertencem ao mesmo bounded context porque não há tradução conceitual entre eles.

### 4.2. Item de feed vs. Conteúdo como fronteira Agregação/Acervo

A distinção entre "item de feed" (efêmero, incompleto, não persistido) e "conteúdo" (registro persistente com atributos completos) é a fronteira mais importante do sistema. Ela materializa a regra de persistência seletiva e evita que o sistema acumule dados sobre conteúdos com os quais o usuário nunca interagiu.

### 4.3. Integração Externa como Anti-Corruption Layer

A Integração Externa é o único contexto que conhece as APIs das plataformas. Todos os demais contextos trabalham com representações internas padronizadas. Isso permite que APIs externas mudem sem impactar regras de negócio, e que novas plataformas sejam adicionadas sem alterar Agregação nem Acervo.

### 4.4. Identidade e Preferências como Open Host Service

Ambos os contextos genéricos expõem serviços padronizados consumidos por todos. Não há relação Customer/Supplier porque não há negociação de contrato — o serviço é estável e consumido sem customização.
