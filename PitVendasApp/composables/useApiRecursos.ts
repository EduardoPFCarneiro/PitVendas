import api from '@/utils/api';
import type {
  Cliente, ClienteForm, ClienteLista,
  Fabrica, FabricaForm, FabricaLista,
  Produto, ProdutoForm, ProdutoLista, Preco, PrecoForm,
  Pedido, PedidoForm, PedidoLista, PedidoStatus,
  Filtro, Lookup, Pagina, Dashboard,
} from '@/types/api';

export function useClientesApi() {
  return {
    listar: (filtro: Filtro = {}) =>
      api.get<Pagina<ClienteLista>>('/clientes', { params: filtro }).then((r) => r.data),

    obter: (id: number) =>
      api.get<Cliente>(`/clientes/${id}`).then((r) => r.data),

    criar: (dados: ClienteForm) =>
      api.post<Cliente>('/clientes', dados).then((r) => r.data),

    atualizar: (id: number, dados: ClienteForm) =>
      api.put<Cliente>(`/clientes/${id}`, dados).then((r) => r.data),

    inativar: (id: number) =>
      api.delete(`/clientes/${id}`).then(() => undefined),
  };
}

export function useFabricasApi() {
  return {
    listar: (filtro: Filtro = {}) =>
      api.get<Pagina<FabricaLista>>('/fabricas', { params: filtro }).then((r) => r.data),

    obter: (id: number) =>
      api.get<Fabrica>(`/fabricas/${id}`).then((r) => r.data),

    criar: (dados: FabricaForm) =>
      api.post<Fabrica>('/fabricas', dados).then((r) => r.data),

    atualizar: (id: number, dados: FabricaForm) =>
      api.put<Fabrica>(`/fabricas/${id}`, dados).then((r) => r.data),

    inativar: (id: number) =>
      api.delete(`/fabricas/${id}`).then(() => undefined),
  };
}

export function useProdutosApi() {
  return {
    listar: (filtro: Filtro & { fabricaId?: number } = {}) =>
      api.get<Pagina<ProdutoLista>>('/produtos', { params: filtro }).then((r) => r.data),

    obter: (id: number) =>
      api.get<Produto>(`/produtos/${id}`).then((r) => r.data),

    criar: (dados: ProdutoForm) =>
      api.post<Produto>('/produtos', dados).then((r) => r.data),

    atualizar: (id: number, dados: ProdutoForm) =>
      api.put<Produto>(`/produtos/${id}`, dados).then((r) => r.data),

    inativar: (id: number) =>
      api.delete(`/produtos/${id}`).then(() => undefined),

    adicionarPreco: (produtoId: number, dados: PrecoForm) =>
      api.post<Preco>(`/produtos/${produtoId}/precos`, dados).then((r) => r.data),

    atualizarPreco: (produtoId: number, precoId: number, dados: PrecoForm) =>
      api.put<Preco>(`/produtos/${produtoId}/precos/${precoId}`, dados).then((r) => r.data),

    removerPreco: (produtoId: number, precoId: number) =>
      api.delete(`/produtos/${produtoId}/precos/${precoId}`).then(() => undefined),
  };
}

export interface FiltroPedido extends Filtro {
  clienteId?: number;
  fabricaId?: number;
  status?: PedidoStatus;
  de?: string;
  ate?: string;
}

export function usePedidosApi() {
  return {
    listar: (filtro: FiltroPedido = {}) =>
      api.get<Pagina<PedidoLista>>('/pedidos', { params: filtro }).then((r) => r.data),

    obter: (id: number) =>
      api.get<Pedido>(`/pedidos/${id}`).then((r) => r.data),

    criar: (dados: PedidoForm) =>
      api.post<Pedido>('/pedidos', dados).then((r) => r.data),

    atualizar: (id: number, dados: PedidoForm) =>
      api.put<Pedido>(`/pedidos/${id}`, dados).then((r) => r.data),

    duplicar: (id: number) =>
      api.post<Pedido>(`/pedidos/${id}/duplicar`).then((r) => r.data),

    alterarStatus: (id: number, status: PedidoStatus) =>
      api.patch<Pedido>(`/pedidos/${id}/status`, { status }).then((r) => r.data),

    excluir: (id: number) =>
      api.delete(`/pedidos/${id}`).then(() => undefined),
  };
}

export function useDashboardApi() {
  return {
    obter: () => api.get<Dashboard>('/dashboard').then((r) => r.data),
  };
}

async function baixar(url: string, params?: Record<string, unknown>) {
  const resposta = await api.get(url, { params, responseType: 'blob' });

  const disposition = resposta.headers['content-disposition'] as string | undefined;
  const nome = disposition?.match(/filename="?([^"]+)"?/)?.[1] ?? 'download';

  const link = document.createElement('a');
  const href = URL.createObjectURL(resposta.data as Blob);
  link.href = href;
  link.download = nome;
  document.body.appendChild(link);
  link.click();
  link.remove();
  URL.revokeObjectURL(href);
}

export function useRelatoriosApi() {
  return {
    pedidoPdf: (id: number) => baixar(`/relatorios/pedido/${id}/pdf`),
    clientesExcel: () => baixar('/relatorios/clientes/excel'),
    produtosExcel: () => baixar('/relatorios/produtos/excel'),
    fabricasExcel: () => baixar('/relatorios/fabricas/excel'),
    pedidosExcel: (periodo?: { de?: string; ate?: string }) =>
      baixar('/relatorios/pedidos/excel', periodo),
    comissaoExcel: (periodo?: { de?: string; ate?: string }) =>
      baixar('/relatorios/comissao/excel', periodo),
  };
}

export interface DisponibilidadeEnvio {
  email: boolean;
  whatsApp: boolean;
}

export interface WhatsAppLink {
  link: string;
  mensagem: string;
  envieOPdfSeparadamente: boolean;
}

export function useEnvioApi() {
  return {
    disponibilidade: (pedidoId: number) =>
      api.get<DisponibilidadeEnvio>(`/pedidos/${pedidoId}/envio/disponibilidade`).then((r) => r.data),
    email: (pedidoId: number, destinatario: string, mensagem?: string) =>
      api.post(`/pedidos/${pedidoId}/envio/email`, { destinatario, mensagem }).then((r) => r.data),
    whatsapp: (pedidoId: number, telefone?: string) =>
      api.get<WhatsAppLink>(`/pedidos/${pedidoId}/envio/whatsapp`, { params: { telefone } }).then((r) => r.data),
  };
}

export function useLookupsApi() {
  return {
    cores: () => api.get<Lookup[]>('/lookups/cores').then((r) => r.data),
    tamanhos: () => api.get<Lookup[]>('/lookups/tamanhos').then((r) => r.data),
    estados: () => api.get<Lookup[]>('/lookups/estados').then((r) => r.data),
    cidades: (estadoId: number) =>
      api.get<Lookup[]>(`/lookups/estados/${estadoId}/cidades`).then((r) => r.data),
  };
}
