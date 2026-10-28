<script setup lang="ts">
import { ref, watch } from 'vue';
import { usePedidosApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useConfirmacao } from '@/composables/useConfirmacao';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import { PedidoStatus, PEDIDO_STATUS_LABEL, PEDIDO_STATUS_COR } from '@/types/api';
import type { PedidoLista } from '@/types/api';

useHead({ title: 'Pedidos' });

const api = usePedidosApi();
const { moeda, data: fmtData, numero } = useFormato();
const { confirmar } = useConfirmacao();
const notificar = useNotificacaoStore();

const itens = ref<PedidoLista[]>([]);
const total = ref(0);
const pagina = ref(1);
const tamanhoPagina = ref(10);
const busca = ref('');
const status = ref<PedidoStatus | null>(null);
const carregando = ref(false);

const opcoesStatus = Object.entries(PEDIDO_STATUS_LABEL).map(([valor, texto]) => ({
  value: Number(valor) as PedidoStatus,
  title: texto,
}));

async function carregar() {
  carregando.value = true;
  try {
    const r = await api.listar({
      busca: busca.value || undefined,
      status: status.value ?? undefined,
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    });
    itens.value = r.itens;
    total.value = r.total;
  } catch (e) {
    notificar.erro('Erro ao carregar pedidos', mensagemDeErro(e));
  } finally {
    carregando.value = false;
  }
}

let debounce: ReturnType<typeof setTimeout>;
watch(busca, () => {
  clearTimeout(debounce);
  debounce = setTimeout(() => {
    pagina.value = 1;
    carregar();
  }, 400);
});
watch([pagina, status], carregar);

async function duplicar(pedido: PedidoLista) {
  try {
    const novo = await api.duplicar(pedido.id);
    notificar.sucesso('Pedido repetido', `Gerado o pedido nº ${novo.numero} como rascunho.`);
    await navigateTo(`/pedidos/${novo.id}/editar`);
  } catch (e) {
    notificar.erro('Não foi possível repetir', mensagemDeErro(e));
  }
}

async function excluir(pedido: PedidoLista) {
  const ok = await confirmar({
    titulo: 'Excluir rascunho?',
    mensagem: `O pedido nº ${pedido.numero} será removido definitivamente.`,
    textoConfirmar: 'Excluir',
    cor: 'error',
  });
  if (!ok) return;
  try {
    await api.excluir(pedido.id);
    notificar.sucesso('Rascunho excluído', `Pedido nº ${pedido.numero}`);
    carregar();
  } catch (e) {
    notificar.erro('Não foi possível excluir', mensagemDeErro(e));
  }
}

onMounted(carregar);
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Listagem de Pedidos"
      :trilha="[{ texto: 'Início', para: '/' }, { texto: 'Pedido' }]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/pedidos/novo">Novo Pedido</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-card elevation="0" rounded="lg">
      <v-card-text class="pa-6">
        <div class="d-flex justify-center flex-wrap ga-3 mb-5">
          <v-text-field
            v-model="busca"
            placeholder="Pesquisar por cliente ou fábrica"
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            style="max-width: 360px"
          />
          <v-select
            v-model="status"
            :items="opcoesStatus"
            placeholder="Todos os status"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            style="max-width: 220px"
          />
        </div>

        <v-progress-linear v-if="carregando" indeterminate color="primary" class="mb-2" />

        <ComumEstadoVazio
          v-if="!carregando && !itens.length"
          icone="mdi-clipboard-text-outline"
          :titulo="busca || status != null ? 'Nenhum pedido encontrado' : 'Nenhum pedido lançado'"
          :descricao="
            busca || status != null
              ? 'Ajuste os filtros de busca.'
              : 'Emita seu primeiro pedido para começar a registrar vendas.'
          "
          :texto-acao="busca || status != null ? undefined : 'Novo pedido'"
          @acao="navigateTo('/pedidos/novo')"
        />

        <v-table v-else-if="!carregando" hover>
          <thead>
            <tr>
              <th class="text-left">Nº</th>
              <th class="text-left">Cliente</th>
              <th class="text-left">Fábrica</th>
              <th class="text-left">Data</th>
              <th class="text-right">Valor</th>
              <th class="text-center">Peças</th>
              <th class="text-center">Status</th>
              <th class="text-center">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in itens" :key="p.id">
              <td class="font-weight-medium">{{ String(p.numero).padStart(2, '0') }}</td>
              <td>{{ p.clienteNome }}</td>
              <td>{{ p.fabricaNome }}</td>
              <td>{{ fmtData(p.data) }}</td>
              <td class="text-right">{{ moeda(p.valorTotal) }}</td>
              <td class="text-center">{{ numero(p.totalPecas) }}</td>
              <td class="text-center">
                <v-chip :color="PEDIDO_STATUS_COR[p.status]" size="small" variant="tonal">
                  {{ p.statusDescricao }}
                </v-chip>
              </td>
              <td class="text-center text-no-wrap">
                <v-btn icon variant="text" size="small" :to="`/pedidos/${p.id}`" title="Visualizar">
                  <v-icon icon="mdi-eye-outline" size="20" />
                </v-btn>
                <v-btn icon variant="text" size="small" title="Repetir pedido" @click="duplicar(p)">
                  <v-icon icon="mdi-content-copy" size="18" />
                </v-btn>
                <v-btn
                  v-if="p.status === PedidoStatus.Rascunho || p.status === PedidoStatus.Enviado"
                  icon
                  variant="text"
                  size="small"
                  :to="`/pedidos/${p.id}/editar`"
                  title="Editar"
                >
                  <v-icon icon="mdi-pencil-outline" size="20" />
                </v-btn>
                <v-btn
                  v-if="p.status === PedidoStatus.Rascunho"
                  icon
                  variant="text"
                  size="small"
                  title="Excluir"
                  @click="excluir(p)"
                >
                  <v-icon icon="mdi-trash-can-outline" size="20" />
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>

        <div v-if="total > tamanhoPagina" class="d-flex justify-center mt-6">
          <v-pagination v-model="pagina" :length="Math.ceil(total / tamanhoPagina)" :total-visible="7" density="comfortable" rounded="circle" />
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>
