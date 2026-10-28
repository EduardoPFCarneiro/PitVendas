import api from '@/utils/api';

export default defineNuxtPlugin(() => {
  const config = useRuntimeConfig();
  api.defaults.baseURL = config.public.apiBase as string;

  return {
    provide: { api },
  };
});
