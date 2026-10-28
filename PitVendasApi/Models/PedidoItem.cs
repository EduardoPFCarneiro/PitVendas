namespace PitVendasApi.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }

        public string DescricaoProduto { get; set; } = null!;
        public string CodigoProduto { get; set; } = null!;
        public string NomeCor { get; set; } = null!;

        public decimal ValorUnitario { get; set; }

        public int Quantidade { get; set; }
        public decimal Subtotal { get; set; }

        public string? Observacao { get; set; }

        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public int CorId { get; set; }
        public Cor Cor { get; set; } = null!;

        public ICollection<PedidoItemGrade> Grade { get; set; } = new List<PedidoItemGrade>();
    }
}
