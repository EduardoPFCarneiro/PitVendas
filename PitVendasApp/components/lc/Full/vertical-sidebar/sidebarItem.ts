export interface menu {
  header?: string;
  title?: string;
  icon?: any;
  to?: string;
  chip?: string;
  BgColor?: string;
  chipBgColor?: string;
  chipColor?: string;
  chipVariant?: string;
  chipIcon?: string;
  children?: menu[];
  disabled?: boolean;
  type?: string;
  subCaption?: string;
}

/** Menu conforme o prototipo: Inicio, Agentes, Catalogo e Vendas, Estatisticas. */
const sidebarItem: menu[] = [
  { header: "Início" },
  {
    title: "Página Inicial",
    icon: "widget-4-line-duotone",
    BgColor: "primary",
    to: "/",
  },

  { header: "Agentes" },
  {
    title: "Cliente",
    icon: "user-rounded-line-duotone",
    BgColor: "primary",
    to: "/clientes",
  },
  {
    title: "Fábrica",
    icon: "buildings-2-line-duotone",
    BgColor: "primary",
    to: "/fabricas",
  },

  { header: "Catálogo e Vendas" },
  {
    title: "Produto",
    icon: "box-line-duotone",
    BgColor: "primary",
    to: "/produtos",
  },
  {
    title: "Pedido",
    icon: "clipboard-list-line-duotone",
    BgColor: "primary",
    to: "/pedidos",
  },

  { header: "Estatísticas" },
  {
    title: "Relatório",
    icon: "chart-2-line-duotone",
    BgColor: "primary",
    to: "/relatorios",
  },
];

export default sidebarItem;
