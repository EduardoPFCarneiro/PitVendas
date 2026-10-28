<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useDashboardApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Dashboard } from '@/types/api';

useHead({ title: 'Página Inicial' });

const api = useDashboardApi();
const { moeda, numero } = useFormato();
const notificar = useNotificacaoStore();

const dados = ref<Dashboard | null>(null);
const carregando = ref(true);

const MESES = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];

const graficoRanking = computed(() => ({
  series: dados.value?.rankingFabricas.map((f) => f.quantidadePedidos) ?? [],
  options: {
    chart: { type: 'donut', fontFamily: 'inherit' },
    labels: dados.value?.rankingFabricas.map((f) => f.nome) ?? [],
    colors: ['#0d86d9', '#4ce0a5', '#f8c076', '#fb977d', '#8763da'],
    legend: { position: 'bottom' },
    dataLabels: { enabled: false },
    plotOptions: {
      pie: {
        donut: {
          size: '70%',
          labels: {
            show: true,
            total: {
              show: true,
              label: 'Pedidos',
              formatter: () =>
                String(dados.value?.rankingFabricas.reduce((s, f) => s + f.quantidadePedidos, 0) ?? 0),
            },
          },
        },
      },
    },
  },
}));

const graficoPedidos = computed(() => ({
  series: [
    {
      name: 'Pedidos',
      data: dados.value?.pedidosPorMes.map((m) => m.quantidade) ?? [],
    },
  ],
  options: {
    chart: { type: 'bar', fontFamily: 'inherit', toolbar: { show: false } },
    colors: ['#0d86d9'],
    plotOptions: { bar: { borderRadius: 4, columnWidth: '45%' } },
    dataLabels: { enabled: false },
    xaxis: { categories: MESES },
    grid: { borderColor: 'rgba(0,0,0,0.06)' },
  },
}));

const indicadores = computed(() => [
  { titulo: 'Pedidos no mês', valor: numero(dados.value?.pedidosNoMes ?? 0), icone: 'mdi-clipboard-text', cor: 'primary' },
  { titulo: 'Comissão no mês', valor: moeda(dados.value?.comissaoNoMes ?? 0), icone: 'mdi-cash-multiple', cor: 'success' },
  { titulo: 'Comissão no ano', valor: moeda(dados.value?.comissaoNoAno ?? 0), icone: 'mdi-chart-line', cor: 'info' },
  { titulo: 'Clientes ativos', valor: numero(dados.value?.totalClientes ?? 0), icone: 'mdi-account-group', cor: 'warning' },
]);

onMounted(async () => {
  try {
    dados.value = await api.obter();
  } catch (e) {
    notificar.erro('Erro ao carregar o painel', mensagemDeErro(e));
  } finally {
    carregando.value = false;
  }
});
</script>

<template>
  <div>
    <ComumCabecalhoPagina titulo="Página Inicial" :trilha="[{ texto: 'Início' }]" />

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <template v-else-if="dados">

      <v-row class="mb-2">
        <v-col v-for="ind in indicadores" :key="ind.titulo" cols="12" sm="6" lg="3">
          <v-card elevation="0" rounded="lg">
            <v-card-text class="pa-5 d-flex align-center ga-4">
              <v-avatar :color="`light${ind.cor}`" size="56" rounded="lg">
                <v-icon :icon="ind.icone" :color="ind.cor" size="28" />
              </v-avatar>
              <div>
                <div class="text-body-2 text-medium-emphasis">{{ ind.titulo }}</div>
                <div class="text-h5 font-weight-semibold">{{ ind.valor }}</div>
              </div>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <v-row>

        <v-col cols="12" md="5">
          <v-card elevation="0" rounded="lg" height="100%">
            <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Ranking de Fábricas</h5></v-card-item>
            <v-divider />
            <v-card-text class="pa-6">
              <ComumEstadoVazio
                v-if="!dados.rankingFabricas.length"
                icone="mdi-factory"
                titulo="Sem pedidos ainda"
                descricao="O ranking aparece assim que você emitir pedidos."
              />
              <ClientOnly v-else>
                <apexchart type="donut" height="300" :options="graficoRanking.options" :series="graficoRanking.series" />
              </ClientOnly>
            </v-card-text>
          </v-card>
        </v-col>

        <v-col cols="12" md="7">
          <v-card elevation="0" rounded="lg" height="100%">
            <v-card-item class="px-6 py-4">
              <h5 class="text-h5 font-weight-medium">Pedidos por Mês</h5>
              <div class="text-body-2 text-medium-emphasis">Pedidos lançados no ano corrente</div>
            </v-card-item>
            <v-divider />
            <v-card-text class="pa-6">
              <ClientOnly>
                <apexchart type="bar" height="300" :options="graficoPedidos.options" :series="graficoPedidos.series" />
              </ClientOnly>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>

      <v-card v-if="dados.rankingFabricas.length" elevation="0" rounded="lg" class="mt-2">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Detalhe por Fábrica</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-table>
            <thead>
              <tr>
                <th class="text-left">Fábrica</th>
                <th class="text-center">Pedidos</th>
                <th class="text-right">Valor total</th>
                <th class="text-right">Comissão</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="f in dados.rankingFabricas" :key="f.fabricaId">
                <td class="font-weight-medium">{{ f.nome }}</td>
                <td class="text-center">{{ f.quantidadePedidos }}</td>
                <td class="text-right">{{ moeda(f.valorTotal) }}</td>
                <td class="text-right">{{ moeda(f.comissao) }}</td>
              </tr>
            </tbody>
          </v-table>
        </v-card-text>
      </v-card>
    </template>
  </div>
</template>
