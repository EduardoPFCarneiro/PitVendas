import axios, { AxiosError } from 'axios';
import type { ProblemDetails } from '@/types/api';
import { lerToken, limparSessao } from '@/utils/sessaoStorage';

const api = axios.create({
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  if (import.meta.client) {
    const token = lerToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
  }
  return config;
});

api.interceptors.response.use(
  (response) => response,
  (error: AxiosError<ProblemDetails>) => {
    if (error.response?.status === 401 && import.meta.client) {
      limparSessao();
      if (!window.location.pathname.startsWith('/login')) {
        window.location.href = '/login';
      }
    }
    return Promise.reject(error);
  },
);

export function mensagemDeErro(erro: unknown, padrao = 'Nao foi possivel completar a operacao.'): string {
  const axiosErro = erro as AxiosError<ProblemDetails & { mensagem?: string }>;
  const dados = axiosErro?.response?.data;

  if (!dados) {
    return axiosErro?.message === 'Network Error'
      ? 'Nao foi possivel conectar a API. Verifique se ela esta em execucao.'
      : padrao;
  }

  if (dados.errors) {
    const primeiro = Object.values(dados.errors)[0];
    if (primeiro?.length) return primeiro[0];
  }

  return dados.detail ?? dados.mensagem ?? padrao;
}

export default api;
