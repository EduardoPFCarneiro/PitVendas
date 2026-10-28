using System.ComponentModel.DataAnnotations;

namespace PitVendasApi.Dtos
{
    public class RegistroDto
    {
        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(200)]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        public string Email { get; set; } = null!;

        [Phone(ErrorMessage = "Telefone invalido.")]
        public string? Telefone { get; set; }

        [Required(ErrorMessage = "Informe a senha.")]
        [MinLength(8, ErrorMessage = "A senha deve ter ao menos 8 caracteres.")]
        public string Senha { get; set; } = null!;
    }

    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Senha { get; set; } = null!;
    }

    public class TokenDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiraEm { get; set; }
        public UsuarioDto Usuario { get; set; } = null!;
    }

    public class UsuarioDto
    {
        public string Id { get; set; } = null!;
        public string Nome { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Foto { get; set; }
    }
}
