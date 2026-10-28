using AutoMapper;
using PitVendasApi.Dtos;
using PitVendasApi.Identity;
using PitVendasApi.Models;

namespace PitVendasApi.DataContexts
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<ApplicationUser, UsuarioDto>()
                .ForMember(d => d.Telefone, o => o.MapFrom(s => s.PhoneNumber));

            CreateMap<Endereco, EnderecoDto>()
                .ForMember(d => d.CidadeNome, o => o.MapFrom(s => s.Cidade!.Nome))
                .ForMember(d => d.EstadoSigla, o => o.MapFrom(s => s.Cidade!.Estado.Sigla));

            CreateMap<Cliente, ClienteDto>();
            CreateMap<Cliente, ClienteListaDto>()
                .ForMember(d => d.CidadeNome, o => o.MapFrom(s => s.Endereco!.Cidade!.Nome));

            CreateMap<Fabrica, FabricaDto>();
            CreateMap<Fabrica, FabricaListaDto>();

            CreateMap<Produto, ProdutoDto>()
                .ForMember(d => d.FabricaNome, o => o.MapFrom(s => s.Fabrica.Nome))
                .ForMember(d => d.Cores, o => o.MapFrom(s =>
                    s.Cores.Select(c => new LookupDto { Id = c.CorId, Nome = c.Cor.Nome })))
                .ForMember(d => d.Tamanhos, o => o.MapFrom(s =>
                    s.Tamanhos.Select(t => new LookupDto { Id = t.TamanhoId, Nome = t.Tamanho.Codigo })));

            CreateMap<Produto, ProdutoListaDto>()
                .ForMember(d => d.FabricaNome, o => o.MapFrom(s => s.Fabrica.Nome))
                .ForMember(d => d.MenorPreco, o => o.MapFrom(s =>
                    s.Precos.Where(p => p.Ativo).Min(p => (decimal?)p.Valor)));

            CreateMap<Preco, PrecoDto>();

            CreateMap<Pedido, PedidoDto>()
                .ForMember(d => d.ClienteNome, o => o.MapFrom(s => s.Cliente.Nome))
                .ForMember(d => d.FabricaNome, o => o.MapFrom(s => s.Fabrica.Nome));

            CreateMap<Pedido, PedidoListaDto>()
                .ForMember(d => d.ClienteNome, o => o.MapFrom(s => s.Cliente.Nome))
                .ForMember(d => d.FabricaNome, o => o.MapFrom(s => s.Fabrica.Nome));

            CreateMap<PedidoItem, PedidoItemDto>();
            CreateMap<PedidoItemGrade, GradeDto>();

            CreateMap<Cor, LookupDto>();
            CreateMap<Tamanho, LookupDto>()
                .ForMember(d => d.Nome, o => o.MapFrom(s => s.Codigo));
            CreateMap<Estado, LookupDto>();
            CreateMap<Cidade, LookupDto>();
        }
    }
}
