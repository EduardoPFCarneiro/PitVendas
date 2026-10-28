using System.ComponentModel.DataAnnotations;

namespace PitVendasApi.Dtos
{
    public class EnviarEmailDto
    {
        [Required(ErrorMessage = "Informe o destinatário.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string Destinatario { get; set; } = null!;

        public string? Mensagem { get; set; }
    }

    public class WhatsAppLinkDto
    {
        public string Link { get; set; } = null!;
        public string Mensagem { get; set; } = null!;
        public bool EnvieOPdfSeparadamente { get; set; }
    }

    public class DisponibilidadeEnvioDto
    {
        public bool Email { get; set; }
        public bool WhatsApp { get; set; } = true;
    }
}
