<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFabricasApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Fabrica, FabricaForm } from '@/types/api';

const rota = useRoute();
const api = useFabricasApi();
const notificar = useNotificacaoStore();

const fabrica = ref<Fabrica | null>(null);
const carregando = ref(true);
const salvando = ref(false);

useHead({ title: 'Editar fábrica' });

onMounted(async () => {
  try {
    fabrica.value = await api.obter(Number(rota.params.id));
  } catch (e) {
    notificar.erro('Fábrica não encontrada', mensagemDeErro(e));
    await navigateTo('/fabricas');
  } finally {
    carregando.value = false;
  }
});

async function salvar(dados: FabricaForm) {
  salvando.value = true;
  try {
    await api.atualizar(Number(rota.params.id), dados);
    notificar.sucesso('Fábrica atualizada', dados.nome);
    await navigateTo(`/fabricas/${rota.params.id}`);
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
      titulo="Edição de Fábrica"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Fábrica', para: '/fabricas' },
        { texto: fabrica?.nome ?? '...' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/fabricas">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <FabricasFormularioFabrica v-else :valor-inicial="fabrica" :salvando="salvando" @salvar="salvar" />
  </div>
</template>
