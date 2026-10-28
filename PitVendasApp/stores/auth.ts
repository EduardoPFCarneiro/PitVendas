import { defineStore } from 'pinia';
import api, { mensagemDeErro } from '@/utils/api';
import {
  lerToken, lerUsuario, gravarSessao, atualizarUsuario, limparSessao,
  lerEmailLembrado, gravarEmailLembrado, lembrarAtivo,
} from '@/utils/sessaoStorage';
import type { LoginPayload, RegistroPayload, TokenResposta, Usuario } from '@/types/api';

interface AuthState {
  token: string | null;
  usuario: Usuario | null;
  carregando: boolean;
}

export const useAuthStore = defineStore('auth', {
  state: (): AuthState => ({
    token: null,
    usuario: null,
    carregando: false,
  }),

  getters: {
    autenticado: (state) => !!state.token,
    primeiroNome: (state) => state.usuario?.nome?.split(' ')[0] ?? '',
    iniciais: (state) => {
      const partes = state.usuario?.nome?.trim().split(/\s+/) ?? [];
      if (!partes.length) return '';
      const primeira = partes[0][0] ?? '';
      const ultima = partes.length > 1 ? partes[partes.length - 1][0] : '';
      return (primeira + ultima).toUpperCase();
    },
  },

  actions: {
    restaurar() {
      if (!import.meta.client) return;

      this.token = lerToken();
      const bruto = lerUsuario();

      if (bruto) {
        try {
          this.usuario = JSON.parse(bruto) as Usuario;
        } catch {
          this.limpar();
        }
      }
    },

    async login(payload: LoginPayload, lembrar = false) {
      this.carregando = true;
      try {
        const { data } = await api.post<TokenResposta>('/auth/login', payload);
        this.salvar(data, lembrar);
        gravarEmailLembrado(payload.email, lembrar);
        return data;
      } catch (erro) {
        throw new Error(mensagemDeErro(erro, 'E-mail ou senha invalidos.'));
      } finally {
        this.carregando = false;
      }
    },

    async registrar(payload: RegistroPayload) {
      this.carregando = true;
      try {
        const { data } = await api.post<TokenResposta>('/auth/registrar', payload);
        this.salvar(data, false);
        return data;
      } catch (erro) {
        throw new Error(mensagemDeErro(erro, 'Nao foi possivel criar a conta.'));
      } finally {
        this.carregando = false;
      }
    },

    async esqueciSenha(email: string) {
      const { data } = await api.post<{ mensagem: string; linkDesenvolvimento?: string }>(
        '/auth/esqueci-senha', { email },
      );
      return data;
    },

    async redefinirSenha(email: string, token: string, novaSenha: string) {
      try {
        const { data } = await api.post<{ mensagem: string }>('/auth/redefinir-senha', {
          email, token, novaSenha,
        });
        return data;
      } catch (erro) {
        throw new Error(mensagemDeErro(erro, 'Nao foi possivel redefinir a senha.'));
      }
    },

    async carregarPerfil() {
      const { data } = await api.get<Usuario>('/auth/eu');
      this.usuario = data;
      atualizarUsuario(JSON.stringify(data));
      return data;
    },

    salvar(resposta: TokenResposta, lembrar: boolean) {
      this.token = resposta.token;
      this.usuario = resposta.usuario;
      gravarSessao(resposta.token, JSON.stringify(resposta.usuario), lembrar);
    },

    limpar() {
      this.token = null;
      this.usuario = null;
      limparSessao();
    },

    logout() {
      this.limpar();
      return navigateTo('/login');
    },

    emailLembrado: () => lerEmailLembrado(),
    lembrarMarcado: () => lembrarAtivo(),
  },
});
