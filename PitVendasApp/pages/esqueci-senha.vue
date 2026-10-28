<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { mensagemDeErro } from '@/utils/api';

definePageMeta({ layout: 'blank' });
useHead({ title: 'Recuperar senha | Pit Vendas' });

const auth = useAuthStore();

const email = ref('');
const formValido = ref(false);
const enviando = ref(false);
const enviado = ref(false);
const mensagem = ref('');
const erro = ref('');

const linkDesenvolvimento = ref<string | null>(null);

const regrasEmail = [
  (v: string) => !!v || 'Informe o e-mail.',
  (v: string) => /.+@.+\..+/.test(v) || 'E-mail inválido.',
];

async function solicitar() {
  erro.value = '';
  enviando.value = true;
  try {
    const r = await auth.esqueciSenha(email.value);
    mensagem.value = r.mensagem;
    linkDesenvolvimento.value = r.linkDesenvolvimento ?? null;
    enviado.value = true;
  } catch (e) {
    erro.value = mensagemDeErro(e);
  } finally {
    enviando.value = false;
  }
}
</script>

<template>
  <div class="d-flex align-center justify-center pa-4" style="min-height: 100vh">
    <v-card elevation="10" class="pa-2" max-width="450" width="100%">
      <v-card-item class="pa-sm-8 pa-4">
        <div class="text-center mb-6">
          <h1 class="text-h4 font-weight-bold text-primary mb-1">Recuperar senha</h1>
          <p class="text-subtitle-1 text-medium-emphasis">
            Informe seu e-mail e enviaremos as instruções.
          </p>
        </div>

        <v-alert v-if="erro" type="error" variant="tonal" density="compact" class="mb-5">
          {{ erro }}
        </v-alert>

        <template v-if="!enviado">
          <v-form v-model="formValido" @submit.prevent="solicitar">
            <v-label class="text-subtitle-1 font-weight-medium pb-2">E-mail</v-label>
            <v-text-field
              v-model="email"
              :rules="regrasEmail"
              type="email"
              autocomplete="username"
              variant="outlined"
              density="comfortable"
              placeholder="voce@exemplo.com"
              class="mb-5"
              hide-details="auto"
            />

            <v-btn
              type="submit"
              color="primary"
              size="large"
              block
              flat
              :loading="enviando"
              :disabled="!formValido"
            >
              Enviar instruções
            </v-btn>
          </v-form>
        </template>

        <template v-else>
          <v-alert type="success" variant="tonal" density="comfortable" class="mb-4">
            {{ mensagem }}
          </v-alert>

          <v-alert
            v-if="linkDesenvolvimento"
            type="info"
            variant="tonal"
            density="comfortable"
            class="mb-4"
          >
            <div class="font-weight-medium mb-1">Ambiente de desenvolvimento</div>
            <div class="text-body-2 mb-2">
              O e-mail não está configurado, então o link aparece aqui para você testar:
            </div>
            <v-btn :href="linkDesenvolvimento" color="info" variant="flat" size="small" block>
              Abrir link de redefinição
            </v-btn>
          </v-alert>
        </template>

        <div class="text-center mt-6">
          <NuxtLink to="/login" class="text-primary font-weight-medium">
            Voltar para o login
          </NuxtLink>
        </div>
      </v-card-item>
    </v-card>
  </div>
</template>
