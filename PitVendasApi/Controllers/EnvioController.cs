using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitVendasApi.DataContexts;
using PitVendasApi.Dtos;
using PitVendasApi.Models;
using PitVendasApi.Services;

namespace PitVendasApi.Controllers
{
    [ApiController]
    [Route("api/pedidos/{id:int}/envio")]
    [Authorize]
    public class EnvioController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPedidoPdfService _pdf;
        private readonly IEmailService _email;

        public EnvioController(AppDbContext db, IPedidoPdfService pdf, IEmailService email)
        {
            _db = db;
            _pdf = pdf;
            _email = email;
        }

        [HttpGet("disponibilidade")]
        public ActionResult<DisponibilidadeEnvioDto> Disponibilidade() =>
            Ok(new DisponibilidadeEnvioDto { Email = _email.Disponivel, WhatsApp = true });

        [HttpPost("email")]
        public async Task<IActionResult> Email(int id, EnviarEmailDto dto, CancellationToken ct)
        {
            var pedido = await CarregarAsync(id, ct);
            if (pedido is null) return NotFound();

            var pdf = _pdf.Gerar(pedido);

            var corpo = MontarCorpoEmail(pedido, dto.Mensagem);
            await _email.EnviarAsync(dto.Destinatario,
                $"Pedido nº {pedido.Numero:D4} — {pedido.Fabrica.Nome}",
                corpo,
                new AnexoEmail
                {
                    Nome = $"pedido-{pedido.Numero:D4}.pdf",
                    Conteudo = pdf,
                    TipoMime = "application/pdf",
                },
                ct);

            return Ok(new { mensagem = "Pedido enviado por e-mail." });
        }

        [HttpGet("whatsapp")]
        public async Task<ActionResult<WhatsAppLinkDto>> WhatsApp(
            int id, [FromQuery] string? telefone, CancellationToken ct)
        {
            var pedido = await CarregarAsync(id, ct);
            if (pedido is null) return NotFound();

            var texto = MontarMensagemWhatsApp(pedido);
            var numero = new string((telefone ?? pedido.Cliente.Telefone ?? "")
                .Where(char.IsDigit).ToArray());

            if (numero.Length is 10 or 11) numero = "55" + numero;

            var link = string.IsNullOrEmpty(numero)
                ? $"https://wa.me/?text={Uri.EscapeDataString(texto)}"
                : $"https://wa.me/{numero}?text={Uri.EscapeDataString(texto)}";

            return Ok(new WhatsAppLinkDto
            {
                Link = link,
                Mensagem = texto,
                EnvieOPdfSeparadamente = true,
            });
        }

        // ------------------------------------------------------------

        private Task<Pedido?> CarregarAsync(int id, CancellationToken ct) =>
            _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Fabrica)
                .Include(p => p.Itens).ThenInclude(i => i.Grade).ThenInclude(g => g.Tamanho)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        private static string MontarCorpoEmail(Pedido pedido, string? mensagem)
        {
            var extra = string.IsNullOrWhiteSpace(mensagem)
                ? ""
                : $"<p>{System.Net.WebUtility.HtmlEncode(mensagem)}</p>";

            return $"""
                <div style="font-family: Arial, sans-serif; color:#2A3547;">
                  <h2 style="color:#0d86d9;">Pedido nº {pedido.Numero:D4}</h2>
                  <p>Olá, {System.Net.WebUtility.HtmlEncode(pedido.Cliente.Nome)}.</p>
                  <p>Segue em anexo o pedido referente à fábrica
                     <strong>{System.Net.WebUtility.HtmlEncode(pedido.Fabrica.Nome)}</strong>.</p>
                  {extra}
                  <ul>
                    <li><strong>Total de peças:</strong> {pedido.TotalPecas}</li>
                    <li><strong>Valor líquido:</strong> {Moeda(pedido.ValorTotal)}</li>
                  </ul>
                  <p style="color:#707a82; font-size:12px;">Enviado pelo sistema Pit Vendas.</p>
                </div>
                """;
        }

        private static string MontarMensagemWhatsApp(Pedido pedido)
        {
            return
                $"*Pedido nº {pedido.Numero:D4}*\n" +
                $"Fábrica: {pedido.Fabrica.Nome}\n" +
                $"Cliente: {pedido.Cliente.Nome}\n" +
                $"Peças: {pedido.TotalPecas}\n" +
                $"Total: {Moeda(pedido.ValorTotal)}\n\n" +
                "O PDF do pedido segue em anexo.";
        }

        private static string Moeda(decimal v) =>
            v.ToString("C", new CultureInfo("pt-BR"));
    }
}
