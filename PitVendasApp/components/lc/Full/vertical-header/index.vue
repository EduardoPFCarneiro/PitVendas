<script setup lang="ts">
import { ref, onBeforeMount } from "vue";
import { useCustomizerStore } from "@/stores/customizer";
import { Icon } from "@iconify/vue";

const customizer = useCustomizerStore();

const stickyHeader = ref(false);

function handleScroll() {
  stickyHeader.value = window.scrollY > 0;
}

onBeforeMount(() => {
  window.addEventListener("scroll", handleScroll);
});
</script>

<template>

  <v-app-bar
    elevation="0"
    height="70"
    color="secondary"
    id="top"
    :class="stickyHeader ? 'sticky' : ''"
  >
    <v-btn
      class="hidden-md-and-down"
      icon
      variant="text"
      size="small"
      @click.stop="customizer.SET_MINI_SIDEBAR(!customizer.mini_sidebar)"
    >
      <Icon icon="solar:list-bold-duotone" height="24" width="24" />
    </v-btn>

    <v-btn
      class="hidden-lg-and-up"
      icon
      variant="text"
      size="small"
      @click.stop="customizer.SET_SIDEBAR_DRAWER"
    >
      <Icon icon="solar:list-bold-duotone" height="24" width="24" />
    </v-btn>

    <v-spacer />

    <div class="me-4">
      <LcFullVerticalHeaderProfileDD />
    </div>
  </v-app-bar>
</template>
