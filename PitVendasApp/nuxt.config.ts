import { createResolver } from "@nuxt/kit";
import vuetify, { transformAssetUrls } from "vite-plugin-vuetify";

const { resolve } = createResolver(import.meta.url);

export default defineNuxtConfig({
  ssr: false,

  typescript: {
    shim: false,
  },

  build: {
    transpile: ["vuetify"],
  },

  modules: ["@pinia/nuxt"],

  runtimeConfig: {
    public: {
      apiBase: "http://localhost:5023/api",
    },
  },

  app: {
    head: {
      title: "Pit Vendas",
      htmlAttrs: { lang: "pt-BR" },
      meta: [
        { charset: "utf-8" },
        { name: "viewport", content: "width=device-width, initial-scale=1" },
        {
          name: "description",
          content:
            "Sistema de controle e emissao de vendas para representantes comerciais.",
        },
      ],
    },
  },

  nitro: {
    serveStatic: true,
  },

  devServerHandlers: [],

  compatibilityDate: "2024-09-06",
});
