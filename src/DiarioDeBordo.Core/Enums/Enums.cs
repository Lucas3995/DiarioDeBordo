namespace DiarioDeBordo.Core.Enums;

public enum FormatoMidia { Nenhum, Audio, Texto, Video, Imagem, Interativo, Misto }
public enum PapelConteudo { Item, Coletanea }
public enum TipoColetanea { Guiada, Miscelanea, Subscricao }
public enum TipoFonte { Url, ArquivoLocal, Rss, Identificador }
public enum OrigemImagem { Manual, Automatica }
public enum EstadoProgresso { NaoIniciado, EmAndamento, Concluido }

/// <summary>Reação imediata do usuário ao conteúdo. Null = não classificado (D-08).</summary>
public enum Classificacao { Gostei, NaoGostei }
