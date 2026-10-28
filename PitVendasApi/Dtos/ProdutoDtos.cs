using System.ComponentModel.DataAnnotations;

namespace PitVendasApi.Dtos
{
    public class ProdutoListaDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public string FabricaNome { get; set; } = null!;
        public decimal? MenorPreco { get; set; }
        public bool Ativo { get; set; }
    }

    public class ProdutoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public string Descricao { get; set; } = null!;
        public string? Observacao { get; set; }
        public string? Foto { get; set; }
        public bool Ativo { get; set; }
        public int FabricaId { get; set; }
        public string FabricaNome { get; set; } = null!;

        public List<LookupDto> Cores { get; set; } = new();
        public List<LookupDto> Tamanhos { get; set; } = new();
        public List<PrecoDto> Precos { get; set; } = new();
    }

    public class ProdutoFormDto
    {
        [Required(ErrorMessage = "Informe o codigo.")]
        [StringLength(60)]
        public string Codigo { get; set; } = null!;

        [Required(ErrorMessage = "Informe a descricao.")]
        [StringLength(300)]
        public string Descricao { get; set; } = null!;

        [StringLength(500)]
        public string? Observacao { get; set; }

        [StringLength(300)]
        public string? Foto { get; set; }

        [Required(ErrorMessage = "Informe a fabrica.")]
        public int FabricaId { get; set; }

        public bool Ativo { get; set; } = true;

        [MinLength(1, ErrorMessage = "Informe ao menos uma cor.")]
        public List<int> CorIds { get; set; } = new();

        [MinLength(1, ErrorMessage = "Informe ao menos um tamanho.")]
        public List<int> TamanhoIds { get; set; } = new();
    }

    public class PrecoDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = null!;
        public decimal Valor { get; set; }
        public string? Observacao { get; set; }
        public DateOnly DataAlteracao { get; set; }
        public bool Ativo { get; set; }
    }

    public class PrecoFormDto
    {
        [Required(ErrorMessage = "Informe o codigo da tabela de preco.")]
        [StringLength(30)]
        public string Codigo { get; set; } = null!;

        [Range(0.01, 999999, ErrorMessage = "O valor deve ser maior que zero.")]
        public decimal Valor { get; set; }

        [StringLength(300)]
        public string? Observacao { get; set; }

        public bool Ativo { get; set; } = true;
    }
}
