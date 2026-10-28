import { defineStore } from 'pinia';

export type TipoNotificacao = 'success' | 'error' | 'warning' | 'info';

export interface Notificacao {
  id: number;
  tipo: TipoNotificacao;
  titulo: string;
  texto?: string;
  duracao: number;
}

let proximoId = 1;

export const useNotificacaoStore = defineStore('notificacao', {
  state: () => ({
    itens: [] as Notificacao[],
  }),

  actions: {
    mostrar(tipo: TipoNotificacao, titulo: string, texto?: string, duracao?: number) {
      const id = proximoId++;
      const padrao = tipo === 'error' ? 8000 : 4000;

      this.itens.push({ id, tipo, titulo, texto, duracao: duracao ?? padrao });

      setTimeout(() => this.remover(id), duracao ?? padrao);
      return id;
    },

    sucesso(titulo: string, texto?: string) {
      return this.mostrar('success', titulo, texto);
    },

    erro(titulo: string, texto?: string) {
      return this.mostrar('error', titulo, texto);
    },

    aviso(titulo: string, texto?: string) {
      return this.mostrar('warning', titulo, texto);
    },

    info(titulo: string, texto?: string) {
      return this.mostrar('info', titulo, texto);
    },

    remover(id: number) {
      this.itens = this.itens.filter((n) => n.id !== id);
    },
  },
});
