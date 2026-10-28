<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFabricasApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Fabrica } from '@/types/api';

const rota = useRoute();
const api = useFabricasApi();
const { cnpj, telefone, percentual } = useFormato();
const notificar = useNotificacaoStore();

const fabrica = ref<Fabrica | null>(null);
const carregando = ref(true);

useHead({ title: 'Fábrica' });

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
</script>

<template>
  <div>
    <ComumCabecalhoPagina
      titulo="Visualização de Fábricas"
      :trilha="[
        { texto: 'Início', para: '/' },
        { texto: 'Fábrica', para: '/fabricas' },
        { texto: fabrica?.nome ?? '...' },
      ]"
    >
      <template #acoes>
        <v-btn v-if="fabrica" color="secondary" variant="tonal" :to="`/fabricas/${fabrica.id}/editar`">
          Editar
        </v-btn>
        <v-btn color="primary" variant="flat" to="/fabricas">Listagem</v-btn>
      </template>
    </ComumCabecalhoPagina>

    <v-progress-linear v-if="carregando" indeterminate color="primary" />

    <template v-else-if="fabrica">
      <v-card elevation="0" rounded="lg" class="mb-6">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Informações Gerais</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-row>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Nome</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.nome }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Razão Social</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.razaoSocial }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">CNPJ</div>
              <div class="text-body-1 font-weight-medium">{{ cnpj(fabrica.cnpj) }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Comissão</div>
              <div class="text-body-1 font-weight-medium">{{ percentual(fabrica.comissao) }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Telefone</div>
              <div class="text-body-1 font-weight-medium">{{ telefone(fabrica.telefone) || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">E-mail</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.email || '—' }}</div>
            </v-col>
          </v-row>
        </v-card-text>
      </v-card>

      <v-card elevation="0" rounded="lg">
        <v-card-item class="px-6 py-4"><h5 class="text-h5 font-weight-medium">Endereço</h5></v-card-item>
        <v-divider />
        <v-card-text class="pa-6">
          <v-row v-if="fabrica.endereco">
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Logradouro</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.endereco.logradouro || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Número</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.endereco.numero || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Bairro</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.endereco.bairro || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">CEP</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.endereco.cep || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Cidade</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.endereco.cidadeNome || '—' }}</div>
            </v-col>
            <v-col cols="12" md="4">
              <div class="text-body-2 text-medium-emphasis">Estado</div>
              <div class="text-body-1 font-weight-medium">{{ fabrica.endereco.estadoSigla || '—' }}</div>
            </v-col>
          </v-row>
          <p v-else class="text-medium-emphasis mb-0">Nenhum endereço cadastrado.</p>
        </v-card-text>
      </v-card>
    </template>
  </div>
</template>
