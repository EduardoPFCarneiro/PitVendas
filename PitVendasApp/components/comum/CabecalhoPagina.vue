<script setup lang="ts">
interface Trilha {
  texto: string;
  para?: string;
}

defineProps<{
  titulo: string;
  trilha: Trilha[];
}>();
</script>

<template>

  <v-card elevation="0" rounded="lg" class="mb-6">
    <v-card-item class="py-4 px-6">
      <div class="d-flex align-center justify-space-between flex-wrap ga-3">
        <div>
          <h3 class="text-h4 font-weight-medium">{{ titulo }}</h3>
          <div class="d-flex align-center text-body-2 text-medium-emphasis mt-1">
            <template v-for="(item, i) in trilha" :key="i">
              <span v-if="i > 0" class="mx-1">›</span>
              <NuxtLink v-if="item.para" :to="item.para" class="text-medium-emphasis text-decoration-none">
                {{ item.texto }}
              </NuxtLink>
              <span v-else>{{ item.texto }}</span>
            </template>
          </div>
        </div>

        <div class="d-flex align-center ga-2">
          <slot name="acoes" />
        </div>
      </div>
    </v-card-item>
  </v-card>
</template>
