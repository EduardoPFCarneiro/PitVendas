<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { usePedidosApi, useRelatoriosApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useConfirmacao } from '@/composables/useConfirmacao';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import { PedidoStatus, PEDIDO_STATUS_LABEL, PEDIDO_STATUS_COR } from '@/types/api';
import type { Pedido, PedidoItem } from '@/types/api';

const rota = useRoute();
const api = usePedidosApi();
const relatoriosApi = useRelatoriosApi();
const { moeda, data: fmtData, percentual } = useFormato();
const { confirmar } = useConfirmacao();
const notificar = useNotificacaoStore();

const pedido = ref<Pedido | null>(null);
const carregando = ref(true);
const baixandoPdf = ref(false);
const dialogoEnvio = ref(false);

async function baixarPdf() {
  baixandoPdf.value = true;
  try {
    await relatoriosApi.pedidoPdf(Number(rota.params.id));
  } catch (e) {
    notificar.erro('Não foi possível gerar o PDF', mensagemDeErro(e));
  } finally {
    baixandoPdf.value = false;
  }
}

useHead({ title: 'Pedido' });

const proximosStatus = computed(() => {
  if (!pedido.value) return [];
  switch (pedido.value.status) {
    case PedidoStatus.Rascunho: return [PedidoStatus.Enviado];
    case PedidoStatus.Enviado: return [PedidoStatus.Confirmado, PedidoStatus.Cancelado];
    case PedidoStatus.Confirmado: return [PedidoStatus.Faturado, PedidoStatus.Cancelado];
    default: return [];
  }
});

function resumoGrade(item: PedidoItem) {
  return item.grade.map((g) => `${g.codigoTamanho}:${g.quantidade}`).join('  ');
}

async function carregar() {
  try {
    pedido.value = await api.obter(Number(rota.params.id));
  } catch (e) {
    notificar.erro('Pedido não encontrado', mensagemDeErro(e));
    await navigateTo('/pedidos');
  } finally {
    carregando.value = false;
  }
}

async function alterarStatus(novo: PedidoStatus) {
  const rotulo = PEDIDO_STATUS_LABEL[novo];
  const ok = await confirmar({
    titulo: `Marcar como ${rotulo}?`,
    mensagem:
      novo === PedidoStatus.Cancelado
        ? 'O pedido será cancelado e não poderá mais mudar de status.'
        : `O pedido passará para a situação "${rotulo}".`,
    textoConfirmar: rotulo,
    cor: novo === PedidoStatus.Cancelado ? 'error' : 'primary',
  });
  if (!ok) return;

  try {
    pedido.value = await api.alterarStatus(Number(rota.params.id), novo);
    notificar.sucesso('Status atualizado', `Pedido nº ${pedido.value.numero}: ${rotulo}`);
  } catch (e) {
    notificar.erro('Não foi possível alterar', mensagemDeErro(e));
  }
}

onMounted(carregar);
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Visualização do Pedido"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Pedido', para: '/pedidos' },
        { texto: pedido ? `Nº ${String(pedido.numero).padStart(2, '0')}` : '...' },
      ]"
    >
      <template #acoes>
        <v-btn v-if="pedido" variant="tonal" color="error" :loading="baixandoPdf" @click="baixarPdf">
          <v-icon icon="mdi-file-pdf-box" start /> PDF
        </v-btn>
        <v-btn v-if="pedido" variant="tonal" color="success" @click="dialogoEnvio = true">
          <v-icon icon="mdi-send" start /> Enviar
        </v-btn>
        <v-btn
          v-if="pedido && (pedido.status === PedidoStatus.Rascunho || pedido.status === PedidoStatus.Enviado)"
          color="secondary"
          variant="tonal"
          :to="`/pedidos/${pedido.id}/editar`"
        >
          Editar
        </v-btn>
        <v-btn color="primary" variant="flat" to="/pedidos">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <PedidosDialogoEnvio v-if="pedido" v-model="dialogoEnvio" :pedido="pedido" />

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <template v-else-if="pedido">

      <v-card elevation="0" rounded="lg" class="mb-6">
        <v-card-text class="pa-6 d-flex align-center justify-space-between flex-wrap ga-3">
          <div class="d-flex align-center ga-3">
            <span class="text-body-1 text-medium-emphasis">Situação:</span>
            <v-chip :color="PEDIDO_STATUS_COR[pedido.status]" variant="tonal" size="large">
              {{ pedido.statusDescricao }}
            </v-chip>
          </div>
          <div class="d-flex ga-2">
            <v-btn
              v-for="s in proximosStatus"
              :key="s"
              :color="s === PedidoStatus.Cancelado ? 'error' : 'primary'"
              :variant="s === PedidoStatus.Cancelado ? 'tonal' : 'flat'"
              @click="alterarStatus(s)"
            >
              {{ PEDIDO_STATUS_LABEL[s] }}
            </v-btn>
          </div>
        </v-card-text>
      </v-card>

      <v-card elevation="0" rounded="lg" class="mb-6">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Informações do Pedido</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-row>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Número</div><div class="text-body-1 font-weight-medium">{{ String(pedido.numero).padStart(2, '0') }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Data</div><div class="text-body-1 font-weight-medium">{{ fmtData(pedido.data) }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Faturamento</div><div class="text-body-1 font-weight-medium">{{ pedido.dataFaturamento ? fmtData(pedido.dataFaturamento) : '—' }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Tabela</div><div class="text-body-1 font-weight-medium">{{ pedido.tabelaPreco || '—' }}</div></v-col>

            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Cliente</div><div class="text-body-1 font-weight-medium">{{ pedido.clienteNome }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Fábrica</div><div class="text-body-1 font-weight-medium">{{ pedido.fabricaNome }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Condição</div><div class="text-body-1 font-weight-medium">{{ pedido.condicaoPagamento || '—' }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Parcelas</div><div class="text-body-1 font-weight-medium">{{ pedido.parcelas }}</div></v-col>

            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Prazo</div><div class="text-body-1 font-weight-medium">{{ pedido.prazo || '—' }}</div></v-col>
            <v-col cols="12" md="3"><div class="text-body-2 text-medium-emphasis">Frete</div><div class="text-body-1 font-weight-medium">{{ pedido.frete || '—' }}</div></v-col>
            <v-col v-if="pedido.observacao" cols="12" md="6"><div class="text-body-2 text-medium-emphasis">Observação</div><div class="text-body-1">{{ pedido.observacao }}</div></v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card elevation="0" rounded="lg">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Itens</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-table>
            <thead>
              <tr>
                <th class="text-left">Código</th>
                <th class="text-left">Descrição</th>
                <th class="text-left">Cor</th>
                <th class="text-left">Grade</th>
                <th class="text-center">Qtd</th>
                <th class="text-right">Unitário</th>
                <th class="text-right">Subtotal</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="item in pedido.itens" :key="item.id">
                <td class="font-weight-medium">{{ item.codigoProduto }}</td>
                <td>{{ item.descricaoProduto }}</td>
                <td>{{ item.nomeCor }}</td>
                <td class="text-body-2" style="font-family: monospace">{{ resumoGrade(item) }}</td>
                <td class="text-center">{{ item.quantidade }}</td>
                <td class="text-right">{{ moeda(item.valorUnitario) }}</td>
                <td class="text-right">{{ moeda(item.subtotal) }}</td>
              </tr>
            </tbody>
          </v-table>

          <v-divider class="my-4" />
          <div class="d-flex justify-end">
            <div style="min-width: 320px">
              <div class="d-flex justify-space-between py-1"><span class="text-medium-emphasis">Total de peças</span><span class="font-weight-medium">{{ pedido.totalPecas }}</span></div>
              <div class="d-flex justify-space-between py-1"><span class="text-medium-emphasis">Valor bruto</span><span class="font-weight-medium">{{ moeda(pedido.valorBruto) }}</span></div>
              <div class="d-flex justify-space-between py-1"><span class="text-medium-emphasis">Desconto ({{ pedido.percentualDesconto }}%)</span><span class="font-weight-medium text-error">− {{ moeda(pedido.desconto) }}</span></div>
              <div class="d-flex justify-space-between py-1"><span class="text-medium-emphasis">Comissão ({{ percentual(pedido.percentualComissao) }})</span><span class="font-weight-medium">{{ moeda(pedido.valorComissao) }}</span></div>
              <v-divider class="my-2" />
              <div class="d-flex justify-space-between py-1"><span class="text-h6 font-weight-semibold">Valor líquido</span><span class="text-h6 font-weight-semibold text-primary">{{ moeda(pedido.valorTotal) }}</span></div>
            </div>
          </div>
        </v-card-text>
      </v-card>
    </template>
  </div>
</template>
