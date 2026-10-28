using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class FabricasController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _usuario;

        public FabricasController(AppDbContext db, IMapper mapper, ICurrentUser usuario)
        {
            _db = db;
            _mapper = mapper;
            _usuario = usuario;
        }

        [HttpGet]
        public async Task<ActionResult<PaginaDto<FabricaListaDto>>> Listar(
            [FromQuery] FiltroDto filtro, CancellationToken ct)
        {
            var query = _db.Fabricas.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                var busca = filtro.Busca.Trim();
                query = query.Where(f =>
                    f.Nome.Contains(busca) ||
                    f.RazaoSocial.Contains(busca) ||
                    f.Cnpj.Contains(busca));
            }

            if (filtro.Ativo is not null)
                query = query.Where(f => f.Ativo == filtro.Ativo);

            var total = await query.CountAsync(ct);

            var itens = await query
                .OrderBy(f => f.Nome)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .ProjectTo<FabricaListaDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return Ok(new PaginaDto<FabricaListaDto>
            {
                Itens = itens,
                Total = total,
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FabricaDto>> Obter(int id, CancellationToken ct)
        {
            var fabrica = await _db.Fabricas
                .AsNoTracking()
                .Include(f => f.Endereco).ThenInclude(e => e!.Cidade).ThenInclude(c => c!.Estado)
                .FirstOrDefaultAsync(f => f.Id == id, ct);

            return fabrica is null ? NotFound() : Ok(_mapper.Map<FabricaDto>(fabrica));
        }

        [HttpPost]
        public async Task<ActionResult<FabricaDto>> Criar(FabricaFormDto dto, CancellationToken ct)
        {
            if (await CnpjDuplicadoAsync(dto.Cnpj, null, ct))
                return Conflict(new { mensagem = "Ja existe uma fabrica com este CNPJ." });

            var fabrica = new Fabrica { VendedorId = _usuario.Id! };
            AplicarForm(fabrica, dto);

            _db.Fabricas.Add(fabrica);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(Obter), new { id = fabrica.Id },
                _mapper.Map<FabricaDto>(fabrica));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<FabricaDto>> Atualizar(int id, FabricaFormDto dto, CancellationToken ct)
        {
            var fabrica = await _db.Fabricas
                .Include(f => f.Endereco)
                .FirstOrDefaultAsync(f => f.Id == id, ct);

            if (fabrica is null) return NotFound();

            if (await CnpjDuplicadoAsync(dto.Cnpj, id, ct))
                return Conflict(new { mensagem = "Ja existe outra fabrica com este CNPJ." });

            AplicarForm(fabrica, dto);
            await _db.SaveChangesAsync(ct);

            return Ok(_mapper.Map<FabricaDto>(fabrica));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Inativar(int id, CancellationToken ct)
        {
            var fabrica = await _db.Fabricas.FirstOrDefaultAsync(f => f.Id == id, ct);
            if (fabrica is null) return NotFound();

            fabrica.Ativo = false;
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        private async Task<bool> CnpjDuplicadoAsync(string cnpj, int? idAtual, CancellationToken ct) =>
            await _db.Fabricas.AnyAsync(f => f.Cnpj == cnpj && (idAtual == null || f.Id != idAtual), ct);

        private void AplicarForm(Fabrica fabrica, FabricaFormDto dto)
        {
            fabrica.Nome = dto.Nome;
            fabrica.RazaoSocial = dto.RazaoSocial;
            fabrica.Cnpj = dto.Cnpj;
            fabrica.Email = dto.Email;
            fabrica.Telefone = dto.Telefone;
            fabrica.Comissao = dto.Comissao;
            fabrica.Ativo = dto.Ativo;

            if (dto.Endereco is null) return;

            fabrica.Endereco ??= new Endereco();
            fabrica.Endereco.Logradouro = dto.Endereco.Logradouro;
            fabrica.Endereco.Numero = dto.Endereco.Numero;
            fabrica.Endereco.Complemento = dto.Endereco.Complemento;
            fabrica.Endereco.Bairro = dto.Endereco.Bairro;
            fabrica.Endereco.Cep = dto.Endereco.Cep;
            fabrica.Endereco.CidadeId = dto.Endereco.CidadeId;
        }
    }
}
