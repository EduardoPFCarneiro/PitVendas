using PitVendasApi.Identity;

namespace PitVendasApi.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public DateTime Data { get; set; }

        public DateOnly? DataFaturamento { get; set; }

        public PedidoStatus Status { get; set; } = PedidoStatus.Rascunho;

        public string? TabelaPreco { get; set; }

        public decimal ValorBruto { get; set; }

        public decimal PercentualDesconto { get; set; }

        public decimal Desconto { get; set; }

        public decimal ValorTotal { get; set; }

        public decimal PercentualComissao { get; set; }
        public decimal ValorComissao { get; set; }

        public int TotalPecas { get; set; }

        public string? Prazo { get; set; }
        public string? Frete { get; set; }
        public string? CondicaoPagamento { get; set; }
        public int Parcelas { get; set; } = 1;
        public string? Observacao { get; set; }

        public DateTime? EnviadoEm { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public int FabricaId { get; set; }
        public Fabrica Fabrica { get; set; } = null!;

        public string VendedorId { get; set; } = null!;
        public ApplicationUser Vendedor { get; set; } = null!;

        public ICollection<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
    }
}
