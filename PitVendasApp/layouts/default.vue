<script setup lang="ts">
import { useCustomizerStore } from "@/stores/customizer";
const customizer = useCustomizerStore();

useHead({
  titleTemplate: (titleChunk) =>
    titleChunk && titleChunk !== "Pit Vendas"
      ? `${titleChunk} | Pit Vendas`
      : "Pit Vendas",
});

</script>

<template>
  <!-----RTL LAYOUT------->
  <v-locale-provider v-if="customizer.setRTLLayout" rtl>
    <v-app

      :class="[
        customizer.actTheme,
        customizer.mini_sidebar ? 'mini-sidebar' : '',
        customizer.setHorizontalLayout ? 'horizontalLayout' : 'verticalLayout',
        customizer.setBorderCard ? 'cardBordered' : '',
      ]"
    >

      <ClientOnly>
        <LcFullVerticalSidebar v-if="!customizer.setHorizontalLayout" />
        <div :class="customizer.boxed ? 'maxWidth' : 'full-header'">
          <LcFullVerticalHeader v-if="!customizer.setHorizontalLayout" />
        </div>
        <div :class="customizer.boxed ? 'maxWidth' : 'full-header'">
          <LcFullHorizontalHeader v-if="customizer.setHorizontalLayout" />
        </div>
        <LcFullHorizontalSidebar v-if="customizer.setHorizontalLayout" />
      </ClientOnly>
      <v-main>
        <div class="rtl-lyt mb-3 hr-layout">
          <v-container
            fluid
            class="page-wrapper  px-sm-5 px-4 pt-12 rounded-xl"
          >
            <div class="">
              <div :class="customizer.boxed ? 'maxWidth' : ''">
                <NuxtPage />
              </div>
            </div>
          </v-container>
        </div>
      </v-main>
    </v-app>
  </v-locale-provider>

  <!-----LTR LAYOUT------->
  <v-locale-provider v-else>
    <v-app
    :theme="customizer.actTheme"
      :class="[
        customizer.actTheme,
        customizer.mini_sidebar ? 'mini-sidebar' : '',
        customizer.setHorizontalLayout ? 'horizontalLayout' : 'verticalLayout',
        customizer.setBorderCard ? 'cardBordered' : '',
      ]"
    >
      <ClientOnly>
        <LcFullVerticalSidebar v-if="!customizer.setHorizontalLayout" />
        <div :class="customizer.boxed ? 'maxWidth' : 'full-header'">
          <LcFullVerticalHeader v-if="!customizer.setHorizontalLayout" />
        </div>
        <div :class="customizer.boxed ? 'maxWidth' : 'full-header'">
          <LcFullHorizontalHeader v-if="customizer.setHorizontalLayout" />
        </div>
        <LcFullHorizontalSidebar v-if="customizer.setHorizontalLayout" />
      </ClientOnly>
      <v-main>
        <div class="mb-3 hr-layout">
          <v-container
            fluid
            class="page-wrapper  px-sm-5 px-4 pt-12 rounded-xl"
          >
            <div class="">
              <div :class="customizer.boxed ? 'maxWidth' : ''">
                <NuxtPage />
              </div>
            </div>
          </v-container>
        </div>
      </v-main>
    </v-app>
  </v-locale-provider>
</template>
