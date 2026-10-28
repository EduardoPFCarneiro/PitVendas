using System.ComponentModel.DataAnnotations;

namespace PitVendasApi.Dtos
{
    public class FabricaListaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public decimal Comissao { get; set; }
        public string? Telefone { get; set; }
        public bool Ativo { get; set; }
    }

    public class FabricaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public decimal Comissao { get; set; }
        public bool Ativo { get; set; }
        public EnderecoDto? Endereco { get; set; }
    }

    public class FabricaFormDto
    {
        [Required(ErrorMessage = "Informe o nome.")]
        [StringLength(200)]
        public string Nome { get; set; } = null!;

        [Required(ErrorMessage = "Informe a razao social.")]
        [StringLength(200)]
        public string RazaoSocial { get; set; } = null!;

        [Required(ErrorMessage = "Informe o CNPJ.")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "O CNPJ deve conter 14 digitos, sem pontuacao.")]
        public string Cnpj { get; set; } = null!;

        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [Range(0, 100, ErrorMessage = "A comissao deve estar entre 0 e 100%.")]
        public decimal Comissao { get; set; }

        public bool Ativo { get; set; } = true;

        public EnderecoDto? Endereco { get; set; }
    }
}
