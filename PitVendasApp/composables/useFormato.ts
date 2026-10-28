export function useFormato() {
  const moeda = (valor?: number | null) =>
    (valor ?? 0).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

  const numero = (valor?: number | null) =>
    (valor ?? 0).toLocaleString('pt-BR');

  const data = (valor?: string | Date | null) => {
    if (!valor) return '';
    const d = typeof valor === 'string' ? new Date(valor) : valor;
    return Number.isNaN(d.getTime()) ? '' : d.toLocaleDateString('pt-BR');
  };

  const dataHora = (valor?: string | Date | null) => {
    if (!valor) return '';
    const d = typeof valor === 'string' ? new Date(valor) : valor;
    return Number.isNaN(d.getTime())
      ? ''
      : d.toLocaleString('pt-BR', { dateStyle: 'short', timeStyle: 'short' });
  };

  const percentual = (valor?: number | null) =>
    `${(valor ?? 0).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}%`;

  const cnpj = (valor?: string | null) => {
    const so = (valor ?? '').replace(/\D/g, '');
    if (so.length !== 14) return valor ?? '';
    return so.replace(/^(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})$/, '$1.$2.$3/$4-$5');
  };

  const telefone = (valor?: string | null) => {
    const so = (valor ?? '').replace(/\D/g, '');
    if (so.length === 11) return so.replace(/^(\d{2})(\d{5})(\d{4})$/, '($1) $2-$3');
    if (so.length === 10) return so.replace(/^(\d{2})(\d{4})(\d{4})$/, '($1) $2-$3');
    return valor ?? '';
  };

  const soDigitos = (valor?: string | null) => (valor ?? '').replace(/\D/g, '');

  return { moeda, numero, data, dataHora, percentual, cnpj, telefone, soDigitos };
}
