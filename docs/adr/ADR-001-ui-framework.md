# ADR-001: Framework de Interface — Avalonia UI + SukiUI

**Data:** 2026-04-02
**Status:** Aceito

## Contexto

O sistema DiarioDeBordo é uma aplicação desktop nativa com suporte obrigatório a Linux e Windows desde a Etapa 1. A interface precisa funcionar sem dependência de browser ou WebView, suportar tema escuro por padrão, oferecer componentes customizáveis e atender à conformidade WCAG 2.2 AAA.

Forças em tensão:
- Cross-platform nativo (Linux + Windows) sem Electron
- Ecossistema .NET (C#) já definido para o backend
- Necessidade de componentes modernos com suporte a temas claro/escuro
- Suporte ao padrão MVVM com separação clara de ViewModel e View

**Referência:** Padrões Técnicos v4, seção 2.1 — Stack de Interface.

## Decisão

Adotamos Avalonia UI como framework de UI e SukiUI como biblioteca de componentes. O padrão MVVM é implementado com CommunityToolkit.Mvvm.

## Consequências

### Positivas
- Runtime próprio baseado em Skia — sem dependência de WebView ou Electron
- Suporte nativo a Linux e Windows (cross-platform real, não emulado)
- MVVM nativo com data binding bidirecional
- SukiUI fornece temas claro/escuro prontos, reduzindo trabalho de estilização
- Licença MIT — sem custo de licença comercial
- Adotado pelo JetBrains, o que sinaliza maturidade e longevidade do projeto

### Negativas / Trade-offs
- Ecossistema menor que WPF e MAUI em termos de componentes de terceiros
- SukiUI tem menos componentes que bibliotecas equivalentes para web (MUI, Ant Design)
- Curva de aprendizado maior que WinForms para desenvolvedores sem experiência em MVVM

## Alternativas Consideradas

| Alternativa | Por que não adotada |
|---|---|
| MAUI | Não tem suporte real a Linux desktop — foco é mobile e Windows |
| Electron | Não é nativo; consome significativamente mais memória (processo Chromium); não é aplicação .NET |
| WPF | Somente Windows — viola o requisito cross-platform obrigatório desde a Etapa 1 |
| WinUI 3 | Somente Windows — mesmo problema do WPF |
