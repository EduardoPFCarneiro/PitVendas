import { reactive } from 'vue';

interface EstadoConfirmacao {
  aberto: boolean;
  titulo: string;
  mensagem: string;
  textoConfirmar: string;
  textoCancelar: string;
  cor: string;
  resolver: ((confirmado: boolean) => void) | null;
}

const estado = reactive<EstadoConfirmacao>({
  aberto: false,
  titulo: '',
  mensagem: '',
  textoConfirmar: 'Confirmar',
  textoCancelar: 'Cancelar',
  cor: 'primary',
  resolver: null,
});

export interface OpcoesConfirmacao {
  titulo: string;
  mensagem: string;
  textoConfirmar?: string;
  textoCancelar?: string;
  cor?: string;
}

export function useConfirmacao() {
  function confirmar(opcoes: OpcoesConfirmacao): Promise<boolean> {
    estado.titulo = opcoes.titulo;
    estado.mensagem = opcoes.mensagem;
    estado.textoConfirmar = opcoes.textoConfirmar ?? 'Confirmar';
    estado.textoCancelar = opcoes.textoCancelar ?? 'Cancelar';
    estado.cor = opcoes.cor ?? 'primary';
    estado.aberto = true;

    return new Promise((resolve) => {
      estado.resolver = resolve;
    });
  }

  function responder(confirmado: boolean) {
    estado.aberto = false;
    estado.resolver?.(confirmado);
    estado.resolver = null;
  }

  return { estado, confirmar, responder };
}
