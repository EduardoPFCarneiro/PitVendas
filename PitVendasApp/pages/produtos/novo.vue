<script setup lang="ts">
import { ref } from 'vue';
import { useProdutosApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { ProdutoForm, PrecoForm } from '@/types/api';

useHead({ title: 'Novo produto' });

const api = useProdutosApi();
const notificar = useNotificacaoStore();
const salvando = ref(false);
const formularioRef = ref<{ precos: PrecoForm[] } | null>(null);

async function salvar(dados: ProdutoForm) {
  salvando.value = true;
  try {
    const criado = await api.criar(dados);

    const precos = formularioRef.value?.precos ?? [];
    for (const preco of precos) {
      try {
        await api.adicionarPreco(criado.id, preco);
      } catch {
        notificar.aviso('Preço não salvo', `A tabela ${preco.codigo} precisa ser recadastrada.`);
      }
    }

    notificar.sucesso('Produto cadastrado', criado.descricao);
    await navigateTo('/produtos');
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
      titulo="Cadastrar Produto"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Produto', para: '/produtos' },
        { texto: 'Cadastrar' },
      ]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/produtos">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <ProdutosFormularioProduto ref="formularioRef" :salvando="salvando" @salvar="salvar" />
  </div>
</template>
