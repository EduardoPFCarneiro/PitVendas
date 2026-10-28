import { maska } from 'maska';

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.directive('maska', maska);
});
