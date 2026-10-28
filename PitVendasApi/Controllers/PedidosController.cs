using AutoMapper;
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
    [Route("api/[controller]")]
    [Authorize]
    public class PedidosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IPedidoService _pedidos;

        public PedidosController(AppDbContext db, IMapper mapper, IPedidoService pedidos)
        {
            _db = db;
            _mapper = mapper;
            _pedidos = pedidos;
        }

        [HttpGet]
        public async Task<ActionResult<PaginaDto<PedidoListaDto>>> Listar(
            [FromQuery] FiltroDto filtro,
            [FromQuery] int? clienteId,
            [FromQuery] int? fabricaId,
            [FromQuery] PedidoStatus? status,
            [FromQuery] DateTime? de,
            [FromQuery] DateTime? ate,
            CancellationToken ct)
        {
            var query = _db.Pedidos.AsNoTracking();

            if (clienteId is not null) query = query.Where(p => p.ClienteId == clienteId);
            if (fabricaId is not null) query = query.Where(p => p.FabricaId == fabricaId);
            if (status is not null) query = query.Where(p => p.Status == status);
            if (de is not null) query = query.Where(p => p.Data >= de);
            if (ate is not null) query = query.Where(p => p.Data <= ate);

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                var busca = filtro.Busca.Trim();
                query = query.Where(p =>
                    p.Cliente.Nome.Contains(busca) ||
                    p.Fabrica.Nome.Contains(busca));
            }

            var total = await query.CountAsync(ct);

            var itens = await query
                .OrderByDescending(p => p.Data)
                .ThenByDescending(p => p.Numero)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .Select(p => new PedidoListaDto
                {
                    Id = p.Id,
                    Numero = p.Numero,
                    Data = p.Data,
                    ClienteNome = p.Cliente.Nome,
                    FabricaNome = p.Fabrica.Nome,
                    ValorTotal = p.ValorTotal,
                    TotalPecas = p.TotalPecas,
                    Status = p.Status
                })
                .ToListAsync(ct);

            return Ok(new PaginaDto<PedidoListaDto>
            {
                Itens = itens,
                Total = total,
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PedidoDto>> Obter(int id, CancellationToken ct)
        {
            var pedido = await CarregarCompletoAsync(id, ct);
            return pedido is null ? NotFound() : Ok(Montar(pedido));
        }

        [HttpPost]
        public async Task<ActionResult<PedidoDto>> Criar(PedidoFormDto dto, CancellationToken ct)
        {
            var pedido = await _pedidos.CriarAsync(dto, ct);
            var completo = await CarregarCompletoAsync(pedido.Id, ct);

            return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, Montar(completo!));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<PedidoDto>> Atualizar(int id, PedidoFormDto dto, CancellationToken ct)
        {
            await _pedidos.AtualizarAsync(id, dto, ct);
            var completo = await CarregarCompletoAsync(id, ct);

            return Ok(Montar(completo!));
        }

        [HttpPost("{id:int}/duplicar")]
        public async Task<ActionResult<PedidoDto>> Duplicar(int id, CancellationToken ct)
        {
            var pedido = await _pedidos.DuplicarAsync(id, ct);
            var completo = await CarregarCompletoAsync(pedido.Id, ct);

            return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, Montar(completo!));
        }

        [HttpPatch("{id:int}/status")]
        public async Task<ActionResult<PedidoDto>> AlterarStatus(
            int id, AlterarStatusDto dto, CancellationToken ct)
        {
            await _pedidos.AlterarStatusAsync(id, dto.Status, ct);
            var completo = await CarregarCompletoAsync(id, ct);

            return Ok(Montar(completo!));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Excluir(int id, CancellationToken ct)
        {
            var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (pedido is null) return NotFound();

            if (pedido.Status != PedidoStatus.Rascunho)
            {
                return BadRequest(new
                {
                    mensagem = "Apenas rascunhos podem ser excluidos. " +
                               "Para os demais, altere o status para Cancelado."
                });
            }

            _db.Pedidos.Remove(pedido);
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        // ------------------------------------------------------------

        private Task<Pedido?> CarregarCompletoAsync(int id, CancellationToken ct) =>
            _db.Pedidos
                .AsNoTracking()
                .Include(p => p.Cliente)
                .Include(p => p.Fabrica)
                .Include(p => p.Itens).ThenInclude(i => i.Grade).ThenInclude(g => g.Tamanho)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        private PedidoDto Montar(Pedido pedido)
        {
            var dto = _mapper.Map<PedidoDto>(pedido);

            foreach (var item in dto.Itens)
            {
                var origem = pedido.Itens.First(i => i.Id == item.Id);
                item.Grade = origem.Grade
                    .OrderBy(g => g.Tamanho.Ordem)
                    .Select(g => new GradeDto
                    {
                        TamanhoId = g.TamanhoId,
                        CodigoTamanho = g.CodigoTamanho,
                        Quantidade = g.Quantidade
                    })
                    .ToList();
            }

            return dto;
        }
    }
}
