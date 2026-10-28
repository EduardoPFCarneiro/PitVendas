<script setup lang="ts">
import { ref, computed, onMounted } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { useNotificacaoStore } from '@/stores/notificacao';

definePageMeta({ layout: 'blank' });
useHead({ title: 'Redefinir senha | Pit Vendas' });

const auth = useAuthStore();
const rota = useRoute();
const notificar = useNotificacaoStore();

const email = ref('');
const token = ref('');

const novaSenha = ref('');
const confirmacao = ref('');
const mostrarSenha = ref(false);
const formValido = ref(false);
const salvando = ref(false);
const erro = ref('');

const linkValido = computed(() => !!email.value && !!token.value);

const regrasSenha = [
  (v: string) => !!v || 'Informe a nova senha.',
  (v: string) => (v?.length ?? 0) >= 8 || 'A senha deve ter ao menos 8 caracteres.',
];

const regrasConfirmacao = [
  (v: string) => !!v || 'Repita a nova senha.',
  (v: string) => v === novaSenha.value || 'As senhas não conferem.',
];

onMounted(() => {
  email.value = (rota.query.email as string) ?? '';
  token.value = (rota.query.token as string) ?? '';
});

async function redefinir() {
  erro.value = '';
  salvando.value = true;
  try {
    const r = await auth.redefinirSenha(email.value, token.value, novaSenha.value);
    notificar.sucesso('Senha redefinida', r.mensagem);
    await navigateTo('/login');
  } catch (e) {
    erro.value = (e as Error).message;
  } finally {
    salvando.value = false;
  }
}
</script>

<template>
  <div class="d-flex align-center justify-center pa-4" style="min-height: 100vh">
    <v-card elevation="10" class="pa-2" max-width="450" width="100%">
      <v-card-item class="pa-sm-8 pa-4">
        <div class="text-center mb-6">
          <h1 class="text-h4 font-weight-bold text-primary mb-1">Nova senha</h1>
          <p v-if="linkValido" class="text-subtitle-1 text-medium-emphasis">
            Defina a nova senha de <strong>{{ email }}</strong>.
          </p>
        </div>

        <v-alert v-if="!linkValido" type="warning" variant="tonal" density="comfortable">
          Link de recuperação incompleto. Solicite um novo pela tela de recuperação de senha.
        </v-alert>

        <template v-else>
          <v-alert v-if="erro" type="error" variant="tonal" density="compact" class="mb-5">
            {{ erro }}
          </v-alert>

          <v-form v-model="formValido" @submit.prevent="redefinir">
            <v-label class="text-subtitle-1 font-weight-medium pb-2">Nova senha</v-label>
            <v-text-field
              v-model="novaSenha"
              :rules="regrasSenha"
              :type="mostrarSenha ? 'text' : 'password'"
              :append-inner-icon="mostrarSenha ? 'mdi-eye-off' : 'mdi-eye'"
              autocomplete="new-password"
              variant="outlined"
              density="comfortable"
              class="mb-3"
              hide-details="auto"
              @click:append-inner="mostrarSenha = !mostrarSenha"
            />

            <v-label class="text-subtitle-1 font-weight-medium pb-2">Repita a nova senha</v-label>
            <v-text-field
              v-model="confirmacao"
              :rules="regrasConfirmacao"
              :type="mostrarSenha ? 'text' : 'password'"
              autocomplete="new-password"
              variant="outlined"
              density="comfortable"
              class="mb-5"
              hide-details="auto"
            />

            <v-btn
              type="submit"
              color="primary"
              size="large"
              block
              flat
              :loading="salvando"
              :disabled="!formValido"
            >
              Redefinir senha
            </v-btn>
          </v-form>
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
