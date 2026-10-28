// ----------------------------- comuns -----------------------------

export interface Pagina<T> {
  itens: T[];
  total: number;
  pagina: number;
  tamanhoPagina: number;
  totalPaginas: number;
}

export interface Filtro {
  busca?: string;
  pagina?: number;
  tamanhoPagina?: number;
  ativo?: boolean | null;
}

export interface Lookup {
  id: number;
  nome: string;
}

export interface Endereco {
  id?: number | null;
  logradouro?: string | null;
  numero?: string | null;
  complemento?: string | null;
  bairro?: string | null;
  cep?: string | null;
  cidadeId?: number | null;
  cidadeNome?: string | null;
  estadoSigla?: string | null;
}

// ------------------------------ auth ------------------------------

export interface Usuario {
  id: string;
  nome: string;
  email?: string | null;
  telefone?: string | null;
  foto?: string | null;
}

export interface TokenResposta {
  token: string;
  expiraEm: string;
  usuario: Usuario;
}

export interface LoginPayload {
  email: string;
  senha: string;
}

export interface RegistroPayload {
  nome: string;
  email: string;
  telefone?: string | null;
  senha: string;
}

// ----------------------------- cliente -----------------------------

export interface ClienteLista {
  id: number;
  nome: string;
  razaoSocial: string;
  cnpj: string;
  telefone?: string | null;
  cidadeNome?: string | null;
  ativo: boolean;
}

export interface Cliente {
  id: number;
  nome: string;
  razaoSocial: string;
  cnpj: string;
  inscricaoEstadual?: string | null;
  email?: string | null;
  telefone?: string | null;
  foto?: string | null;
  observacao?: string | null;
  ativo: boolean;
  endereco?: Endereco | null;
}

export type ClienteForm = Omit<Cliente, 'id'>;

// ----------------------------- fabrica -----------------------------

export interface FabricaLista {
  id: number;
  nome: string;
  razaoSocial: string;
  cnpj: string;
  comissao: number;
  telefone?: string | null;
  ativo: boolean;
}

export interface Fabrica {
  id: number;
  nome: string;
  razaoSocial: string;
  cnpj: string;
  email?: string | null;
  telefone?: string | null;
  comissao: number;
  ativo: boolean;
  endereco?: Endereco | null;
}

export type FabricaForm = Omit<Fabrica, 'id'>;

// ----------------------------- produto -----------------------------

export interface ProdutoLista {
  id: number;
  codigo: string;
  descricao: string;
  fabricaNome: string;
  menorPreco?: number | null;
  ativo: boolean;
}

export interface Preco {
  id: number;
  codigo: string;
  valor: number;
  observacao?: string | null;
  dataAlteracao: string;
  ativo: boolean;
}

export interface PrecoForm {
  codigo: string;
  valor: number;
  observacao?: string | null;
  ativo: boolean;
}

export interface Produto {
  id: number;
  codigo: string;
  descricao: string;
  observacao?: string | null;
  foto?: string | null;
  ativo: boolean;
  fabricaId: number;
  fabricaNome: string;
  cores: Lookup[];
  tamanhos: Lookup[];
  precos: Preco[];
}

export interface ProdutoForm {
  codigo: string;
  descricao: string;
  observacao?: string | null;
  foto?: string | null;
  fabricaId: number;
  ativo: boolean;
  corIds: number[];
  tamanhoIds: number[];
}

// ----------------------------- pedido -----------------------------

export enum PedidoStatus {
  Rascunho = 0,
  Enviado = 1,
  Confirmado = 2,
  Faturado = 3,
  Cancelado = 4,
}

export const PEDIDO_STATUS_LABEL: Record<PedidoStatus, string> = {
  [PedidoStatus.Rascunho]: 'Rascunho',
  [PedidoStatus.Enviado]: 'Enviado',
  [PedidoStatus.Confirmado]: 'Confirmado',
  [PedidoStatus.Faturado]: 'Faturado',
  [PedidoStatus.Cancelado]: 'Cancelado',
};

export const PEDIDO_STATUS_COR: Record<PedidoStatus, string> = {
  [PedidoStatus.Rascunho]: 'grey',
  [PedidoStatus.Enviado]: 'info',
  [PedidoStatus.Confirmado]: 'primary',
  [PedidoStatus.Faturado]: 'success',
  [PedidoStatus.Cancelado]: 'error',
};

export interface PedidoLista {
  id: number;
  numero: number;
  data: string;
  clienteNome: string;
  fabricaNome: string;
  valorTotal: number;
  totalPecas: number;
  status: PedidoStatus;
  statusDescricao: string;
}

export interface Grade {
  tamanhoId: number;
  codigoTamanho: string;
  quantidade: number;
}

export interface PedidoItem {
  id: number;
  produtoId: number;
  codigoProduto: string;
  descricaoProduto: string;
  corId: number;
  nomeCor: string;
  valorUnitario: number;
  quantidade: number;
  subtotal: number;
  observacao?: string | null;
  grade: Grade[];
}

export interface Pedido {
  id: number;
  numero: number;
  data: string;

  dataFaturamento?: string | null;
  status: PedidoStatus;
  statusDescricao: string;
  clienteId: number;
  clienteNome: string;
  fabricaId: number;
  fabricaNome: string;

  tabelaPreco?: string | null;
  valorBruto: number;

  percentualDesconto: number;
  desconto: number;
  valorTotal: number;
  percentualComissao: number;
  valorComissao: number;
  totalPecas: number;
  prazo?: string | null;
  frete?: string | null;
  condicaoPagamento?: string | null;
  parcelas: number;
  observacao?: string | null;
  enviadoEm?: string | null;
  itens: PedidoItem[];
}

export interface GradeForm {
  tamanhoId: number;
  quantidade: number;
}

export interface PedidoItemForm {
  produtoId: number;
  corId: number;
  valorUnitario?: number | null;
  observacao?: string | null;
  grade: GradeForm[];
}

export interface PedidoForm {
  clienteId: number;
  fabricaId: number;
  data?: string | null;
  dataFaturamento?: string | null;
  tabelaPreco?: string | null;

  percentualDesconto: number;
  prazo?: string | null;
  frete?: string | null;
  condicaoPagamento?: string | null;
  parcelas: number;
  observacao?: string | null;
  itens: PedidoItemForm[];
}

// ---------------------------- dashboard ----------------------------

export interface RankingFabrica {
  fabricaId: number;
  nome: string;
  quantidadePedidos: number;
  valorTotal: number;
  comissao: number;
}

export interface PedidosPorMes {
  mes: number;
  quantidade: number;
  valor: number;
}

export interface Dashboard {
  totalPedidos: number;
  pedidosNoMes: number;
  comissaoNoMes: number;
  comissaoNoAno: number;
  totalClientes: number;
  totalFabricas: number;
  rankingFabricas: RankingFabrica[];
  pedidosPorMes: PedidosPorMes[];
}

// ------------------------------ erros ------------------------------

export interface ProblemDetails {
  title?: string;
  status?: number;
  detail?: string;
  errors?: Record<string, string[]>;
}
