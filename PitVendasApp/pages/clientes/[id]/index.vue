<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useClientesApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Cliente } from '@/types/api';

const rota = useRoute();
const api = useClientesApi();
const { cnpj, telefone } = useFormato();
const notificar = useNotificacaoStore();

const cliente = ref<Cliente | null>(null);
const carregando = ref(true);

useHead({ title: 'Cliente' });

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
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Visualização de Clientes"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Cliente', para: '/clientes' },
        { texto: cliente?.nome ?? '...' },
      ]"
    >
      <template #acoes>
        <v-btn
          v-if="cliente"
          color="secondary"
          variant="tonal"
          :to="`/clientes/${cliente.id}/editar`"
        >
          Editar
        </v-btn>
        <v-btn color="primary" variant="flat" to="/clientes">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <template v-else-if="cliente">
      <v-card elevation="0" rounded="lg" class="mb-6">
        <v-card-item class="px-6 py-4">
          <h5 class="text-h5 font-weight-medium">Informações Gerais</h5>
        </v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-row>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Nome</div>
              <div class="text-body-1 font-weight-medium">{{ cliente.nome }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Razão Social</div>
              <div class="text-body-1 font-weight-medium">{{ cliente.razaoSocial }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">CNPJ</div>
              <div class="text-body-1 font-weight-medium">{{ cnpj(cliente.cnpj) }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Telefone</div>
              <div class="text-body-1 font-weight-medium">
                {{ telefone(cliente.telefone) || '—' }}
              </div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">E-mail</div>
              <div class="text-body-1 font-weight-medium">{{ cliente.email || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Situação</div>
              <v-chip :color="cliente.ativo ? 'success' : 'secondary'" size="small" variant="tonal">
                {{ cliente.ativo ? 'Ativo' : 'Inativo' }}
              </v-chip>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card elevation="0" rounded="lg">
        <v-card-item class="px-6 py-4">
          <h5 class="text-h5 font-weight-medium">Endereço</h5>
        </v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-row v-if="cliente.endereco">
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Logradouro</div>
              <div class="text-body-1 font-weight-medium">
                {{ cliente.endereco.logradouro || '—' }}
              </div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Número</div>
              <div class="text-body-1 font-weight-medium">
                {{ cliente.endereco.numero || '—' }}
              </div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Bairro</div>
              <div class="text-body-1 font-weight-medium">
                {{ cliente.endereco.bairro || '—' }}
              </div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">CEP</div>
              <div class="text-body-1 font-weight-medium">
                {{ cliente.endereco.cep || '—' }}
              </div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Cidade</div>
              <div class="text-body-1 font-weight-medium">
                {{ cliente.endereco.cidadeNome || '—' }}
              </div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Estado</div>
              <div class="text-body-1 font-weight-medium">
                {{ cliente.endereco.estadoSigla || '—' }}
              </div>
            </v-col>
          </v-row>
          <p v-else class="text-medium-emphasis mb-0">Nenhum endereço cadastrado.</p>
        </v-card-text>
      </v-card>
    </template>
  </div>
</template>
