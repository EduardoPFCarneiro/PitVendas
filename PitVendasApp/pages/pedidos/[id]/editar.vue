<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { usePedidosApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Pedido, PedidoForm } from '@/types/api';

const rota = useRoute();
const api = usePedidosApi();
const notificar = useNotificacaoStore();

const pedido = ref<Pedido | null>(null);
const carregando = ref(true);
const salvando = ref(false);

useHead({ title: 'Editar pedido' });

onMounted(async () => {
  try {
    pedido.value = await api.obter(Number(rota.params.id));
  } catch (e) {
    notificar.erro('Pedido não encontrado', mensagemDeErro(e));
    await navigateTo('/pedidos');
  } finally {
    carregando.value = false;
  }
});

async function salvar(dados: PedidoForm) {
  salvando.value = true;
  try {
    await api.atualizar(Number(rota.params.id), dados);
    notificar.sucesso('Pedido atualizado', `Pedido nº ${pedido.value?.numero}`);
    await navigateTo(`/pedidos/${rota.params.id}`);
  } catch (e) {
    notificar.erro('Não foi possível salvar', mensagemDeErro(e));
  } finally {
    salvando.value = false;
  }
}
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Editar Pedido"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Pedido', para: '/pedidos' },
        { texto: pedido ? `Nº ${pedido.numero}` : '...' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/pedidos">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <PedidosFormularioPedido v-else :valor-inicial="pedido" :salvando="salvando" @salvar="salvar" />
  </div>
</template>
