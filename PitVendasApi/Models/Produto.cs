namespace PitVendasApi.Models
{
    public class Produto
    {
        public int Id { get; set; }

        public string Codigo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public string? Observacao { get; set; }
        public string? Foto { get; set; }
        public bool Ativo { get; set; } = true;

        public int FabricaId { get; set; }
        public Fabrica Fabrica { get; set; } = null!;

        public ICollection<ProdutoCor> Cores { get; set; } = new List<ProdutoCor>();
        public ICollection<ProdutoTamanho> Tamanhos { get; set; } = new List<ProdutoTamanho>();
        public ICollection<Preco> Precos { get; set; } = new List<Preco>();
    }
}
