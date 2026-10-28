import { useAuthStore } from '@/stores/auth';

const PUBLICAS = ['/login', '/registrar', '/esqueci-senha', '/redefinir-senha'];

export default defineNuxtRouteMiddleware((to) => {
  const auth = useAuthStore();

  if (!auth.token) {
    auth.restaurar();
  }

  const publica = PUBLICAS.some((rota) => to.path.startsWith(rota));

  if (!auth.autenticado && !publica) {
    return navigateTo({ path: '/login', query: { redirect: to.fullPath } });
  }

  if (auth.autenticado && publica) {
    return navigateTo('/');
  }
});
