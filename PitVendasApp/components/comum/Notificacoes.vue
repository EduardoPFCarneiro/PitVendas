<script setup lang="ts">
import { useNotificacaoStore } from '@/stores/notificacao';
import type { TipoNotificacao } from '@/stores/notificacao';

const notificacoes = useNotificacaoStore();

const ICONES: Record<TipoNotificacao, string> = {
  success: 'mdi-check-circle',
  error: 'mdi-alert-circle',
  warning: 'mdi-alert',
  info: 'mdi-information',
};
</script>

<template>

  <div class="pv-notificacoes">
    <TransitionGroup name="pv-toast">
      <v-alert
        v-for="n in notificacoes.itens"
        :key="n.id"
        :type="n.tipo"
        :icon="ICONES[n.tipo]"
        variant="elevated"
        density="comfortable"
        class="mb-3 pv-toast"
        closable
        @click:close="notificacoes.remover(n.id)"
      >
        <div class="font-weight-semibold">{{ n.titulo }}</div>
        <div v-if="n.texto" class="text-body-2 mt-1 text-medium-emphasis-inverse">
          {{ n.texto }}
        </div>
      </v-alert>
    </TransitionGroup>
  </div>
</template>

<style scoped lang="scss">
.pv-notificacoes {
  position: fixed;
  top: 24px;
  right: 24px;
  z-index: 3000;
  width: min(380px, calc(100vw - 32px));
  pointer-events: none;
}

.pv-toast {
  pointer-events: auto;
  box-shadow: 0 8px 24px rgba(17, 28, 45, 0.18);
}

.pv-toast-enter-active,
.pv-toast-leave-active {
  transition: all 0.28s cubic-bezier(0.22, 1, 0.36, 1);
}

.pv-toast-enter-from {
  opacity: 0;
  transform: translateX(24px);
}

.pv-toast-leave-to {
  opacity: 0;
  transform: translateX(24px);
}

.pv-toast-move {
  transition: transform 0.28s ease;
}

@media (max-width: 600px) {
  .pv-notificacoes {
    top: 12px;
    right: 12px;
    left: 12px;
    width: auto;
  }
}
</style>
