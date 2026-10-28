<script setup lang="ts">
import { ref, watch } from 'vue';
import { useClientesApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useConfirmacao } from '@/composables/useConfirmacao';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { ClienteLista } from '@/types/api';

useHead({ title: 'Clientes' });

const api = useClientesApi();
const { cnpj, telefone } = useFormato();
const { confirmar } = useConfirmacao();
const notificar = useNotificacaoStore();

const itens = ref<ClienteLista[]>([]);
const total = ref(0);
const pagina = ref(1);
const tamanhoPagina = ref(10);
const busca = ref('');
const carregando = ref(false);

async function carregar() {
  carregando.value = true;
  try {
    const resposta = await api.listar({
      busca: busca.value || undefined,
      pagina: pagina.value,
      tamanhoPagina: tamanhoPagina.value,
    });
    itens.value = resposta.itens;
    total.value = resposta.total;
  } catch (e) {
    notificar.erro('Erro ao carregar clientes', mensagemDeErro(e));
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

async function inativar(cliente: ClienteLista) {
  const ok = await confirmar({
    titulo: 'Inativar cliente?',
    mensagem:
      `${cliente.nome} deixara de aparecer para novos pedidos, mas o historico ` +
      'de vendas dele continua preservado.',
    textoConfirmar: 'Inativar',
    cor: 'error',
  });

  if (!ok) return;

  try {
    await api.inativar(cliente.id);
    notificar.sucesso('Cliente inativado', cliente.nome);
    carregar();
  } catch (e) {
    notificar.erro('Nao foi possivel inativar', mensagemDeErro(e));
  }
}

onMounted(carregar);
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Listagem de Clientes"
      :trilha="[{ texto: 'Início', para: '/' }, { texto: 'Cliente' }]"
    >
      <template #acoes>
        <v-btn color="primary" variant="flat" to="/clientes/novo">Cadastrar</v-btn>
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
          icone="mdi-account-outline"
          :titulo="busca ? 'Nenhum cliente encontrado' : 'Nenhum cliente cadastrado'"
          :descricao="
            busca
              ? 'Tente outro termo de busca.'
              : 'Cadastre seu primeiro cliente para começar a emitir pedidos.'
          "
          :texto-acao="busca ? undefined : 'Cadastrar cliente'"
          @acao="navigateTo('/clientes/novo')"
        />

        <v-table v-else-if="!carregando" hover>
          <thead>
            <tr>
              <th class="text-left">Nome</th>
              <th class="text-left">CNPJ</th>
              <th class="text-left">Cidade</th>
              <th class="text-left">Telefone</th>
              <th class="text-center">Situação</th>
              <th class="text-center">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="c in itens" :key="c.id">
              <td>
                <div class="font-weight-medium">{{ c.nome }}</div>
                <div class="text-caption text-medium-emphasis">{{ c.razaoSocial }}</div>
              </td>
              <td>{{ cnpj(c.cnpj) }}</td>
              <td>{{ c.cidadeNome || '—' }}</td>
              <td>{{ telefone(c.telefone) || '—' }}</td>
              <td class="text-center">
                <v-chip :color="c.ativo ? 'success' : 'secondary'" size="small" variant="tonal">
                  {{ c.ativo ? 'Ativo' : 'Inativo' }}
                </v-chip>
              </td>
              <td class="text-center text-no-wrap">
                <v-btn icon variant="text" size="small" :to="`/clientes/${c.id}`" title="Visualizar">
                  <v-icon icon="mdi-eye-outline" size="20" />
                </v-btn>
                <v-btn icon variant="text" size="small" :to="`/clientes/${c.id}/editar`" title="Editar">
                  <v-icon icon="mdi-pencil-outline" size="20" />
                </v-btn>
                <v-btn
                  v-if="c.ativo"
                  icon
                  variant="text"
                  size="small"
                  title="Inativar"
                  @click="inativar(c)"
                >
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
