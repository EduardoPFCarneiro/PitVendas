<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import {
  useClientesApi, useFabricasApi, useProdutosApi,
} from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useNotificacaoStore } from '@/stores/notificacao';
import { usePedidoCalculo, type ItemPedido } from '@/composables/usePedidoCalculo';
import { mensagemDeErro } from '@/utils/api';
import type {
  Pedido, PedidoForm, ClienteLista, FabricaLista, ProdutoLista, Produto,
} from '@/types/api';

const props = defineProps<{
  valorInicial?: Pedido | null;
  salvando: boolean;
}>();

const emit = defineEmits<{ salvar: [dados: PedidoForm] }>();

const clientesApi = useClientesApi();
const fabricasApi = useFabricasApi();
const produtosApi = useProdutosApi();
const { moeda, percentual } = useFormato();
const notificar = useNotificacaoStore();

// -------------------------- cabeçalho --------------------------
const clientes = ref<ClienteLista[]>([]);
const fabricas = ref<FabricaLista[]>([]);

const clienteId = ref<number | null>(null);
const fabricaId = ref<number | null>(null);
const tabelaPreco = ref<string | null>(null);
const percentualDesconto = ref(0);
const dataFaturamento = ref<string | null>(null);
const prazo = ref<string | null>(null);
const frete = ref<string | null>(null);
const condicaoPagamento = ref<string | null>(null);
const parcelas = ref(1);
const observacao = ref<string | null>(null);

const itens = ref<ItemPedido[]>([]);

const fabricaSelecionada = computed(() =>
  fabricas.value.find((f) => f.id === fabricaId.value) ?? null,
);
const percentualComissao = computed(() => fabricaSelecionada.value?.comissao ?? 0);

const { quantidadeItem, subtotalItem, valorBruto, totalPecas, desconto, valorLiquido, valorComissao } =
  usePedidoCalculo(itens, percentualDesconto, percentualComissao);

// ------------------ entrada de item (grade) ------------------
const produtosDaFabrica = ref<ProdutoLista[]>([]);
const produtoSelecionadoId = ref<number | null>(null);
const produtoAtual = ref<Produto | null>(null);
const corEntradaId = ref<number | null>(null);
const valorEntrada = ref<number>(0);
const gradeEntrada = ref<{ tamanhoId: number; codigoTamanho: string; quantidade: number }[]>([]);

watch(fabricaId, async (nova, antiga) => {
  if (antiga != null && itens.value.length) {
    itens.value = [];
    notificar.info('Itens limpos', 'A fábrica do pedido mudou.');
  }
  produtoSelecionadoId.value = null;
  produtoAtual.value = null;
  produtosDaFabrica.value = [];
  if (nova) {
    const r = await produtosApi.listar({ fabricaId: nova, ativo: true, tamanhoPagina: 200 });
    produtosDaFabrica.value = r.itens;
  }
});

watch(produtoSelecionadoId, async (id) => {
  produtoAtual.value = null;
  corEntradaId.value = null;
  gradeEntrada.value = [];
  valorEntrada.value = 0;
  if (!id) return;

  produtoAtual.value = await produtosApi.obter(id);
  gradeEntrada.value = produtoAtual.value.tamanhos.map((t) => ({
    tamanhoId: t.id,
    codigoTamanho: t.nome,
    quantidade: 0,
  }));
  valorEntrada.value = precoSugerido(produtoAtual.value);
});

function precoSugerido(produto: Produto): number {
  const daTabela = produto.precos.find((p) => p.ativo && p.codigo === tabelaPreco.value);
  if (daTabela) return daTabela.valor;
  const ativos = produto.precos.filter((p) => p.ativo).map((p) => p.valor);
  return ativos.length ? Math.min(...ativos) : 0;
}

const totalEntrada = computed(() =>
  gradeEntrada.value.reduce((s, g) => s + (g.quantidade || 0), 0),
);

function adicionarItem() {
  if (!produtoAtual.value) {
    notificar.aviso('Selecione um produto', 'Escolha o produto antes de adicionar.');
    return;
  }
  if (!corEntradaId.value) {
    notificar.aviso('Selecione a cor', 'Informe a cor do item.');
    return;
  }
  if (totalEntrada.value <= 0) {
    notificar.aviso('Grade vazia', 'Informe a quantidade em ao menos um tamanho.');
    return;
  }
  if (valorEntrada.value <= 0) {
    notificar.aviso('Valor inválido', 'Informe o valor unitário do item.');
    return;
  }

  const jaExiste = itens.value.some(
    (i) => i.produtoId === produtoAtual.value!.id && i.corId === corEntradaId.value,
  );
  if (jaExiste) {
    notificar.aviso('Item repetido', 'Este produto nesta cor já está no pedido.');
    return;
  }

  const cor = produtoAtual.value.cores.find((c) => c.id === corEntradaId.value)!;

  itens.value.push({
    produtoId: produtoAtual.value.id,
    codigoProduto: produtoAtual.value.codigo,
    descricaoProduto: produtoAtual.value.descricao,
    corId: cor.id,
    nomeCor: cor.nome,
    valorUnitario: valorEntrada.value,
    grade: gradeEntrada.value
      .filter((g) => g.quantidade > 0)
      .map((g) => ({ ...g })),
  });

  produtoSelecionadoId.value = null;
}

function removerItem(indice: number) {
  itens.value.splice(indice, 1);
}

function resumoGrade(item: ItemPedido) {
  return item.grade.map((g) => `${g.codigoTamanho}:${g.quantidade}`).join('  ');
}

// ----------------------------- salvar -----------------------------
function submeter() {
  if (!clienteId.value) {
    notificar.aviso('Selecione o cliente', 'O pedido precisa de um cliente.');
    return;
  }
  if (!fabricaId.value) {
    notificar.aviso('Selecione a fábrica', 'O pedido precisa de uma fábrica.');
    return;
  }
  if (!itens.value.length) {
    notificar.aviso('Pedido vazio', 'Adicione ao menos um item.');
    return;
  }

  emit('salvar', {
    clienteId: clienteId.value,
    fabricaId: fabricaId.value,
    tabelaPreco: tabelaPreco.value,
    percentualDesconto: percentualDesconto.value || 0,
    dataFaturamento: dataFaturamento.value,
    prazo: prazo.value,
    frete: frete.value,
    condicaoPagamento: condicaoPagamento.value,
    parcelas: parcelas.value,
    observacao: observacao.value,
    itens: itens.value.map((i) => ({
      produtoId: i.produtoId,
      corId: i.corId,
      valorUnitario: i.valorUnitario,
      grade: i.grade.map((g) => ({ tamanhoId: g.tamanhoId, quantidade: g.quantidade })),
    })),
  });
}

onMounted(async () => {
  const [rc, rf] = await Promise.all([
    clientesApi.listar({ ativo: true, tamanhoPagina: 200 }),
    fabricasApi.listar({ ativo: true, tamanhoPagina: 200 }),
  ]);
  clientes.value = rc.itens;
  fabricas.value = rf.itens;

  if (props.valorInicial) {
    const v = props.valorInicial;
    clienteId.value = v.clienteId;
    fabricaId.value = v.fabricaId;
    tabelaPreco.value = v.tabelaPreco ?? null;
    percentualDesconto.value = v.percentualDesconto;
    dataFaturamento.value = v.dataFaturamento ?? null;
    prazo.value = v.prazo ?? null;
    frete.value = v.frete ?? null;
    condicaoPagamento.value = v.condicaoPagamento ?? null;
    parcelas.value = v.parcelas;
    observacao.value = v.observacao ?? null;

    itens.value = v.itens.map((i) => ({
      produtoId: i.produtoId,
      codigoProduto: i.codigoProduto,
      descricaoProduto: i.descricaoProduto,
      corId: i.corId,
      nomeCor: i.nomeCor,
      valorUnitario: i.valorUnitario,
      grade: i.grade.map((g) => ({
        tamanhoId: g.tamanhoId,
        codigoTamanho: g.codigoTamanho,
        quantidade: g.quantidade,
      })),
    }));
  }
});
</script>

<template>
  <div>
    <!-- ======================= cabeçalho ======================= -->
    <v-card elevation="0" rounded="lg" class="mb-6">
      <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Dados do Pedido</h5></v-card-item>
      <v-divider />
      <v-card-text class="pa-6">
        <v-row>
          <v-col cols="12" md="4">
            <v-label class="mb-2 font-weight-medium">Cliente *</v-label>
            <v-select v-model="clienteId" :items="clientes" item-title="nome" item-value="id" placeholder="Selecione o cliente" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="4">
            <v-label class="mb-2 font-weight-medium">Fábrica *</v-label>
            <v-select v-model="fabricaId" :items="fabricas" item-title="nome" item-value="id" placeholder="Selecione a fábrica" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="2">
            <v-label class="mb-2 font-weight-medium">Tabela</v-label>
            <v-text-field v-model="tabelaPreco" placeholder="Ex.: P1" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="2">
            <v-label class="mb-2 font-weight-medium">Desconto (%)</v-label>
            <v-text-field v-model.number="percentualDesconto" type="number" min="0" max="100" step="0.5" suffix="%" variant="outlined" density="comfortable" hide-details />
          </v-col>

          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Faturamento</v-label>
            <v-text-field v-model="dataFaturamento" type="date" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Condição de Pagamento</v-label>
            <v-text-field v-model="condicaoPagamento" placeholder="Ex.: 30/60/90" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="2">
            <v-label class="mb-2 font-weight-medium">Parcelas</v-label>
            <v-text-field v-model.number="parcelas" type="number" min="1" max="60" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="2">
            <v-label class="mb-2 font-weight-medium">Prazo</v-label>
            <v-text-field v-model="prazo" placeholder="Ex.: 15 dias" variant="outlined" density="comfortable" hide-details />
          </v-col>
          <v-col cols="12" md="2">
            <v-label class="mb-2 font-weight-medium">Frete</v-label>
            <v-text-field v-model="frete" placeholder="Ex.: CIF" variant="outlined" density="comfortable" hide-details />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <!-- ==================== entrada de item ==================== -->
    <v-card elevation="0" rounded="lg" class="mb-6">
      <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Adicionar Item</h5></v-card-item>
      <v-divider />
      <v-card-text class="pa-6">
        <v-alert v-if="!fabricaId" type="info" variant="tonal" density="comfortable">
          Selecione a fábrica no topo para carregar os produtos disponíveis.
        </v-alert>

        <template v-else>
          <v-row>
            <v-col cols="12" md="6">
              <v-label class="mb-2 font-weight-medium">Produto</v-label>
              <v-autocomplete
                v-model="produtoSelecionadoId"
                :items="produtosDaFabrica"
                :item-title="(p: ProdutoLista) => `${p.codigo} — ${p.descricao}`"
                item-value="id"
                placeholder="Busque por código ou descrição"
                variant="outlined"
                density="comfortable"
                hide-details
                clearable
              />
            </v-col>
            <v-col cols="12" md="3">
              <v-label class="mb-2 font-weight-medium">Cor</v-label>
              <v-select
                v-model="corEntradaId"
                :items="produtoAtual?.cores ?? []"
                item-title="nome"
                item-value="id"
                :disabled="!produtoAtual"
                placeholder="Cor"
                variant="outlined"
                density="comfortable"
                hide-details
              />
            </v-col>
            <v-col cols="12" md="3">
              <v-label class="mb-2 font-weight-medium">Valor unitário</v-label>
              <v-text-field
                v-model.number="valorEntrada"
                type="number"
                min="0"
                step="0.01"
                prefix="R$"
                :disabled="!produtoAtual"
                variant="outlined"
                density="comfortable"
                hide-details
              />
            </v-col>
          </v-row>

          <div v-if="produtoAtual" class="mt-4">
            <v-label class="mb-2 font-weight-medium">Grade (quantidade por tamanho)</v-label>
            <div class="d-flex flex-wrap ga-3">
              <div v-for="g in gradeEntrada" :key="g.tamanhoId" style="width: 90px">
                <div class="text-center text-body-2 font-weight-medium mb-1">{{ g.codigoTamanho }}</div>
                <v-text-field
                  v-model.number="g.quantidade"
                  type="number"
                  min="0"
                  density="compact"
                  variant="outlined"
                  hide-details
                  class="text-center"
                />
              </div>
            </div>

            <div class="d-flex align-center justify-space-between mt-4 flex-wrap ga-3">
              <span class="text-body-2 text-medium-emphasis">
                Total do item: <strong>{{ totalEntrada }}</strong> peças ·
                {{ moeda(totalEntrada * (valorEntrada || 0)) }}
              </span>
              <v-btn color="success" variant="flat" @click="adicionarItem">
                <v-icon icon="mdi-plus" start /> Adicionar ao pedido
              </v-btn>
            </div>
          </div>
        </template>
      </v-card-text>
    </v-card>

    <!-- ==================== itens do pedido ==================== -->
    <v-card elevation="0" rounded="lg">
      <v-card-item class="px-6 py-4">
        <div class="d-flex align-center justify-space-between">
          <h5 class="text-h5 font-weight-medium">Itens do Pedido</h5>
          <span class="text-body-2 text-medium-emphasis">{{ itens.length }} item(ns)</span>
        </div>
      </v-card-item>
      <v-divider />
      <v-card-text class="pa-6">
        <ComumEstadoVazio
          v-if="!itens.length"
          icone="mdi-cart-outline"
          titulo="Nenhum item adicionado"
          descricao="Use a seção acima para incluir produtos com a grade."
        />

        <v-table v-else>
          <thead>
            <tr>
              <th class="text-left">Código</th>
              <th class="text-left">Descrição</th>
              <th class="text-left">Cor</th>
              <th class="text-left">Grade</th>
              <th class="text-center">Qtd</th>
              <th class="text-right">Unitário</th>
              <th class="text-right">Subtotal</th>
              <th class="text-center">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, i) in itens" :key="i">
              <td class="font-weight-medium">{{ item.codigoProduto }}</td>
              <td>{{ item.descricaoProduto }}</td>
              <td>{{ item.nomeCor }}</td>
              <td class="text-body-2" style="font-family: monospace">{{ resumoGrade(item) }}</td>
              <td class="text-center">{{ quantidadeItem(item) }}</td>
              <td class="text-right">{{ moeda(item.valorUnitario) }}</td>
              <td class="text-right">{{ moeda(subtotalItem(item)) }}</td>
              <td class="text-center">
                <v-btn icon variant="text" size="small" color="error" @click="removerItem(i)">
                  <v-icon icon="mdi-trash-can-outline" size="20" />
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>

        <!-- ======================= totais ======================= -->
        <v-divider class="my-4" />
        <div class="d-flex justify-end">
          <div style="min-width: 320px">
            <div class="d-flex justify-space-between py-1">
              <span class="text-medium-emphasis">Total de peças</span>
              <span class="font-weight-medium">{{ totalPecas }}</span>
            </div>
            <div class="d-flex justify-space-between py-1">
              <span class="text-medium-emphasis">Valor bruto</span>
              <span class="font-weight-medium">{{ moeda(valorBruto) }}</span>
            </div>
            <div class="d-flex justify-space-between py-1">
              <span class="text-medium-emphasis">Desconto ({{ percentualDesconto || 0 }}%)</span>
              <span class="font-weight-medium text-error">− {{ moeda(desconto) }}</span>
            </div>
            <div class="d-flex justify-space-between py-1">
              <span class="text-medium-emphasis">
                Comissão ({{ percentual(percentualComissao) }})
              </span>
              <span class="font-weight-medium">{{ moeda(valorComissao) }}</span>
            </div>
            <v-divider class="my-2" />
            <div class="d-flex justify-space-between py-1">
              <span class="text-h6 font-weight-semibold">Valor líquido</span>
              <span class="text-h6 font-weight-semibold text-primary">{{ moeda(valorLiquido) }}</span>
            </div>
          </div>
        </div>

        <div class="d-flex justify-space-between mt-6 flex-wrap ga-3">
          <v-btn variant="tonal" color="error" to="/pedidos">Cancelar</v-btn>
          <v-btn color="success" variant="flat" size="large" :loading="salvando" @click="submeter">
            {{ valorInicial ? 'Salvar pedido' : 'Finalizar pedido' }}
          </v-btn>
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>
