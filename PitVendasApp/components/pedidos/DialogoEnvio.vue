<script setup lang="ts">
import { ref, watch } from 'vue';
import { useEnvioApi, useRelatoriosApi } from '@/composables/useApiRecursos';
import { useNotificacaoStore } from '@/stores/notificacao';
import { mensagemDeErro } from '@/utils/api';
import type { Pedido } from '@/types/api';

const props = defineProps<{
  modelValue: boolean;
  pedido: Pedido;
}>();

const emit = defineEmits<{ 'update:modelValue': [valor: boolean] }>();

const envioApi = useEnvioApi();
const relatoriosApi = useRelatoriosApi();
const notificar = useNotificacaoStore();

const emailDisponivel = ref(false);
const destinatario = ref('');
const mensagem = ref('');
const enviandoEmail = ref(false);
const gerandoWhats = ref(false);

watch(
  () => props.modelValue,
  async (aberto) => {
    if (!aberto) return;
    destinatario.value = props.pedido.clienteNome ? '' : '';
    try {
      const disp = await envioApi.disponibilidade(props.pedido.id);
      emailDisponivel.value = disp.email;
    } catch {
      emailDisponivel.value = false;
    }
  },
);

function fechar() {
  emit('update:modelValue', false);
}

async function enviarEmail() {
  if (!destinatario.value) {
    notificar.aviso('Informe o e-mail', 'Digite o e-mail do destinatário.');
    return;
  }
  enviandoEmail.value = true;
  try {
    await envioApi.email(props.pedido.id, destinatario.value, mensagem.value || undefined);
    notificar.sucesso('E-mail enviado', `Pedido nº ${props.pedido.numero} enviado para ${destinatario.value}.`);
    fechar();
  } catch (e) {
    notificar.erro('Não foi possível enviar', mensagemDeErro(e));
  } finally {
    enviandoEmail.value = false;
  }
}

async function enviarWhatsApp() {
  gerandoWhats.value = true;
  try {
    await relatoriosApi.pedidoPdf(props.pedido.id);
    const { link } = await envioApi.whatsapp(props.pedido.id);
    window.open(link, '_blank');
    notificar.info('WhatsApp aberto', 'A mensagem foi preenchida. Anexe o PDF que acabou de baixar.');
    fechar();
  } catch (e) {
    notificar.erro('Não foi possível abrir o WhatsApp', mensagemDeErro(e));
  } finally {
    gerandoWhats.value = false;
  }
}
</script>

<template>
  <v-dialog :model-value="modelValue" max-width="520" @update:model-value="emit('update:modelValue', $event)">
    <v-card rounded="lg">
      <v-card-item class="px-6 pt-5">
        <div class="d-flex align-center justify-space-between">
          <h5 class="text-h5 font-weight-medium">Enviar pedido nº {{ pedido.numero }}</h5>
          <v-btn icon variant="text" size="small" @click="fechar">
            <v-icon icon="mdi-close" />
          </v-btn>
        </div>
      </v-card-item>

      <v-divider class="mt-3" />

      <v-card-text class="pa-6">

        <div class="mb-6">
          <div class="d-flex align-center ga-2 mb-2">
            <v-icon icon="mdi-whatsapp" color="success" />
            <span class="text-body-1 font-weight-medium">WhatsApp</span>
          </div>
          <p class="text-body-2 text-medium-emphasis mb-3">
            Abre o WhatsApp com a mensagem do pedido pronta. O PDF é baixado para você anexar.
          </p>
          <v-btn color="success" variant="flat" :loading="gerandoWhats" @click="enviarWhatsApp">
            <v-icon icon="mdi-whatsapp" start /> Abrir no WhatsApp
          </v-btn>
        </div>

        <v-divider class="mb-6" />

        <div>
          <div class="d-flex align-center ga-2 mb-2">
            <v-icon icon="mdi-email-outline" color="primary" />
            <span class="text-body-1 font-weight-medium">E-mail</span>
          </div>

          <v-alert v-if="!emailDisponivel" type="info" variant="tonal" density="comfortable" class="mb-2">
            O envio por e-mail não está configurado neste ambiente. Configure o SMTP para habilitar.
          </v-alert>

          <template v-else>
            <v-text-field
              v-model="destinatario"
              type="email"
              label="E-mail do destinatário"
              placeholder="cliente@exemplo.com"
              variant="outlined"
              density="comfortable"
              hide-details
              class="mb-3"
            />
            <v-textarea
              v-model="mensagem"
              label="Mensagem (opcional)"
              rows="2"
              variant="outlined"
              density="comfortable"
              hide-details
              class="mb-3"
            />
            <v-btn color="primary" variant="flat" :loading="enviandoEmail" @click="enviarEmail">
              <v-icon icon="mdi-send" start /> Enviar por e-mail
            </v-btn>
          </template>
        </div>
      </v-card-text>
    </v-card>
  </v-dialog>
</template>
