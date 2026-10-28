<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useClientesApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Cliente, ClienteForm } from '@/types/api';

const rota = useRoute();
const api = useClientesApi();
const notificar = useNotificacaoStore();

const cliente = ref<Cliente | null>(null);
const carregando = ref(true);
const salvando = ref(false);

useHead({ title: 'Editar cliente' });

onMounted(async () => {
  try {
    cliente.value = await api.obter(Number(rota.params.id));
  } catch (e) {
    notificar.erro('Cliente não encontrado', mensagemDeErro(e));
    await navigateTo('/clientes');
  } finally {
    carregando.value = false;
  }
});

async function salvar(dados: ClienteForm) {
  salvando.value = true;
  try {
    await api.atualizar(Number(rota.params.id), dados);
    notificar.sucesso('Cliente atualizado', dados.nome);
    await navigateTo(`/clientes/${rota.params.id}`);
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
      titulo="Edição de Cliente"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Cliente', para: '/clientes' },
        { texto: cliente?.nome ?? '...' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/clientes">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <ClientesFormularioCliente
      v-else
      :valor-inicial="cliente"
      :salvando="salvando"
      @salvar="salvar"
    />
  </div>
</template>
