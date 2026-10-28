<script setup lang="ts">
import { ref, watch } from 'vue';
import { useFabricasApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useConfirmacao } from '@/composables/useConfirmacao';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { FabricaLista } from '@/types/api';

useHead({ title: 'Fábricas' });

const api = useFabricasApi();
const { cnpj, telefone, percentual } = useFormato();
const { confirmar } = useConfirmacao();
const notificar = useNotificacaoStore();

const itens = ref<FabricaLista[]>([]);
const total = ref(0);
const pagina = ref(1);
const tamanhoPagina = ref(10);
const busca = ref('');
const carregando = ref(false);

async function carregar() {
  carregando.value = true;
  try {
    const r = await api.listar({
      busca: busca.value || undefined,
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    });
    itens.value = r.itens;
    total.value = r.total;
  } catch (e) {
    notificar.erro('Erro ao carregar fábricas', mensagemDeErro(e));
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
watch(pagina, carregar);

async function inativar(fabrica: FabricaLista) {
  const ok = await confirmar({
    titulo: 'Inativar fábrica?',
    mensagem:
      `${fabrica.nome} deixará de aparecer para novos pedidos e produtos, ` +
      'mas o histórico permanece intacto.',
    textoConfirmar: 'Inativar',
    cor: 'error',
  });
  if (!ok) return;

  try {
    await api.inativar(fabrica.id);
    notificar.sucesso('Fábrica inativada', fabrica.nome);
    carregar();
  } catch (e) {
    notificar.erro('Não foi possível inativar', mensagemDeErro(e));
  }
}

onMounted(carregar);
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Listagem de Fábricas"
      :trilha="[{ texto: 'Início', para: '/' }, { texto: 'Fábrica' }]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/fabricas/novo">Cadastrar</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-card elevation="0" rounded="lg">
      <v-card-text class="pa-6">
        <div class="d-flex justify-center mb-5">
          <v-text-field
            v-model="busca"
            placeholder="Pesquisar por nome, razão social ou CNPJ"
            prepend-inner-icon="mdi-magnify"
            variant="outlined"
            density="comfortable"
            hide-details
            clearable
            style="max-width: 420px"
          />
        </div>

        <v-progress-linear v-if="carregando" indeterminate color="primary" class="mb-2" />

        <ComumEstadoVazio
          v-if="!carregando && !itens.length"
          icone="mdi-factory"
          :titulo="busca ? 'Nenhuma fábrica encontrada' : 'Nenhuma fábrica cadastrada'"
          :descricao="
            busca
              ? 'Tente outro termo de busca.'
              : 'Cadastre as fábricas que você representa para lançar produtos e pedidos.'
          "
          :texto-acao="busca ? undefined : 'Cadastrar fábrica'"
          @acao="navigateTo('/fabricas/novo')"
        />

        <v-table v-else-if="!carregando" hover>
          <thead>
            <tr>
              <th class="text-left">Razão Social</th>
              <th class="text-left">CNPJ</th>
              <th class="text-center">Comissão</th>
              <th class="text-left">Telefone</th>
              <th class="text-center">Situação</th>
              <th class="text-center">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="f in itens" :key="f.id">
              <td>
                <div class="font-weight-medium">{{ f.nome }}</div>
                <div class="text-caption text-medium-emphasis">{{ f.razaoSocial }}</div>
              </td>
              <td>{{ cnpj(f.cnpj) }}</td>
              <td class="text-center">{{ percentual(f.comissao) }}</td>
              <td>{{ telefone(f.telefone) || '—' }}</td>
              <td class="text-center">
                <v-chip :color="f.ativo ? 'success' : 'secondary'" size="small" variant="tonal">
                  {{ f.ativo ? 'Ativa' : 'Inativa' }}
                </v-chip>
              </td>
              <td class="text-center text-no-wrap">
                <v-btn icon variant="text" size="small" :to="`/fabricas/${f.id}`" title="Visualizar">
                  <v-icon icon="mdi-eye-outline" size="20" />
                </v-btn>
                <v-btn icon variant="text" size="small" :to="`/fabricas/${f.id}/editar`" title="Editar">
                  <v-icon icon="mdi-pencil-outline" size="20" />
                </v-btn>
                <v-btn v-if="f.ativo" icon variant="text" size="small" title="Inativar" @click="inativar(f)">
                  <v-icon icon="mdi-trash-can-outline" size="20" />
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>

        <div v-if="total > tamanhoPagina" class="d-flex justify-center mt-6">
          <v-pagination
            v-model="pagina"
            :length="Math.ceil(total / tamanhoPagina)"
            :total-visible="7"
            density="comfortable"
            rounded="circle"
          />
        </div>
      </v-card-text>
    </v-card>
  </div>
</template>
