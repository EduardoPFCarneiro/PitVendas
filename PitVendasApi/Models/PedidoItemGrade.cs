namespace PitVendasApi.Models
{
    public class PedidoItemGrade
    {
        public int Id { get; set; }

        public int Quantidade { get; set; }

        public string CodigoTamanho { get; set; } = null!;

        public int PedidoItemId { get; set; }
        public PedidoItem PedidoItem { get; set; } = null!;

        public int TamanhoId { get; set; }
        public Tamanho Tamanho { get; set; } = null!;
    }
}
