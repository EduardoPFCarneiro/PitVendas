using System.ComponentModel.DataAnnotations;

namespace PitVendasApi.Dtos
{
    public class EsqueciSenhaDto
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        public string Email { get; set; } = null!;
    }

    public class RedefinirSenhaDto
    {
        [Required]
        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Token de recuperacao ausente.")]
        public string Token { get; set; } = null!;

        [Required(ErrorMessage = "Informe a nova senha.")]
        [MinLength(8, ErrorMessage = "A senha deve ter ao menos 8 caracteres.")]
        public string NovaSenha { get; set; } = null!;
    }

    public class EsqueciSenhaRespostaDto
    {
        public string Mensagem { get; set; } = null!;

        public string? LinkDesenvolvimento { get; set; }
    }
}
