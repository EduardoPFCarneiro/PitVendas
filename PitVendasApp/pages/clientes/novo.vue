<script setup lang="ts">
import { ref } from 'vue';
import { useClientesApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { ClienteForm } from '@/types/api';

useHead({ title: 'Novo cliente' });

const api = useClientesApi();
const notificar = useNotificacaoStore();
const salvando = ref(false);

async function salvar(dados: ClienteForm) {
  salvando.value = true;
  try {
    const criado = await api.criar(dados);
    notificar.sucesso('Cliente cadastrado', criado.nome);
    await navigateTo('/clientes');
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
      titulo="Cadastro de Clientes"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Cliente', para: '/clientes' },
        { texto: 'Cadastro' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/clientes">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <ClientesFormularioCliente :salvando="salvando" @salvar="salvar" />
  </div>
</template>
