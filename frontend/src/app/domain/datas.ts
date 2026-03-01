/**
 * Formata uma data em texto relativo ao dia atual (ex.: "hoje", "há 3 dias", "há 1 mês").
 * Usado na listagem de obras para exibir a última atualização de posição.
 */
export function formatarDataRelativa(data: Date): string {
  const hoje = new Date();
  const diff = hoje.getTime() - data.getTime();
  const dias = Math.floor(diff / (1000 * 60 * 60 * 24));

  if (dias === 0) return 'hoje';
  if (dias === 1) return 'há 1 dia';
  if (dias < 30) return `há ${dias} dias`;
  if (dias < 60) return 'há 1 mês';
  if (dias < 365) return `há ${Math.floor(dias / 30)} meses`;
  if (dias < 730) return 'há 1 ano';
  return `há ${Math.floor(dias / 365)} anos`;
}

/**
 * Formata uma data no padrão dd/MM/yyyy para tooltip ou exibição fixa.
 */
export function formatarDataTooltip(data: Date): string {
  const d = data.getDate().toString().padStart(2, '0');
  const m = (data.getMonth() + 1).toString().padStart(2, '0');
  const y = data.getFullYear();
  return `${d}/${m}/${y}`;
}
