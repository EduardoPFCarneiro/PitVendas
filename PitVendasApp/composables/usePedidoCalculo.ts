import { computed, type Ref } from 'vue';

export interface ItemPedido {
  produtoId: number;
  codigoProduto: string;
  descricaoProduto: string;
  corId: number;
  nomeCor: string;
  valorUnitario: number;
  grade: { tamanhoId: number; codigoTamanho: string; quantidade: number }[];
}

export function usePedidoCalculo(
  itens: Ref<ItemPedido[]>,
  percentualDesconto: Ref<number>,
  percentualComissao: Ref<number>,
) {
  const quantidadeItem = (item: ItemPedido) =>
    item.grade.reduce((soma, g) => soma + (g.quantidade || 0), 0);

  const subtotalItem = (item: ItemPedido) =>
    quantidadeItem(item) * item.valorUnitario;

  const valorBruto = computed(() =>
    itens.value.reduce((soma, i) => soma + subtotalItem(i), 0),
  );

  const totalPecas = computed(() =>
    itens.value.reduce((soma, i) => soma + quantidadeItem(i), 0),
  );

  const desconto = computed(() =>
    Math.round(valorBruto.value * (percentualDesconto.value || 0)) / 100,
  );

  const valorLiquido = computed(() => valorBruto.value - desconto.value);

  const valorComissao = computed(() =>
    Math.round(valorLiquido.value * (percentualComissao.value || 0)) / 100,
  );

  return {
    quantidadeItem,
    subtotalItem,
    valorBruto,
    totalPecas,
    desconto,
    valorLiquido,
    valorComissao,
  };
}
