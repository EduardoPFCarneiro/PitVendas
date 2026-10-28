<script setup lang="ts">
withDefaults(
  defineProps<{
    icone?: string;
    titulo: string;
    descricao?: string;
    textoAcao?: string;
  }>(),
  { icone: 'mdi-inbox-outline' },
);

defineEmits<{ acao: [] }>();
</script>

<template>

  <div class="text-center py-12 px-4">
    <v-avatar size="72" color="lightprimary" class="mb-4">
      <v-icon :icon="icone" size="36" color="primary" />
    </v-avatar>
    <h5 class="text-h6 font-weight-semibold mb-1">{{ titulo }}</h5>
    <p v-if="descricao" class="text-body-2 text-medium-emphasis mb-4">
      {{ descricao }}
    </p>
    <v-btn v-if="textoAcao" color="primary" variant="flat" @click="$emit('acao')">
      {{ textoAcao }}
    </v-btn>
  </div>
</template>
