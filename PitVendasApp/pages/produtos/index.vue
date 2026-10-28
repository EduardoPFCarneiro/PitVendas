<script setup lang="ts">
import { ref, watch } from 'vue';
import { useProdutosApi, useFabricasApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useConfirmacao } from '@/composables/useConfirmacao';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { ProdutoLista, FabricaLista } from '@/types/api';

useHead({ title: 'Produtos' });

const api = useProdutosApi();
const fabricasApi = useFabricasApi();
const { moeda } = useFormato();
const { confirmar } = useConfirmacao();
const notificar = useNotificacaoStore();

const itens = ref<ProdutoLista[]>([]);
const total = ref(0);
const pagina = ref(1);
const tamanhoPagina = ref(10);
const busca = ref('');
const fabricaId = ref<number | null>(null);
const fabricas = ref<FabricaLista[]>([]);
const carregando = ref(false);

async function carregar() {
  carregando.value = true;
  try {
    const r = await api.listar({
      busca: busca.value || undefined,
      fabricaId: fabricaId.value ?? undefined,
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    });
    itens.value = r.itens;
    total.value = r.total;
  } catch (e) {
    notificar.erro('Erro ao carregar produtos', mensagemDeErro(e));
  } finally {
    carregando.value = false;
  }
}

let debounce: ReturnType<typeof setTimeout>;
watch(busca, () => {
  clearTimeout(debounce);
  debounce = setTimeout(() => {
    pagina.value = 1;
    carregar();
  }, 400);
});
watch([pagina, fabricaId], carregar);

async function inativar(produto: ProdutoLista) {
  const ok = await confirmar({
    titulo: 'Inativar produto?',
    mensagem: `${produto.descricao} não poderá mais ser incluído em novos pedidos.`,
    textoConfirmar: 'Inativar',
    cor: 'error',
  });
  if (!ok) return;
  try {
    await api.inativar(produto.id);
    notificar.sucesso('Produto inativado', produto.descricao);
    carregar();
  } catch (e) {
    notificar.erro('Não foi possível inativar', mensagemDeErro(e));
  }
}

onMounted(async () => {
  await carregar();
  const r = await fabricasApi.listar({ ativo: true, tamanhoPagina: 100 });
  fabricas.value = r.itens;
});
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Listagem de Produtos"
      :trilha="[{ texto: 'Início', para: '/' }, { texto: 'Produto' }]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/produtos/novo">Cadastrar</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-card elevation="0" rounded="lg">
      <v-card-text class="pa-6">
        <div class="d-flex justify-center flex-wrap ga-3 mb-5">
          <v-text-field
            v-model="busca"
            placeholder="Pesquisar por código ou descrição"
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            style="max-width: 360px"
          />
          <v-select
            v-model="fabricaId"
            :items="fabricas"
            item-title="nome"
            item-value="id"
            placeholder="Todas as fábricas"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            style="max-width: 260px"
          />
        </div>

        <v-progress-linear v-if="carregando" indeterminate color="primary" class="mb-2" />

        <ComumEstadoVazio
          v-if="!carregando && !itens.length"
          icone="mdi-package-variant-closed"
          :titulo="busca || fabricaId ? 'Nenhum produto encontrado' : 'Nenhum produto cadastrado'"
          :descricao="
            busca || fabricaId
              ? 'Ajuste os filtros de busca.'
              : 'Cadastre os produtos das suas fábricas para montar pedidos.'
          "
          :texto-acao="busca || fabricaId ? undefined : 'Cadastrar produto'"
          @acao="navigateTo('/produtos/novo')"
        />

        <v-table v-else-if="!carregando" hover>
          <thead>
            <tr>
              <th class="text-left">Código</th>
              <th class="text-left">Descrição</th>
              <th class="text-left">Fábrica</th>
              <th class="text-right">Menor preço</th>
              <th class="text-center">Situação</th>
              <th class="text-center">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in itens" :key="p.id">
              <td class="font-weight-medium">{{ p.codigo }}</td>
              <td>{{ p.descricao }}</td>
              <td>{{ p.fabricaNome }}</td>
              <td class="text-right">{{ p.menorPreco != null ? moeda(p.menorPreco) : '—' }}</td>
              <td class="text-center">
                <v-chip :color="p.ativo ? 'success' : 'secondary'" size="small" variant="tonal">
                  {{ p.ativo ? 'Ativo' : 'Inativo' }}
                </v-chip>
              </td>
              <td class="text-center text-no-wrap">
                <v-btn icon variant="text" size="small" :to="`/produtos/${p.id}`" title="Visualizar">
                  <v-icon icon="mdi-eye-outline" size="20" />
                </v-btn>
                <v-btn icon variant="text" size="small" :to="`/produtos/${p.id}/editar`" title="Editar">
                  <v-icon icon="mdi-pencil-outline" size="20" />
                </v-btn>
                <v-btn v-if="p.ativo" icon variant="text" size="small" title="Inativar" @click="inativar(p)">
                  <v-icon icon="mdi-trash-can-outline" size="20" />
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>

        <div v-if="total > tamanhoPagina" class="d-flex justify-center mt-6">
          <v-pagination v-model="pagina" :length="Math.ceil(total / tamanhoPagina)" :total-visible="7" density="comfortable" rounded="circle" />
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>
