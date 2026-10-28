<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useProdutosApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Produto } from '@/types/api';

const rota = useRoute();
const api = useProdutosApi();
const { moeda, data } = useFormato();
const notificar = useNotificacaoStore();

const produto = ref<Produto | null>(null);
const carregando = ref(true);

useHead({ title: 'Produto' });

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
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Visualização de Produto"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Produto', para: '/produtos' },
        { texto: produto?.descricao ?? '...' },
      ]"
    >
      <template #acoes>
        <v-btn v-if="produto" color="secondary" variant="tonal" :to="`/produtos/${produto.id}/editar`">Editar</v-btn>
        <v-btn color="primary" variant="flat" to="/produtos">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <template v-else-if="produto">
      <v-card elevation="0" rounded="lg" class="mb-6">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Dados Gerais</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-row>
            <v-col cols="12" md="3">
              <div class="text-body-2 text-medium-emphasis">Código</div>
              <div class="text-body-1 font-weight-medium">{{ produto.codigo }}</div>
            </v-col>
            <v-col cols="12" md="6">
              <div class="text-body-2 text-medium-emphasis">Descrição</div>
              <div class="text-body-1 font-weight-medium">{{ produto.descricao }}</div>
            </v-col>
            <v-col cols="12" md="3">
              <div class="text-body-2 text-medium-emphasis">Fábrica</div>
              <div class="text-body-1 font-weight-medium">{{ produto.fabricaNome }}</div>
            </v-col>
            <v-col cols="12" md="6">
              <div class="text-body-2 text-medium-emphasis mb-1">Cores</div>
              <v-chip v-for="c in produto.cores" :key="c.id" size="small" variant="tonal" color="primary" class="mr-1 mb-1">
                {{ c.nome }}
              </v-chip>
            </v-col>
            <v-col cols="12" md="6">
              <div class="text-body-2 text-medium-emphasis mb-1">Grade (tamanhos)</div>
              <v-chip v-for="t in produto.tamanhos" :key="t.id" size="small" variant="tonal" color="secondary" class="mr-1 mb-1">
                {{ t.nome }}
              </v-chip>
            </v-col>
            <v-col v-if="produto.observacao" cols="12">
              <div class="text-body-2 text-medium-emphasis">Observação</div>
              <div class="text-body-1">{{ produto.observacao }}</div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card elevation="0" rounded="lg">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Tabelas de Preço</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-table v-if="produto.precos.length">
            <thead>
              <tr>
                <th class="text-left">Código</th>
                <th class="text-right">Valor</th>
                <th class="text-left">Última alteração</th>
                <th class="text-center">Situação</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="p in produto.precos" :key="p.id">
                <td class="font-weight-medium">{{ p.codigo }}</td>
                <td class="text-right">{{ moeda(p.valor) }}</td>
                <td>{{ data(p.dataAlteracao) }}</td>
                <td class="text-center">
                  <v-chip :color="p.ativo ? 'success' : 'secondary'" size="small" variant="tonal">
                    {{ p.ativo ? 'Ativa' : 'Inativa' }}
                  </v-chip>
                </td>
              </tr>
            </tbody>
          </v-table>
          <p v-else class="text-medium-emphasis mb-0">Nenhuma tabela de preço cadastrada.</p>
        </v-card-text>
      </v-card>
    </template>
  </div>
</template>
