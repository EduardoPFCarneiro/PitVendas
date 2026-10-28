<script setup lang="ts">
import { ref } from 'vue';
import { useRelatoriosApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';

useHead({ title: 'Relatórios' });

const api = useRelatoriosApi();
const notificar = useNotificacaoStore();

const de = ref<string | null>(null);
const ate = ref<string | null>(null);
const baixando = ref<string | null>(null);

async function baixar(chave: string, acao: () => Promise<void>, rotulo: string) {
  baixando.value = chave;
  try {
    await acao();
    notificar.sucesso('Relatório gerado', rotulo);
  } catch (e) {
    notificar.erro('Não foi possível gerar', mensagemDeErro(e));
  } finally {
    baixando.value = null;
  }
}

function periodo() {
  return { de: de.value || undefined, ate: ate.value || undefined };
}

const relatorios = [
  {
    chave: 'clientes',
    titulo: 'Clientes',
    descricao: 'Lista completa de clientes em Excel.',
    icone: 'mdi-account-group',
    cor: 'primary',
    usaPeriodo: false,
    acao: () => api.clientesExcel(),
  },
  {
    chave: 'produtos',
    titulo: 'Produtos',
    descricao: 'Catálogo de produtos com o menor preço.',
    icone: 'mdi-package-variant-closed',
    cor: 'info',
    usaPeriodo: false,
    acao: () => api.produtosExcel(),
  },
  {
    chave: 'fabricas',
    titulo: 'Fábricas',
    descricao: 'Fábricas cadastradas e suas comissões.',
    icone: 'mdi-factory',
    cor: 'secondary',
    usaPeriodo: false,
    acao: () => api.fabricasExcel(),
  },
  {
    chave: 'pedidos',
    titulo: 'Pedidos',
    descricao: 'Pedidos do período, com valores e status.',
    icone: 'mdi-clipboard-text',
    cor: 'warning',
    usaPeriodo: true,
    acao: () => api.pedidosExcel(periodo()),
  },
  {
    chave: 'comissao',
    titulo: 'Comissão',
    descricao: 'Comissões consolidadas por fábrica no período.',
    icone: 'mdi-cash-multiple',
    cor: 'success',
    usaPeriodo: true,
    acao: () => api.comissaoExcel(periodo()),
  },
  {
    chave: 'faturamento',
    titulo: 'Faturamento',
    descricao: 'Faturamento por fábrica no período (mesma base da comissão).',
    icone: 'mdi-chart-line',
    cor: 'indigo',
    usaPeriodo: true,
    acao: () => api.comissaoExcel(periodo()),
  },
];
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Seleção de Relatórios"
      :trilha="[{ texto: 'Início', para: '/' }, { texto: 'Relatório' }]"
    />

    <v-card elevation="0" rounded="lg" class="mb-6">
      <v-card-text class="pa-6">
        <div class="d-flex align-center flex-wrap ga-4">
          <v-icon icon="mdi-calendar-range" color="primary" />
          <span class="text-body-1 font-weight-medium">Período</span>
          <v-text-field v-model="de" type="date" label="De" variant="outlined" density="comfortable" hide-details style="max-width: 200px" />
          <v-text-field v-model="ate" type="date" label="Até" variant="outlined" density="comfortable" hide-details style="max-width: 200px" />
          <span class="text-body-2 text-medium-emphasis">
            Aplica-se aos relatórios de Pedidos, Comissão e Faturamento. Em branco, considera todo o histórico.
          </span>
        </div>
      </v-card-text>
    </v-card>

    <v-row>
      <v-col v-for="r in relatorios" :key="r.chave" cols="12" sm="6" md="4">
        <v-card elevation="0" rounded="lg" height="100%">
          <v-card-text class="pa-6 d-flex flex-column" style="height: 100%">
            <v-avatar :color="`light${r.cor}`" size="56" rounded="lg" class="mb-4">
              <v-icon :icon="r.icone" :color="r.cor" size="28" />
            </v-avatar>
            <h5 class="text-h5 font-weight-medium mb-1">{{ r.titulo }}</h5>
            <p class="text-body-2 text-medium-emphasis mb-4 flex-grow-1">{{ r.descricao }}</p>
            <v-btn
              :color="r.cor"
              variant="flat"
              block
              :loading="baixando === r.chave"
              :disabled="baixando !== null"
              @click="baixar(r.chave, r.acao, r.titulo)"
            >
              <v-icon icon="mdi-microsoft-excel" start /> Baixar Excel
            </v-btn>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </div>
</template>
