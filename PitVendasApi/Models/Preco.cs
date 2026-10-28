namespace PitVendasApi.Models
{
    public class Preco
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = null!;
        public decimal Valor { get; set; }
        public string? Observacao { get; set; }
        public DateOnly DataAlteracao { get; set; }
        public bool Ativo { get; set; } = true;

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;
    }
}
