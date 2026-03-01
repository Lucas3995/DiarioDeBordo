import { formatarDataRelativa, formatarDataTooltip } from './datas';

describe('formatarDataRelativa', () => {
  it('deve retornar "hoje" quando a data é hoje', () => {
    const hoje = new Date();
    expect(formatarDataRelativa(hoje)).toBe('hoje');
  });

  it('deve retornar "há 1 dia" quando a data foi ontem', () => {
    const ontem = new Date();
    ontem.setDate(ontem.getDate() - 1);
    expect(formatarDataRelativa(ontem)).toBe('há 1 dia');
  });

  it('deve retornar "há N dias" quando menos de 30 dias', () => {
    const diasAtras = new Date();
    diasAtras.setDate(diasAtras.getDate() - 5);
    expect(formatarDataRelativa(diasAtras)).toBe('há 5 dias');
  });

  it('deve retornar "há 1 mês" entre 30 e 59 dias', () => {
    const data = new Date();
    data.setDate(data.getDate() - 45);
    expect(formatarDataRelativa(data)).toBe('há 1 mês');
  });

  it('deve retornar "há N meses" quando menos de 1 ano', () => {
    const data = new Date();
    data.setMonth(data.getMonth() - 3);
    expect(formatarDataRelativa(data)).toBe('há 3 meses');
  });

  it('deve retornar "há 1 ano" entre 365 e 729 dias', () => {
    const data = new Date();
    data.setFullYear(data.getFullYear() - 1);
    expect(formatarDataRelativa(data)).toBe('há 1 ano');
  });

  it('deve retornar "há N anos" quando 730+ dias', () => {
    const data = new Date();
    data.setFullYear(data.getFullYear() - 2);
    expect(formatarDataRelativa(data)).toBe('há 2 anos');
  });
});

describe('formatarDataTooltip', () => {
  it('deve formatar no padrão dd/MM/yyyy', () => {
    expect(formatarDataTooltip(new Date(2026, 2, 15))).toBe('15/03/2026');
  });

  it('deve preencher dia e mês com zero à esquerda', () => {
    expect(formatarDataTooltip(new Date(2026, 0, 5))).toBe('05/01/2026');
  });
});
