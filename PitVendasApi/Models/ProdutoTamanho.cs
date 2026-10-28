namespace PitVendasApi.Models
{
    public class ProdutoTamanho
    {
        public int Id { get; set; }

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public int TamanhoId { get; set; }
        public Tamanho Tamanho { get; set; } = null!;
    }
}
