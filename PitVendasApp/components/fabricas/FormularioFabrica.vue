<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useLookupsApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import type { Fabrica, FabricaForm, Lookup } from '@/types/api';

const props = defineProps<{
  valorInicial?: Fabrica | null;
  salvando: boolean;
}>();

const emit = defineEmits<{ salvar: [dados: FabricaForm] }>();

const lookups = useLookupsApi();
const { soDigitos } = useFormato();

const passo = ref(1);
const formGeralValido = ref(false);

const estados = ref<Lookup[]>([]);
const cidades = ref<Lookup[]>([]);
const estadoId = ref<number | null>(null);

const dados = ref<FabricaForm>({
  nome: '',
  razaoSocial: '',
  cnpj: '',
  email: null,
  telefone: null,
  comissao: 0,
  ativo: true,
  endereco: {
    logradouro: null,
    numero: null,
    complemento: null,
    bairro: null,
    cep: null,
    cidadeId: null,
  },
});

const regrasNome = [(v: string) => !!v?.trim() || 'Informe o nome.'];
const regrasRazao = [(v: string) => !!v?.trim() || 'Informe a razão social.'];
const regrasCnpj = [
  (v: string) => !!v?.trim() || 'Informe o CNPJ.',
  (v: string) => soDigitos(v).length === 14 || 'O CNPJ deve ter 14 dígitos.',
];
const regrasEmail = [(v: string) => !v || /.+@.+\..+/.test(v) || 'E-mail inválido.'];
const regrasComissao = [
  (v: number) => v >= 0 || 'A comissão não pode ser negativa.',
  (v: number) => v <= 100 || 'A comissão não pode passar de 100%.',
];

const tituloPasso = computed(() => (passo.value === 1 ? 'Informações Gerais' : 'Endereço'));

watch(estadoId, async (novo) => {
  const anterior = dados.value.endereco?.cidadeId;
  cidades.value = novo ? await lookups.cidades(novo) : [];
  if (anterior && !cidades.value.some((c) => c.id === anterior)) {
    dados.value.endereco!.cidadeId = null;
  }
});

function submeter() {
  emit('salvar', {
    ...dados.value,
    cnpj: soDigitos(dados.value.cnpj),
    telefone: dados.value.telefone ? soDigitos(dados.value.telefone) : null,
    comissao: Number(dados.value.comissao) || 0,
  });
}

onMounted(async () => {
  estados.value = await lookups.estados();

  if (props.valorInicial) {
    const v = props.valorInicial;
    dados.value = {
      nome: v.nome,
      razaoSocial: v.razaoSocial,
      cnpj: v.cnpj,
      email: v.email ?? null,
      telefone: v.telefone ?? null,
      comissao: v.comissao,
      ativo: v.ativo,
      endereco: {
        id: v.endereco?.id ?? null,
        logradouro: v.endereco?.logradouro ?? null,
        numero: v.endereco?.numero ?? null,
        complemento: v.endereco?.complemento ?? null,
        bairro: v.endereco?.bairro ?? null,
        cep: v.endereco?.cep ?? null,
        cidadeId: v.endereco?.cidadeId ?? null,
      },
    };
    if (v.endereco?.estadoSigla) {
      const uf = estados.value.find((e) => e.nome && v.endereco?.estadoSigla);
      estadoId.value = uf?.id ?? null;
    }
  }
});
</script>

<template>
  <v-card elevation="0" rounded="lg">
    <v-card-item class="px-6 pt-5 pb-0">
      <div class="d-flex align-center justify-space-between flex-wrap ga-3">
        <h5 class="text-h5 font-weight-medium">{{ tituloPasso }}</h5>
        <div class="d-flex align-center ga-2 text-body-2 text-medium-emphasis">
          <span :class="passo === 1 ? 'text-primary font-weight-semibold' : ''">1. Dados</span>
          <v-icon icon="mdi-chevron-right" size="16" />
          <span :class="passo === 2 ? 'text-primary font-weight-semibold' : ''">2. Endereço</span>
        </div>
      </div>
    </v-card-item>

    <v-divider class="mt-4" />

    <v-card-text class="pa-6">
      <v-form v-if="passo === 1" v-model="formGeralValido">
        <v-row>
          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">Nome *</v-label>
            <v-text-field
              v-model="dados.nome"
              :rules="regrasNome"
              placeholder="Nome da fábrica"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">Razão Social *</v-label>
            <v-text-field
              v-model="dados.razaoSocial"
              :rules="regrasRazao"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">CNPJ *</v-label>
            <v-text-field
              v-model="dados.cnpj"
              v-maska="'##.###.###/####-##'"
              :rules="regrasCnpj"
              placeholder="00.000.000/0000-00"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Comissão (%) *</v-label>
            <v-text-field
              v-model.number="dados.comissao"
              :rules="regrasComissao"
              type="number"
              step="0.5"
              suffix="%"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Telefone</v-label>
            <v-text-field
              v-model="dados.telefone"
              v-maska="'(##) #####-####'"
              placeholder="(69) 90000-0000"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>
          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">E-mail</v-label>
            <v-text-field
              v-model="dados.email"
              :rules="regrasEmail"
              type="email"
              placeholder="contato@fabrica.com"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>
        </v-row>

        <div class="d-flex justify-end mt-6">
          <v-btn color="primary" variant="flat" :disabled="!formGeralValido" @click="passo = 2">
            Próximo
          </v-btn>
        </div>
      </v-form>

      <v-form v-else>
        <v-row>
          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">Rua</v-label>
            <v-text-field v-model="dados.endereco!.logradouro" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Número</v-label>
            <v-text-field v-model="dados.endereco!.numero" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">CEP</v-label>
            <v-text-field v-model="dados.endereco!.cep" v-maska="'#####-###'" placeholder="76900-000" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">Bairro</v-label>
            <v-text-field v-model="dados.endereco!.bairro" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Estado</v-label>
            <v-select v-model="estadoId" :items="estados" item-title="nome" item-value="id" placeholder="Selecione" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
          <v-col cols="12" md="3">
            <v-label class="mb-2 font-weight-medium">Cidade</v-label>
            <v-select v-model="dados.endereco!.cidadeId" :items="cidades" item-title="nome" item-value="id" :disabled="!estadoId" :placeholder="estadoId ? 'Selecione' : 'Escolha o estado antes'" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
        </v-row>

        <div class="d-flex justify-space-between mt-6">
          <v-btn variant="tonal" color="secondary" @click="passo = 1">Voltar</v-btn>
          <v-btn color="success" variant="flat" :loading="salvando" @click="submeter">
            {{ valorInicial ? 'Salvar' : 'Cadastrar' }}
          </v-btn>
        </div>
      </v-form>
    </v-card-text>
  </v-card>
</template>
