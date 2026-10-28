using PitVendasApi.Identity;

namespace PitVendasApi.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nome { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string? InscricaoEstadual { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Foto { get; set; }
        public string? Observacao { get; set; }
        public bool Ativo { get; set; } = true;

        public int? EnderecoId { get; set; }
        public Endereco? Endereco { get; set; }

        public string VendedorId { get; set; } = null!;
        public ApplicationUser Vendedor { get; set; } = null!;

        public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
    }
}
