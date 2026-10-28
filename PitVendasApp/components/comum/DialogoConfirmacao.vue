<script setup lang="ts">
import { useConfirmacao } from '@/composables/useConfirmacao';

const { estado, responder } = useConfirmacao();
</script>

<template>
  <v-dialog v-model="estado.aberto" max-width="440" persistent>
    <v-card rounded="lg">
      <v-card-item class="pt-6 px-6">
        <h5 class="text-h5 font-weight-semibold">{{ estado.titulo }}</h5>
      </v-card-item>

      <v-card-text class="px-6 text-body-1 text-medium-emphasis">
        {{ estado.mensagem }}
      </v-card-text>

      <v-card-actions class="px-6 pb-6 pt-2 justify-end ga-2">
        <v-btn variant="text" color="secondary" @click="responder(false)">
          {{ estado.textoCancelar }}
        </v-btn>
        <v-btn :color="estado.cor" variant="flat" @click="responder(true)">
          {{ estado.textoConfirmar }}
        </v-btn>
      </v-card-actions>
    </v-card>
  </v-dialog>
</template>
