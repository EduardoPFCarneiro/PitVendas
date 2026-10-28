using System.ComponentModel.DataAnnotations;

namespace PitVendasApi.Dtos
{
    public class ClienteListaDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string? Telefone { get; set; }
        public string? CidadeNome { get; set; }
        public bool Ativo { get; set; }
    }

    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
        public string RazaoSocial { get; set; } = null!;
        public string Cnpj { get; set; } = null!;
        public string? InscricaoEstadual { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public string? Foto { get; set; }
        public string? Observacao { get; set; }
        public bool Ativo { get; set; }
        public EnderecoDto? Endereco { get; set; }
    }

    public class ClienteFormDto
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

        [StringLength(20)]
        public string? InscricaoEstadual { get; set; }

        [EmailAddress(ErrorMessage = "E-mail invalido.")]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [StringLength(300)]
        public string? Foto { get; set; }

        [StringLength(500)]
        public string? Observacao { get; set; }

        public bool Ativo { get; set; } = true;

        public EnderecoDto? Endereco { get; set; }
    }
}
