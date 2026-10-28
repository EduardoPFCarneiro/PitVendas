using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitVendasApi.DataContexts;
using PitVendasApi.Dtos;

namespace PitVendasApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LookupsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LookupsController(AppDbContext db) => _db = db;

        [HttpGet("cores")]
        public async Task<ActionResult<IEnumerable<LookupDto>>> Cores(CancellationToken ct)
        {
            var cores = await _db.Cores
                .OrderBy(c => c.Nome)
                .Select(c => new LookupDto { Id = c.Id, Nome = c.Nome })
                .ToListAsync(ct);

            return Ok(cores);
        }

        [HttpGet("tamanhos")]
        public async Task<ActionResult<IEnumerable<LookupDto>>> Tamanhos(CancellationToken ct)
        {
            var tamanhos = await _db.Tamanhos
                .OrderBy(t => t.Ordem)
                .Select(t => new LookupDto { Id = t.Id, Nome = t.Codigo })
                .ToListAsync(ct);

            return Ok(tamanhos);
        }

        [HttpGet("estados")]
        public async Task<ActionResult<IEnumerable<LookupDto>>> Estados(CancellationToken ct)
        {
            var estados = await _db.Estados
                .OrderBy(e => e.Nome)
                .Select(e => new LookupDto { Id = e.Id, Nome = e.Nome })
                .ToListAsync(ct);

            return Ok(estados);
        }

        [HttpGet("estados/{estadoId:int}/cidades")]
        public async Task<ActionResult<IEnumerable<LookupDto>>> Cidades(int estadoId, CancellationToken ct)
        {
            var cidades = await _db.Cidades
                .Where(c => c.EstadoId == estadoId)
                .OrderBy(c => c.Nome)
                .Select(c => new LookupDto { Id = c.Id, Nome = c.Nome })
                .ToListAsync(ct);

            return Ok(cidades);
        }
    }
}
