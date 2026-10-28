<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { lerEmailLembrado, lembrarAtivo } from '@/utils/sessaoStorage';

definePageMeta({ layout: 'blank' });
useHead({ title: 'Entrar | Pit Vendas' });

const auth = useAuthStore();
const route = useRoute();

const email = ref('');
const senha = ref('');
const lembrar = ref(false);
const mostrarSenha = ref(false);
const erro = ref('');
const formValido = ref(false);

const regrasEmail = [
  (v: string) => !!v || 'Informe o e-mail.',
  (v: string) => /.+@.+\..+/.test(v) || 'E-mail inválido.',
];

const regrasSenha = [(v: string) => !!v || 'Informe a senha.'];

onMounted(() => {
  lembrar.value = lembrarAtivo();
  if (lembrar.value) email.value = lerEmailLembrado();
});

async function entrar() {
  erro.value = '';
  try {
    await auth.login({ email: email.value, senha: senha.value }, lembrar.value);
    const destino = (route.query.redirect as string) || '/';
    await navigateTo(destino);
  } catch (e) {
    erro.value = (e as Error).message;
  }
}
</script>

<template>
  <div class="d-flex align-center justify-center pa-4" style="min-height: 100vh">
    <v-card elevation="10" class="pa-2" max-width="450" width="100%">
      <v-card-item class="pa-sm-8 pa-4">
        <div class="text-center mb-8">
          <h1 class="text-h4 font-weight-bold text-primary mb-1">Pit Vendas</h1>
          <p class="text-subtitle-1 text-medium-emphasis">Controle e emissão de vendas</p>
        </div>

        <v-alert v-if="erro" type="error" variant="tonal" density="compact" class="mb-5">
          {{ erro }}
        </v-alert>

        <v-form v-model="formValido" @submit.prevent="entrar">
          <v-label class="text-subtitle-1 font-weight-medium pb-2">E-mail</v-label>
          <v-text-field
            v-model="email"
            :rules="regrasEmail"
            type="email"
            autocomplete="username"
            variant="outlined"
            density="comfortable"
            placeholder="voce@exemplo.com"
            class="mb-3"
            hide-details="auto"
          />

          <v-label class="text-subtitle-1 font-weight-medium pb-2">Senha</v-label>
          <v-text-field
            v-model="senha"
            :rules="regrasSenha"
            :type="mostrarSenha ? 'text' : 'password'"
            :append-inner-icon="mostrarSenha ? 'mdi-eye-off' : 'mdi-eye'"
            autocomplete="current-password"
            variant="outlined"
            density="comfortable"
            class="mb-2"
            hide-details="auto"
            @click:append-inner="mostrarSenha = !mostrarSenha"
          />

          <div class="d-flex align-center justify-space-between flex-wrap mb-5">
            <v-checkbox
              v-model="lembrar"
              label="Lembrar acesso"
              color="primary"
              density="compact"
              hide-details
            />
            <NuxtLink to="/esqueci-senha" class="text-primary text-body-2 font-weight-medium">
              Esqueceu a senha?
            </NuxtLink>
          </div>

          <v-btn
            type="submit"
            color="primary"
            size="large"
            block
            flat
            :loading="auth.carregando"
            :disabled="!formValido"
          >
            Entrar
          </v-btn>
        </v-form>

        <div class="text-center mt-6">
          <span class="text-medium-emphasis">Ainda não tem conta?</span>
          <NuxtLink to="/registrar" class="text-primary font-weight-medium ml-1">
            Cadastre-se
          </NuxtLink>
        </div>
      </v-card-item>
    </v-card>
  </div>
</template>
