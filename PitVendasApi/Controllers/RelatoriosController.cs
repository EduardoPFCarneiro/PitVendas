using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitVendasApi.DataContexts;
using PitVendasApi.Models;
using PitVendasApi.Services;

namespace PitVendasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RelatoriosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPedidoPdfService _pdf;
        private readonly IExcelService _excel;

        public RelatoriosController(AppDbContext db, IPedidoPdfService pdf, IExcelService excel)
        {
            _db = db;
            _pdf = pdf;
            _excel = excel;
        }

        // ------------------------- PDF do pedido -------------------------

        [HttpGet("pedido/{id:int}/pdf")]
        public async Task<IActionResult> PedidoPdf(int id, CancellationToken ct)
        {
            var pedido = await CarregarPedidoAsync(id, ct);
            if (pedido is null) return NotFound();

            var bytes = _pdf.Gerar(pedido);
            return File(bytes, "application/pdf", $"pedido-{pedido.Numero:D4}.pdf");
        }

        // ------------------------- Excel de listagens -------------------------

        [HttpGet("clientes/excel")]
        public async Task<IActionResult> ClientesExcel(CancellationToken ct)
        {
            var dados = await _db.Clientes
                .AsNoTracking()
                .OrderBy(c => c.Nome)
                .Select(c => new object?[]
                {
                    c.Nome, c.RazaoSocial, c.Cnpj, c.Email, c.Telefone,
                    c.Endereco != null ? c.Endereco.Cidade!.Nome : "",
                    c.Ativo,
                })
                .ToListAsync(ct);

            var bytes = _excel.Gerar("Clientes",
                new[] { "Nome", "Razão Social", "CNPJ", "E-mail", "Telefone", "Cidade", "Ativo" },
                dados);

            return Planilha(bytes, "clientes");
        }

        [HttpGet("produtos/excel")]
        public async Task<IActionResult> ProdutosExcel(CancellationToken ct)
        {
            var dados = await _db.Produtos
                .AsNoTracking()
                .OrderBy(p => p.Descricao)
                .Select(p => new object?[]
                {
                    p.Codigo, p.Descricao, p.Fabrica.Nome,
                    p.Precos.Where(x => x.Ativo).Min(x => (decimal?)x.Valor),
                    p.Ativo,
                })
                .ToListAsync(ct);

            var bytes = _excel.Gerar("Produtos",
                new[] { "Código", "Descrição", "Fábrica", "Menor Preço", "Ativo" },
                dados);

            return Planilha(bytes, "produtos");
        }

        [HttpGet("fabricas/excel")]
        public async Task<IActionResult> FabricasExcel(CancellationToken ct)
        {
            var dados = await _db.Fabricas
                .AsNoTracking()
                .OrderBy(f => f.Nome)
                .Select(f => new object?[]
                {
                    f.Nome, f.RazaoSocial, f.Cnpj, f.Comissao, f.Email, f.Telefone, f.Ativo,
                })
                .ToListAsync(ct);

            var bytes = _excel.Gerar("Fábricas",
                new[] { "Nome", "Razão Social", "CNPJ", "Comissão (%)", "E-mail", "Telefone", "Ativo" },
                dados);

            return Planilha(bytes, "fabricas");
        }

        [HttpGet("pedidos/excel")]
        public async Task<IActionResult> PedidosExcel(
            [FromQuery] DateTime? de, [FromQuery] DateTime? ate, CancellationToken ct)
        {
            var query = _db.Pedidos.AsNoTracking().Where(p => p.Status != PedidoStatus.Cancelado);
            if (de is not null) query = query.Where(p => p.Data >= de);
            if (ate is not null) query = query.Where(p => p.Data <= ate);

            var dados = await query
                .OrderByDescending(p => p.Data)
                .Select(p => new object?[]
                {
                    p.Numero, p.Data, p.Cliente.Nome, p.Fabrica.Nome,
                    p.TotalPecas, p.ValorBruto, p.Desconto, p.ValorTotal,
                    p.ValorComissao, p.Status.ToString(),
                })
                .ToListAsync(ct);

            var bytes = _excel.Gerar("Pedidos",
                new[] { "Nº", "Data", "Cliente", "Fábrica", "Peças", "Bruto", "Desconto", "Líquido", "Comissão", "Status" },
                dados);

            return Planilha(bytes, "pedidos");
        }

        // ------------------------- Comissão / Faturamento -------------------------

        [HttpGet("comissao/excel")]
        public async Task<IActionResult> ComissaoExcel(
            [FromQuery] DateTime? de, [FromQuery] DateTime? ate, CancellationToken ct)
        {
            var query = _db.Pedidos.AsNoTracking().Where(p => p.Status != PedidoStatus.Cancelado);
            if (de is not null) query = query.Where(p => p.Data >= de);
            if (ate is not null) query = query.Where(p => p.Data <= ate);

            var dados = await query
                .GroupBy(p => p.Fabrica.Nome)
                .Select(g => new object?[]
                {
                    g.Key,
                    g.Count(),
                    g.Sum(p => p.ValorTotal),
                    g.Sum(p => p.ValorComissao),
                })
                .OrderByDescending(x => (decimal)x[3]!)
                .ToListAsync(ct);

            var bytes = _excel.Gerar("Comissões por Fábrica",
                new[] { "Fábrica", "Pedidos", "Faturamento", "Comissão" },
                dados);

            return Planilha(bytes, "comissoes");
        }

        // ------------------------------------------------------------

        private Task<Pedido?> CarregarPedidoAsync(int id, CancellationToken ct) =>
            _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Fabrica)
                .Include(p => p.Itens).ThenInclude(i => i.Grade).ThenInclude(g => g.Tamanho)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        private IActionResult Planilha(byte[] bytes, string nome) =>
            File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{nome}-{DateTime.Now:yyyyMMdd}.xlsx");
    }
}
