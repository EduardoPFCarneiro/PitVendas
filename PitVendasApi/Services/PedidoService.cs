using Microsoft.EntityFrameworkCore;
using PitVendasApi.DataContexts;
using PitVendasApi.Dtos;
using PitVendasApi.Models;

namespace PitVendasApi.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CriarAsync(PedidoFormDto dto, CancellationToken ct = default);
        Task<Pedido> AtualizarAsync(int id, PedidoFormDto dto, CancellationToken ct = default);
        Task<Pedido> DuplicarAsync(int id, CancellationToken ct = default);
        Task<Pedido> AlterarStatusAsync(int id, PedidoStatus status, CancellationToken ct = default);
    }

    public class PedidoService : IPedidoService
    {
        private readonly AppDbContext _db;
        private readonly ICurrentUser _usuario;

        public PedidoService(AppDbContext db, ICurrentUser usuario)
        {
            _db = db;
            _usuario = usuario;
        }

        public async Task<Pedido> CriarAsync(PedidoFormDto dto, CancellationToken ct = default)
        {
            var pedido = new Pedido
            {
                VendedorId = _usuario.Id!,
                Numero = await ProximoNumeroAsync(ct),
                Data = dto.Data ?? DateTime.Now,
                Status = PedidoStatus.Rascunho
            };

            await PreencherAsync(pedido, dto, ct);

            _db.Pedidos.Add(pedido);
            await _db.SaveChangesAsync(ct);
            return pedido;
        }

        public async Task<Pedido> AtualizarAsync(int id, PedidoFormDto dto, CancellationToken ct = default)
        {
            var pedido = await _db.Pedidos
                .Include(p => p.Itens).ThenInclude(i => i.Grade)
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new RegraNegocioException("Pedido nao encontrado.");

            if (pedido.Status is not (PedidoStatus.Rascunho or PedidoStatus.Enviado))
            {
                throw new RegraNegocioException(
                    $"Um pedido {pedido.Status} nao pode mais ser editado.");
            }

            _db.PedidoItens.RemoveRange(pedido.Itens);
            pedido.Itens.Clear();

            pedido.Data = dto.Data ?? pedido.Data;
            await PreencherAsync(pedido, dto, ct);

            await _db.SaveChangesAsync(ct);
            return pedido;
        }

        public async Task<Pedido> DuplicarAsync(int id, CancellationToken ct = default)
        {
            var origem = await _db.Pedidos
                .Include(p => p.Itens).ThenInclude(i => i.Grade)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new RegraNegocioException("Pedido nao encontrado.");

            var copia = new Pedido
            {
                VendedorId = _usuario.Id!,
                Numero = await ProximoNumeroAsync(ct),
                Data = DateTime.Now,
                Status = PedidoStatus.Rascunho,
                ClienteId = origem.ClienteId,
                FabricaId = origem.FabricaId,
                PercentualDesconto = origem.PercentualDesconto,
                TabelaPreco = origem.TabelaPreco,
                Prazo = origem.Prazo,
                Frete = origem.Frete,
                CondicaoPagamento = origem.CondicaoPagamento,
                Parcelas = origem.Parcelas,
                Observacao = origem.Observacao,
                PercentualComissao = origem.PercentualComissao
            };

            foreach (var item in origem.Itens)
            {
                var novo = new PedidoItem
                {
                    ProdutoId = item.ProdutoId,
                    CorId = item.CorId,
                    CodigoProduto = item.CodigoProduto,
                    DescricaoProduto = item.DescricaoProduto,
                    NomeCor = item.NomeCor,
                    ValorUnitario = item.ValorUnitario,
                    Observacao = item.Observacao
                };

                foreach (var g in item.Grade)
                {
                    novo.Grade.Add(new PedidoItemGrade
                    {
                        TamanhoId = g.TamanhoId,
                        CodigoTamanho = g.CodigoTamanho,
                        Quantidade = g.Quantidade
                    });
                }

                RecalcularItem(novo);
                copia.Itens.Add(novo);
            }

            RecalcularTotais(copia);

            _db.Pedidos.Add(copia);
            await _db.SaveChangesAsync(ct);
            return copia;
        }

        public async Task<Pedido> AlterarStatusAsync(int id, PedidoStatus status, CancellationToken ct = default)
        {
            var pedido = await _db.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct)
                ?? throw new RegraNegocioException("Pedido nao encontrado.");

            if (pedido.Status == PedidoStatus.Cancelado)
                throw new RegraNegocioException("Um pedido cancelado nao muda mais de status.");

            if (status == PedidoStatus.Enviado && pedido.EnviadoEm is null)
                pedido.EnviadoEm = DateTime.Now;

            pedido.Status = status;
            await _db.SaveChangesAsync(ct);
            return pedido;
        }

        private async Task PreencherAsync(Pedido pedido, PedidoFormDto dto, CancellationToken ct)
        {
            var cliente = await _db.Clientes.FirstOrDefaultAsync(c => c.Id == dto.ClienteId, ct)
                ?? throw new RegraNegocioException("Cliente nao encontrado.");

            if (!cliente.Ativo)
                throw new RegraNegocioException($"O cliente {cliente.Nome} esta inativo.");

            var fabrica = await _db.Fabricas.FirstOrDefaultAsync(f => f.Id == dto.FabricaId, ct)
                ?? throw new RegraNegocioException("Fabrica nao encontrada.");

            if (!fabrica.Ativo)
                throw new RegraNegocioException($"A fabrica {fabrica.Nome} esta inativa.");

            pedido.ClienteId = cliente.Id;
            pedido.FabricaId = fabrica.Id;
            pedido.PercentualDesconto = dto.PercentualDesconto;
            pedido.DataFaturamento = dto.DataFaturamento;
            pedido.TabelaPreco = dto.TabelaPreco;
            pedido.Prazo = dto.Prazo;
            pedido.Frete = dto.Frete;
            pedido.CondicaoPagamento = dto.CondicaoPagamento;
            pedido.Parcelas = dto.Parcelas;
            pedido.Observacao = dto.Observacao;
            pedido.PercentualComissao = fabrica.Comissao;

            var produtoIds = dto.Itens.Select(i => i.ProdutoId).Distinct().ToList();

            var produtos = await _db.Produtos
                .Include(p => p.Cores).ThenInclude(c => c.Cor)
                .Include(p => p.Tamanhos).ThenInclude(t => t.Tamanho)
                .Include(p => p.Precos)
                .Where(p => produtoIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, ct);

            foreach (var itemDto in dto.Itens)
            {
                if (!produtos.TryGetValue(itemDto.ProdutoId, out var produto))
                    throw new RegraNegocioException($"Produto {itemDto.ProdutoId} nao encontrado.");

                if (produto.FabricaId != fabrica.Id)
                {
                    throw new RegraNegocioException(
                        $"O produto {produto.Codigo} nao pertence a fabrica {fabrica.Nome}. " +
                        "Um pedido so pode conter produtos de uma unica fabrica.");
                }

                if (!produto.Ativo)
                    throw new RegraNegocioException($"O produto {produto.Codigo} esta inativo.");

                var produtoCor = produto.Cores.FirstOrDefault(c => c.CorId == itemDto.CorId)
                    ?? throw new RegraNegocioException(
                        $"O produto {produto.Codigo} nao e oferecido na cor informada.");

                var valorUnitario = ResolverValorUnitario(produto, itemDto, dto.TabelaPreco);

                var item = new PedidoItem
                {
                    ProdutoId = produto.Id,
                    CodigoProduto = produto.Codigo,
                    DescricaoProduto = produto.Descricao,
                    CorId = produtoCor.CorId,
                    NomeCor = produtoCor.Cor.Nome,
                    ValorUnitario = valorUnitario,
                    Observacao = itemDto.Observacao
                };

                foreach (var gradeDto in itemDto.Grade)
                {
                    var produtoTamanho = produto.Tamanhos
                        .FirstOrDefault(t => t.TamanhoId == gradeDto.TamanhoId)
                        ?? throw new RegraNegocioException(
                            $"O tamanho informado nao faz parte da grade do produto {produto.Codigo}.");

                    if (item.Grade.Any(g => g.TamanhoId == gradeDto.TamanhoId))
                    {
                        throw new RegraNegocioException(
                            $"O tamanho {produtoTamanho.Tamanho.Codigo} aparece duas vezes " +
                            $"no produto {produto.Codigo} na cor {item.NomeCor}.");
                    }

                    item.Grade.Add(new PedidoItemGrade
                    {
                        TamanhoId = produtoTamanho.TamanhoId,
                        CodigoTamanho = produtoTamanho.Tamanho.Codigo,
                        Quantidade = gradeDto.Quantidade
                    });
                }

                if (pedido.Itens.Any(i => i.ProdutoId == item.ProdutoId && i.CorId == item.CorId))
                {
                    throw new RegraNegocioException(
                        $"O produto {produto.Codigo} na cor {item.NomeCor} foi informado " +
                        "duas vezes. Junte as quantidades em um unico item.");
                }

                RecalcularItem(item);
                pedido.Itens.Add(item);
            }

            RecalcularTotais(pedido);
        }

        private static decimal ResolverValorUnitario(
            Produto produto, PedidoItemFormDto itemDto, string? tabela)
        {
            if (itemDto.ValorUnitario is not null)
                return itemDto.ValorUnitario.Value;

            if (!string.IsNullOrWhiteSpace(tabela))
            {
                var daTabela = produto.Precos
                    .FirstOrDefault(p => p.Ativo && p.Codigo == tabela);

                return daTabela?.Valor ?? throw new RegraNegocioException(
                    $"O produto {produto.Codigo} nao tem preco na tabela {tabela}. " +
                    "Cadastre o preco ou informe o valor unitario.");
            }

            return produto.Precos.Where(p => p.Ativo)
                                 .OrderBy(p => p.Valor)
                                 .Select(p => (decimal?)p.Valor)
                                 .FirstOrDefault()
                ?? throw new RegraNegocioException(
                    $"O produto {produto.Codigo} nao tem preco cadastrado. " +
                    "Informe o valor unitario ou cadastre uma tabela de preco.");
        }

        private static void RecalcularItem(PedidoItem item)
        {
            item.Quantidade = item.Grade.Sum(g => g.Quantidade);
            item.Subtotal = item.Quantidade * item.ValorUnitario;
        }

        private static void RecalcularTotais(Pedido pedido)
        {
            pedido.ValorBruto = pedido.Itens.Sum(i => i.Subtotal);
            pedido.TotalPecas = pedido.Itens.Sum(i => i.Quantidade);

            if (pedido.PercentualDesconto is < 0 or > 100)
            {
                throw new RegraNegocioException(
                    "O desconto deve estar entre 0% e 100%.");
            }

            pedido.Desconto =
                Math.Round(pedido.ValorBruto * pedido.PercentualDesconto / 100m, 2);

            pedido.ValorTotal = pedido.ValorBruto - pedido.Desconto;

            pedido.ValorComissao =
                Math.Round(pedido.ValorTotal * pedido.PercentualComissao / 100m, 2);
        }

        private async Task<int> ProximoNumeroAsync(CancellationToken ct)
        {
            var ultimo = await _db.Pedidos
                .Where(p => p.VendedorId == _usuario.Id)
                .MaxAsync(p => (int?)p.Numero, ct);

            return (ultimo ?? 0) + 1;
        }
    }
}
