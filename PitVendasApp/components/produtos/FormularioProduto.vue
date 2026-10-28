<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useLookupsApi, useFabricasApi, useProdutosApi } from '@/composables/useApiRecursos';
import { useFormato } from '@/composables/useFormato';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Produto, ProdutoForm, PrecoForm, Lookup, FabricaLista } from '@/types/api';

const props = defineProps<{
  valorInicial?: Produto | null;
  salvando: boolean;
}>();

const emit = defineEmits<{ salvar: [dados: ProdutoForm] }>();

const lookups = useLookupsApi();
const fabricasApi = useFabricasApi();
const produtosApi = useProdutosApi();
const { moeda } = useFormato();
const notificar = useNotificacaoStore();

const passo = ref(1);
const formValido = ref(false);

const cores = ref<Lookup[]>([]);
const tamanhos = ref<Lookup[]>([]);
const fabricas = ref<FabricaLista[]>([]);

const dados = ref<ProdutoForm>({
  codigo: '',
  descricao: '',
  observacao: null,
  foto: null,
  fabricaId: null as unknown as number,
  ativo: true,
  corIds: [],
  tamanhoIds: [],
});

const precos = ref<Array<PrecoForm & { id?: number }>>([]);
const novoPreco = ref<PrecoForm>({ codigo: '', valor: 0, observacao: null, ativo: true });

const emEdicao = computed(() => !!props.valorInicial);

const regrasCodigo = [(v: string) => !!v?.trim() || 'Informe o código.'];
const regrasDescricao = [(v: string) => !!v?.trim() || 'Informe a descrição.'];
const regrasFabrica = [(v: number) => !!v || 'Selecione a fábrica.'];
const regrasCores = [(v: number[]) => v.length > 0 || 'Selecione ao menos uma cor.'];
const regrasTamanhos = [(v: number[]) => v.length > 0 || 'Selecione ao menos um tamanho.'];

async function adicionarPreco() {
  if (!novoPreco.value.codigo.trim() || novoPreco.value.valor <= 0) {
    notificar.aviso('Preço incompleto', 'Informe o código da tabela e um valor maior que zero.');
    return;
  }

  if (precos.value.some((p) => p.codigo === novoPreco.value.codigo)) {
    notificar.aviso('Código repetido', `Já existe uma tabela ${novoPreco.value.codigo}.`);
    return;
  }

  const preco = { ...novoPreco.value };

  if (emEdicao.value) {
    try {
      const criado = await produtosApi.adicionarPreco(props.valorInicial!.id, preco);
      precos.value.push(criado);
      notificar.sucesso('Preço adicionado', `${criado.codigo} — ${moeda(criado.valor)}`);
    } catch (e) {
      notificar.erro('Não foi possível adicionar', mensagemDeErro(e));
      return;
    }
  } else {
    precos.value.push(preco);
  }

  novoPreco.value = { codigo: '', valor: 0, observacao: null, ativo: true };
}

async function removerPreco(indice: number) {
  const preco = precos.value[indice];
  if (emEdicao.value && preco.id) {
    try {
      await produtosApi.removerPreco(props.valorInicial!.id, preco.id);
    } catch (e) {
      notificar.erro('Não foi possível remover', mensagemDeErro(e));
      return;
    }
  }
  precos.value.splice(indice, 1);
}

function submeter() {
  emit('salvar', { ...dados.value });
}

onMounted(async () => {
  const [listaCores, listaTamanhos, rf] = await Promise.all([
    lookups.cores(),
    lookups.tamanhos(),
    fabricasApi.listar({ ativo: true, tamanhoPagina: 100 }),
  ]);
  cores.value = listaCores;
  tamanhos.value = listaTamanhos;
  fabricas.value = rf.itens;

  if (props.valorInicial) {
    const v = props.valorInicial;
    dados.value = {
      codigo: v.codigo,
      descricao: v.descricao,
      observacao: v.observacao ?? null,
      foto: v.foto ?? null,
      fabricaId: v.fabricaId,
      ativo: v.ativo,
      corIds: v.cores.map((c) => c.id),
      tamanhoIds: v.tamanhos.map((t) => t.id),
    };
    precos.value = v.precos.map((p) => ({
      id: p.id,
      codigo: p.codigo,
      valor: p.valor,
      observacao: p.observacao ?? null,
      ativo: p.ativo,
    }));
  }
});

defineExpose({ precos });
</script>

<template>
  <v-card elevation="0" rounded="lg">
    <v-card-item class="px-6 pt-5 pb-0">
      <div class="d-flex align-center justify-space-between flex-wrap ga-3">
        <h5 class="text-h5 font-weight-medium">
          {{ passo === 1 ? 'Dados Gerais' : 'Tabelas de Preço' }}
        </h5>
        <div class="d-flex align-center ga-2 text-body-2 text-medium-emphasis">
          <span :class="passo === 1 ? 'text-primary font-weight-semibold' : ''">1. Dados</span>
          <v-icon icon="mdi-chevron-right" size="16" />
          <span :class="passo === 2 ? 'text-primary font-weight-semibold' : ''">2. Preços</span>
        </div>
      </div>
    </v-card-item>

    <v-divider class="mt-4" />

    <v-card-text class="pa-6">
      <!-- --------------------- passo 1: dados --------------------- -->
      <v-form v-if="passo === 1" v-model="formValido">
        <v-row>
          <v-col cols="12" md="4">
            <v-label class="mb-2 font-weight-medium">Código *</v-label>
            <v-text-field v-model="dados.codigo" :rules="regrasCodigo" placeholder="Ex.: 7209" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
          <v-col cols="12" md="8">
            <v-label class="mb-2 font-weight-medium">Descrição *</v-label>
            <v-text-field v-model="dados.descricao" :rules="regrasDescricao" placeholder="Ex.: Pijama Infantil Manga Longa" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>

          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">Fábrica *</v-label>
            <v-select
              v-model="dados.fabricaId"
              :items="fabricas"
              :rules="regrasFabrica"
              item-title="nome"
              item-value="id"
              placeholder="Selecione a fábrica"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
              :disabled="emEdicao"
              :hint="emEdicao ? 'A fábrica não pode ser alterada após o cadastro.' : ''"
              :persistent-hint="emEdicao"
            />
          </v-col>

          <v-col cols="12" md="6">
            <v-label class="mb-2 font-weight-medium">Cores *</v-label>
            <v-select
              v-model="dados.corIds"
              :items="cores"
              :rules="regrasCores"
              item-title="nome"
              item-value="id"
              multiple
              chips
              closable-chips
              placeholder="Cores disponíveis"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>

          <v-col cols="12">
            <v-label class="mb-2 font-weight-medium">Tamanhos (grade) *</v-label>
            <v-select
              v-model="dados.tamanhoIds"
              :items="tamanhos"
              :rules="regrasTamanhos"
              item-title="nome"
              item-value="id"
              multiple
              chips
              closable-chips
              placeholder="Tamanhos que compõem a grade"
              variant="outlined"
              density="comfortable"
              hide-details="auto"
            />
          </v-col>

          <v-col cols="12">
            <v-label class="mb-2 font-weight-medium">Observação</v-label>
            <v-textarea v-model="dados.observacao" rows="2" placeholder="Opcional" variant="outlined" density="comfortable" hide-details="auto" />
          </v-col>
        </v-row>

        <div class="d-flex justify-end mt-6">
          <v-btn color="primary" variant="flat" :disabled="!formValido" @click="passo = 2">Próximo</v-btn>
        </div>
      </v-form>

      <!-- --------------------- passo 2: precos --------------------- -->
      <div v-else>
        <div class="d-flex align-end flex-wrap ga-3 mb-4">
          <div style="min-width: 140px">
            <v-label class="mb-2 font-weight-medium">Código</v-label>
            <v-text-field v-model="novoPreco.codigo" placeholder="Ex.: P1, À VISTA" variant="outlined" density="comfortable" hide-details @keyup.enter="adicionarPreco" />
          </div>
          <div style="min-width: 140px">
            <v-label class="mb-2 font-weight-medium">Valor (R$)</v-label>
            <v-text-field v-model.number="novoPreco.valor" type="number" step="0.01" min="0" placeholder="0,00" variant="outlined" density="comfortable" hide-details @keyup.enter="adicionarPreco" />
          </div>
          <v-btn color="success" variant="flat" size="large" @click="adicionarPreco">
            <v-icon icon="mdi-plus" start /> Adicionar
          </v-btn>
        </div>

        <v-table v-if="precos.length" class="border rounded-lg">
          <thead>
            <tr>
              <th class="text-left">Código</th>
              <th class="text-right">Valor</th>
              <th class="text-center" style="width: 80px">Ações</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(p, i) in precos" :key="p.id ?? i">
              <td class="font-weight-medium">{{ p.codigo }}</td>
              <td class="text-right">{{ moeda(p.valor) }}</td>
              <td class="text-center">
                <v-btn icon variant="text" size="small" color="error" @click="removerPreco(i)">
                  <v-icon icon="mdi-trash-can-outline" size="20" />
                </v-btn>
              </td>
            </tr>
          </tbody>
        </v-table>

        <v-alert v-else type="info" variant="tonal" density="comfortable">
          Nenhuma tabela de preço cadastrada. Você pode cadastrar agora ou depois — o preço também
          pode ser informado direto no pedido.
        </v-alert>

        <div class="d-flex justify-space-between mt-6">
          <v-btn variant="tonal" color="secondary" @click="passo = 1">Voltar</v-btn>
          <v-btn color="success" variant="flat" :loading="salvando" @click="submeter">
            {{ emEdicao ? 'Salvar' : 'Concluir' }}
          </v-btn>
        </div>
      </div>
    </v-card-text>
  </v-card>
</template>
