using AutoMapper;
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
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ProdutosController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginaDto<ProdutoListaDto>>> Listar(
            [FromQuery] FiltroDto filtro, [FromQuery] int? fabricaId, CancellationToken ct)
        {
            var query = _db.Produtos.AsNoTracking();

            if (fabricaId is not null)
                query = query.Where(p => p.FabricaId == fabricaId);

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                var busca = filtro.Busca.Trim();
                query = query.Where(p => p.Codigo.Contains(busca) || p.Descricao.Contains(busca));
            }

            if (filtro.Ativo is not null)
                query = query.Where(p => p.Ativo == filtro.Ativo);

            var total = await query.CountAsync(ct);

            var itens = await query
                .OrderBy(p => p.Descricao)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .Select(p => new ProdutoListaDto
                {
                    Id = p.Id,
                    Codigo = p.Codigo,
                    Descricao = p.Descricao,
                    FabricaNome = p.Fabrica.Nome,
                    MenorPreco = p.Precos.Where(x => x.Ativo).Min(x => (decimal?)x.Valor),
                    Ativo = p.Ativo
                })
                .ToListAsync(ct);

            return Ok(new PaginaDto<ProdutoListaDto>
            {
                Itens = itens,
                Total = total,
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProdutoDto>> Obter(int id, CancellationToken ct)
        {
            var produto = await CarregarCompletoAsync(id, ct);
            return produto is null ? NotFound() : Ok(_mapper.Map<ProdutoDto>(produto));
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDto>> Criar(ProdutoFormDto dto, CancellationToken ct)
        {
            var fabrica = await _db.Fabricas.FirstOrDefaultAsync(f => f.Id == dto.FabricaId, ct);
            if (fabrica is null)
                return BadRequest(new { mensagem = "Fabrica nao encontrada." });

            if (await _db.Produtos.AnyAsync(p => p.FabricaId == dto.FabricaId && p.Codigo == dto.Codigo, ct))
                return Conflict(new { mensagem = "Esta fabrica ja tem um produto com este codigo." });

            var erro = await ValidarCoresETamanhosAsync(dto, ct);
            if (erro is not null) return BadRequest(new { mensagem = erro });

            var produto = new Produto { FabricaId = dto.FabricaId };
            AplicarForm(produto, dto);

            _db.Produtos.Add(produto);
            await _db.SaveChangesAsync(ct);

            var completo = await CarregarCompletoAsync(produto.Id, ct);
            return CreatedAtAction(nameof(Obter), new { id = produto.Id },
                _mapper.Map<ProdutoDto>(completo!));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ProdutoDto>> Atualizar(int id, ProdutoFormDto dto, CancellationToken ct)
        {
            var produto = await _db.Produtos
                .Include(p => p.Cores)
                .Include(p => p.Tamanhos)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

            if (produto is null) return NotFound();

            if (produto.FabricaId != dto.FabricaId)
                return BadRequest(new { mensagem = "Nao e possivel mover um produto para outra fabrica." });

            if (await _db.Produtos.AnyAsync(
                    p => p.FabricaId == dto.FabricaId && p.Codigo == dto.Codigo && p.Id != id, ct))
            {
                return Conflict(new { mensagem = "Esta fabrica ja tem outro produto com este codigo." });
            }

            var erro = await ValidarCoresETamanhosAsync(dto, ct);
            if (erro is not null) return BadRequest(new { mensagem = erro });

            _db.ProdutoCores.RemoveRange(produto.Cores);
            _db.ProdutoTamanhos.RemoveRange(produto.Tamanhos);
            produto.Cores.Clear();
            produto.Tamanhos.Clear();

            AplicarForm(produto, dto);
            await _db.SaveChangesAsync(ct);

            var completo = await CarregarCompletoAsync(id, ct);
            return Ok(_mapper.Map<ProdutoDto>(completo!));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Inativar(int id, CancellationToken ct)
        {
            var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (produto is null) return NotFound();

            produto.Ativo = false;
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        // --------------------- tabelas de preco ---------------------

        [HttpPost("{id:int}/precos")]
        public async Task<ActionResult<PrecoDto>> AdicionarPreco(int id, PrecoFormDto dto, CancellationToken ct)
        {
            var produto = await _db.Produtos.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (produto is null) return NotFound();

            if (await _db.Precos.AnyAsync(p => p.ProdutoId == id && p.Codigo == dto.Codigo, ct))
                return Conflict(new { mensagem = "Este produto ja tem uma tabela de preco com este codigo." });

            var preco = new Preco
            {
                ProdutoId = id,
                Codigo = dto.Codigo,
                Valor = dto.Valor,
                Observacao = dto.Observacao,
                Ativo = dto.Ativo,
                DataAlteracao = DateOnly.FromDateTime(DateTime.Today)
            };

            _db.Precos.Add(preco);
            await _db.SaveChangesAsync(ct);

            return Ok(_mapper.Map<PrecoDto>(preco));
        }

        [HttpPut("{id:int}/precos/{precoId:int}")]
        public async Task<ActionResult<PrecoDto>> AtualizarPreco(
            int id, int precoId, PrecoFormDto dto, CancellationToken ct)
        {
            var preco = await _db.Precos
                .FirstOrDefaultAsync(p => p.Id == precoId && p.ProdutoId == id, ct);

            if (preco is null) return NotFound();

            preco.Codigo = dto.Codigo;
            preco.Valor = dto.Valor;
            preco.Observacao = dto.Observacao;
            preco.Ativo = dto.Ativo;
            preco.DataAlteracao = DateOnly.FromDateTime(DateTime.Today);

            await _db.SaveChangesAsync(ct);
            return Ok(_mapper.Map<PrecoDto>(preco));
        }

        [HttpDelete("{id:int}/precos/{precoId:int}")]
        public async Task<IActionResult> RemoverPreco(int id, int precoId, CancellationToken ct)
        {
            var preco = await _db.Precos
                .FirstOrDefaultAsync(p => p.Id == precoId && p.ProdutoId == id, ct);

            if (preco is null) return NotFound();

            _db.Precos.Remove(preco);
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        // ------------------------------------------------------------

        private Task<Produto?> CarregarCompletoAsync(int id, CancellationToken ct) =>
            _db.Produtos
                .AsNoTracking()
                .Include(p => p.Fabrica)
                .Include(p => p.Cores).ThenInclude(c => c.Cor)
                .Include(p => p.Tamanhos).ThenInclude(t => t.Tamanho)
                .Include(p => p.Precos)
                .FirstOrDefaultAsync(p => p.Id == id, ct);

        private async Task<string?> ValidarCoresETamanhosAsync(ProdutoFormDto dto, CancellationToken ct)
        {
            var coresValidas = await _db.Cores
                .CountAsync(c => dto.CorIds.Contains(c.Id), ct);

            if (coresValidas != dto.CorIds.Distinct().Count())
                return "Uma ou mais cores informadas nao existem.";

            var tamanhosValidos = await _db.Tamanhos
                .CountAsync(t => dto.TamanhoIds.Contains(t.Id), ct);

            if (tamanhosValidos != dto.TamanhoIds.Distinct().Count())
                return "Um ou mais tamanhos informados nao existem.";

            return null;
        }

        private static void AplicarForm(Produto produto, ProdutoFormDto dto)
        {
            produto.Codigo = dto.Codigo;
            produto.Descricao = dto.Descricao;
            produto.Observacao = dto.Observacao;
            produto.Foto = dto.Foto;
            produto.Ativo = dto.Ativo;

            foreach (var corId in dto.CorIds.Distinct())
                produto.Cores.Add(new ProdutoCor { CorId = corId });

            foreach (var tamanhoId in dto.TamanhoIds.Distinct())
                produto.Tamanhos.Add(new ProdutoTamanho { TamanhoId = tamanhoId });
        }
    }
}
