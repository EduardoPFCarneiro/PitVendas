namespace PitVendasApi.Models
{
    public class ProdutoCor
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public int CorId { get; set; }
        public Cor Cor { get; set; } = null!;
    }
}
