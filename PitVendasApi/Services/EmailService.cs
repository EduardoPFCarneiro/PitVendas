using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace PitVendasApi.Services
{
    public class EmailSettings
    {
        public const string SectionName = "Email";

        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string Usuario { get; set; } = "";
        public string Senha { get; set; } = "";
        public string RemetenteNome { get; set; } = "Pit Vendas";
        public string RemetenteEmail { get; set; } = "";

        public bool Configurado =>
            !string.IsNullOrWhiteSpace(Host) && !string.IsNullOrWhiteSpace(RemetenteEmail);
    }

    public class AnexoEmail
    {
        public string Nome { get; set; } = "";
        public byte[] Conteudo { get; set; } = Array.Empty<byte>();
        public string TipoMime { get; set; } = "application/octet-stream";
    }

    public interface IEmailService
    {
        bool Disponivel { get; }
        Task EnviarAsync(string destinatario, string assunto, string corpoHtml,
            AnexoEmail? anexo = null, CancellationToken ct = default);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public bool Disponivel => _settings.Configurado;

        public async Task EnviarAsync(string destinatario, string assunto, string corpoHtml,
            AnexoEmail? anexo = null, CancellationToken ct = default)
        {
            if (!Disponivel)
                throw new RegraNegocioException(
                    "O envio de e-mail não está configurado neste ambiente. " +
                    "Configure as credenciais SMTP para habilitar.");

            var mensagem = new MimeMessage();
            mensagem.From.Add(new MailboxAddress(_settings.RemetenteNome, _settings.RemetenteEmail));
            mensagem.To.Add(MailboxAddress.Parse(destinatario));
            mensagem.Subject = assunto;

            var corpo = new BodyBuilder { HtmlBody = corpoHtml };
            if (anexo is not null)
            {
                corpo.Attachments.Add(anexo.Nome, anexo.Conteudo,
                    ContentType.Parse(anexo.TipoMime));
            }
            mensagem.Body = corpo.ToMessageBody();

            using var cliente = new SmtpClient();
            try
            {
                await cliente.ConnectAsync(_settings.Host, _settings.Port,
                    SecureSocketOptions.StartTls, ct);
                await cliente.AuthenticateAsync(_settings.Usuario, _settings.Senha, ct);
                await cliente.SendAsync(mensagem, ct);
                await cliente.DisconnectAsync(true, ct);
            }
            catch (Exception e) when (e is not RegraNegocioException)
            {
                _logger.LogError(e, "Falha ao enviar e-mail para {Destinatario}", destinatario);
                throw new RegraNegocioException(
                    "Não foi possível enviar o e-mail. Verifique as credenciais e tente novamente.");
            }
        }
    }
}
