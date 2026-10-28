namespace PitVendasApi.Models
{
    public class Cidade
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;

        public int EstadoId { get; set; }
        public Estado Estado { get; set; } = null!;
    }
}
