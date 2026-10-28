using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitVendasApi.DataContexts;
using PitVendasApi.Dtos;
using PitVendasApi.Models;

namespace PitVendasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _db;

        public DashboardController(AppDbContext db) => _db = db;

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> Obter(CancellationToken ct)
        {
            var hoje = DateTime.Today;
            var inicioMes = new DateTime(hoje.Year, hoje.Month, 1);
            var inicioAno = new DateTime(hoje.Year, 1, 1);

            var pedidos = _db.Pedidos.Where(p => p.Status != PedidoStatus.Cancelado);

            var dto = new DashboardDto
            {
                TotalPedidos = await pedidos.CountAsync(ct),
                PedidosNoMes = await pedidos.CountAsync(p => p.Data >= inicioMes, ct),
                ComissaoNoMes = await pedidos
                    .Where(p => p.Data >= inicioMes)
                    .SumAsync(p => (decimal?)p.ValorComissao, ct) ?? 0,
                ComissaoNoAno = await pedidos
                    .Where(p => p.Data >= inicioAno)
                    .SumAsync(p => (decimal?)p.ValorComissao, ct) ?? 0,
                TotalClientes = await _db.Clientes.CountAsync(c => c.Ativo, ct),
                TotalFabricas = await _db.Fabricas.CountAsync(f => f.Ativo, ct),
            };

            dto.RankingFabricas = await pedidos
                .GroupBy(p => new { p.FabricaId, p.Fabrica.Nome })
                .Select(g => new RankingFabricaDto
                {
                    FabricaId = g.Key.FabricaId,
                    Nome = g.Key.Nome,
                    QuantidadePedidos = g.Count(),
                    ValorTotal = g.Sum(p => p.ValorTotal),
                    Comissao = g.Sum(p => p.ValorComissao),
                })
                .OrderByDescending(x => x.QuantidadePedidos)
                .Take(5)
                .ToListAsync(ct);

            var porMes = await pedidos
                .Where(p => p.Data >= inicioAno)
                .GroupBy(p => p.Data.Month)
                .Select(g => new PedidosPorMesDto
                {
                    Mes = g.Key,
                    Quantidade = g.Count(),
                    Valor = g.Sum(p => p.ValorTotal),
                })
                .ToListAsync(ct);

            dto.PedidosPorMes = Enumerable.Range(1, 12)
                .Select(mes => porMes.FirstOrDefault(x => x.Mes == mes)
                               ?? new PedidosPorMesDto { Mes = mes })
                .ToList();

            return Ok(dto);
        }
    }
}
