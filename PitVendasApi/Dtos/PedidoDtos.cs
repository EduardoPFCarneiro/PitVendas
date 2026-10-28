using System.ComponentModel.DataAnnotations;
using PitVendasApi.Models;

namespace PitVendasApi.Dtos
{
    public class PedidoListaDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public string ClienteNome { get; set; } = null!;
        public string FabricaNome { get; set; } = null!;
        public decimal ValorTotal { get; set; }
        public int TotalPecas { get; set; }
        public PedidoStatus Status { get; set; }
        public string StatusDescricao => Status.ToString();
    }

    public class PedidoDto
    {
        public int Id { get; set; }
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public DateOnly? DataFaturamento { get; set; }
        public PedidoStatus Status { get; set; }
        public string StatusDescricao => Status.ToString();

        public int ClienteId { get; set; }
        public string ClienteNome { get; set; } = null!;
        public int FabricaId { get; set; }
        public string FabricaNome { get; set; } = null!;
        public string? TabelaPreco { get; set; }

        public decimal ValorBruto { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal Desconto { get; set; }

        public decimal ValorTotal { get; set; }
        public decimal PercentualComissao { get; set; }
        public decimal ValorComissao { get; set; }
        public int TotalPecas { get; set; }

        public string? Prazo { get; set; }
        public string? Frete { get; set; }
        public string? CondicaoPagamento { get; set; }
        public int Parcelas { get; set; }
        public string? Observacao { get; set; }
        public DateTime? EnviadoEm { get; set; }

        public List<PedidoItemDto> Itens { get; set; } = new();
    }

    public class PedidoItemDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string CodigoProduto { get; set; } = null!;
        public string DescricaoProduto { get; set; } = null!;
        public int CorId { get; set; }
        public string NomeCor { get; set; } = null!;
        public decimal ValorUnitario { get; set; }
        public int Quantidade { get; set; }
        public decimal Subtotal { get; set; }
        public string? Observacao { get; set; }
        public List<GradeDto> Grade { get; set; } = new();
    }

    public class GradeDto
    {
        public int TamanhoId { get; set; }
        public string CodigoTamanho { get; set; } = null!;
        public int Quantidade { get; set; }
    }

    // ----------------------- entrada -----------------------

    public class PedidoFormDto
    {
        [Required(ErrorMessage = "Informe o cliente.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Informe a fabrica.")]
        public int FabricaId { get; set; }

        public DateTime? Data { get; set; }

        public DateOnly? DataFaturamento { get; set; }

        [StringLength(30)]
        public string? TabelaPreco { get; set; }

        [Range(0, 100, ErrorMessage = "O desconto deve estar entre 0% e 100%.")]
        public decimal PercentualDesconto { get; set; }

        [StringLength(100)]
        public string? Prazo { get; set; }

        [StringLength(100)]
        public string? Frete { get; set; }

        [StringLength(100)]
        public string? CondicaoPagamento { get; set; }

        [Range(1, 60, ErrorMessage = "O numero de parcelas deve estar entre 1 e 60.")]
        public int Parcelas { get; set; } = 1;

        [StringLength(500)]
        public string? Observacao { get; set; }

        [MinLength(1, ErrorMessage = "O pedido precisa de ao menos um item.")]
        public List<PedidoItemFormDto> Itens { get; set; } = new();
    }

    public class PedidoItemFormDto
    {
        [Required(ErrorMessage = "Informe o produto.")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Informe a cor.")]
        public int CorId { get; set; }

        [Range(0.01, 999999, ErrorMessage = "O valor unitario deve ser maior que zero.")]
        public decimal? ValorUnitario { get; set; }

        [StringLength(300)]
        public string? Observacao { get; set; }

        [MinLength(1, ErrorMessage = "Informe a grade de tamanhos do item.")]
        public List<GradeFormDto> Grade { get; set; } = new();
    }

    public class GradeFormDto
    {
        [Required]
        public int TamanhoId { get; set; }

        [Range(1, 99999, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }
    }

    public class AlterarStatusDto
    {
        [Required]
        public PedidoStatus Status { get; set; }
    }
}
