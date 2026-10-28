<script setup lang="ts">
import { ref } from 'vue';
import { useFabricasApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { FabricaForm } from '@/types/api';

useHead({ title: 'Nova fábrica' });

const api = useFabricasApi();
const notificar = useNotificacaoStore();
const salvando = ref(false);

async function salvar(dados: FabricaForm) {
  salvando.value = true;
  try {
    const criada = await api.criar(dados);
    notificar.sucesso('Fábrica cadastrada', criada.nome);
    await navigateTo('/fabricas');
  } catch (e) {
    notificar.erro('Não foi possível cadastrar', mensagemDeErro(e));
  } finally {
    salvando.value = false;
  }
}
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Cadastro de Fábricas"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Fábrica', para: '/fabricas' },
        { texto: 'Cadastro' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/fabricas">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <FabricasFormularioFabrica :salvando="salvando" @salvar="salvar" />
  </div>
</template>
