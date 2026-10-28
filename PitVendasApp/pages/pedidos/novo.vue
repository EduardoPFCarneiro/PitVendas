<script setup lang="ts">
import { ref } from 'vue';
import { usePedidosApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { PedidoForm } from '@/types/api';

useHead({ title: 'Emitir pedido' });

const api = usePedidosApi();
const notificar = useNotificacaoStore();
const salvando = ref(false);

async function salvar(dados: PedidoForm) {
  salvando.value = true;
  try {
    const criado = await api.criar(dados);
    notificar.sucesso('Pedido emitido', `Pedido nº ${criado.numero} criado com sucesso.`);
    await navigateTo(`/pedidos/${criado.id}`);
  } catch (e) {
    notificar.erro('Não foi possível emitir', mensagemDeErro(e));
  } finally {
    salvando.value = false;
  }
}
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Emitir Pedido"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Pedido', para: '/pedidos' },
        { texto: 'Emitir Pedido' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/pedidos">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <PedidosFormularioPedido :salvando="salvando" @salvar="salvar" />
  </div>
</template>
