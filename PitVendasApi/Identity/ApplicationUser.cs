using Microsoft.AspNetCore.Identity;

namespace PitVendasApi.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; } = null!;
        public string? Foto { get; set; }
        public bool Ativo { get; set; } = true;
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    }
}
