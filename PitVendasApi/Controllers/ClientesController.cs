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
    public class ClientesController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _usuario;

        public ClientesController(AppDbContext db, IMapper mapper, ICurrentUser usuario)
        {
            _db = db;
            _mapper = mapper;
            _usuario = usuario;
        }

        [HttpGet]
        public async Task<ActionResult<PaginaDto<ClienteListaDto>>> Listar(
            [FromQuery] FiltroDto filtro, CancellationToken ct)
        {
            var query = _db.Clientes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                var busca = filtro.Busca.Trim();
                query = query.Where(c =>
                    c.Nome.Contains(busca) ||
                    c.RazaoSocial.Contains(busca) ||
                    c.Cnpj.Contains(busca));
            }

            if (filtro.Ativo is not null)
                query = query.Where(c => c.Ativo == filtro.Ativo);

            var total = await query.CountAsync(ct);

            var itens = await query
                .OrderBy(c => c.Nome)
                .Skip((filtro.Pagina - 1) * filtro.TamanhoPagina)
                .Take(filtro.TamanhoPagina)
                .Select(c => new ClienteListaDto
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    RazaoSocial = c.RazaoSocial,
                    Cnpj = c.Cnpj,
                    Telefone = c.Telefone,
                    CidadeNome = c.Endereco!.Cidade!.Nome,
                    Ativo = c.Ativo
                })
                .ToListAsync(ct);

            return Ok(new PaginaDto<ClienteListaDto>
            {
                Itens = itens,
                Total = total,
                Pagina = filtro.Pagina,
                TamanhoPagina = filtro.TamanhoPagina
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClienteDto>> Obter(int id, CancellationToken ct)
        {
            var cliente = await _db.Clientes
                .AsNoTracking()
                .Include(c => c.Endereco).ThenInclude(e => e!.Cidade).ThenInclude(c => c!.Estado)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            return cliente is null ? NotFound() : Ok(_mapper.Map<ClienteDto>(cliente));
        }

        [HttpPost]
        public async Task<ActionResult<ClienteDto>> Criar(ClienteFormDto dto, CancellationToken ct)
        {
            if (await CnpjDuplicadoAsync(dto.Cnpj, null, ct))
                return Conflict(new { mensagem = "Ja existe um cliente com este CNPJ." });

            var cliente = new Cliente { VendedorId = _usuario.Id! };
            AplicarForm(cliente, dto);

            _db.Clientes.Add(cliente);
            await _db.SaveChangesAsync(ct);

            return CreatedAtAction(nameof(Obter), new { id = cliente.Id },
                _mapper.Map<ClienteDto>(cliente));
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ClienteDto>> Atualizar(int id, ClienteFormDto dto, CancellationToken ct)
        {
            var cliente = await _db.Clientes
                .Include(c => c.Endereco)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

            if (cliente is null) return NotFound();

            if (await CnpjDuplicadoAsync(dto.Cnpj, id, ct))
                return Conflict(new { mensagem = "Ja existe outro cliente com este CNPJ." });

            AplicarForm(cliente, dto);
            await _db.SaveChangesAsync(ct);

            return Ok(_mapper.Map<ClienteDto>(cliente));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Inativar(int id, CancellationToken ct)
        {
            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == id, ct);
            if (cliente is null) return NotFound();

            cliente.Ativo = false;
            await _db.SaveChangesAsync(ct);

            return NoContent();
        }

        private async Task<bool> CnpjDuplicadoAsync(string cnpj, int? idAtual, CancellationToken ct) =>
            await _db.Clientes.AnyAsync(c => c.Cnpj == cnpj && (idAtual == null || c.Id != idAtual), ct);

        private void AplicarForm(Cliente cliente, ClienteFormDto dto)
        {
            cliente.Nome = dto.Nome;
            cliente.RazaoSocial = dto.RazaoSocial;
            cliente.Cnpj = dto.Cnpj;
            cliente.InscricaoEstadual = dto.InscricaoEstadual;
            cliente.Email = dto.Email;
            cliente.Telefone = dto.Telefone;
            cliente.Foto = dto.Foto;
            cliente.Observacao = dto.Observacao;
            cliente.Ativo = dto.Ativo;

            if (dto.Endereco is null) return;

            cliente.Endereco ??= new Endereco();
            cliente.Endereco.Logradouro = dto.Endereco.Logradouro;
            cliente.Endereco.Numero = dto.Endereco.Numero;
            cliente.Endereco.Complemento = dto.Endereco.Complemento;
            cliente.Endereco.Bairro = dto.Endereco.Bairro;
            cliente.Endereco.Cep = dto.Endereco.Cep;
            cliente.Endereco.CidadeId = dto.Endereco.CidadeId;
        }
    }
}
