<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useProdutosApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Produto, ProdutoForm } from '@/types/api';

const rota = useRoute();
const api = useProdutosApi();
const notificar = useNotificacaoStore();

const produto = ref<Produto | null>(null);
const carregando = ref(true);
const salvando = ref(false);

useHead({ title: 'Editar produto' });

onMounted(async () => {
  try {
    produto.value = await api.obter(Number(rota.params.id));
  } catch (e) {
    notificar.erro('Produto não encontrado', mensagemDeErro(e));
    await navigateTo('/produtos');
  } finally {
    carregando.value = false;
  }
});

async function salvar(dados: ProdutoForm) {
  salvando.value = true;
  try {
    await api.atualizar(Number(rota.params.id), dados);
    notificar.sucesso('Produto atualizado', dados.descricao);
    await navigateTo(`/produtos/${rota.params.id}`);
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
      titulo="Editar Produto"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Produto', para: '/produtos' },
        { texto: produto?.descricao ?? '...' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/produtos">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <ProdutosFormularioProduto v-else :valor-inicial="produto" :salvando="salvando" @salvar="salvar" />
  </div>
</template>
