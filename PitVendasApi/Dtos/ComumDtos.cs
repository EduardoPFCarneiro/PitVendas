namespace PitVendasApi.Dtos
{
    public class PaginaDto<T>
    {
        public IReadOnlyList<T> Itens { get; set; } = Array.Empty<T>();
        public int Total { get; set; }
        public int Pagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalPaginas =>
            TamanhoPagina == 0 ? 0 : (int)Math.Ceiling(Total / (double)TamanhoPagina);
    }

    public class FiltroDto
    {
        private const int TamanhoMaximo = 100;
        private int _tamanhoPagina = 20;

        public string? Busca { get; set; }
        public int Pagina { get; set; } = 1;

        public int TamanhoPagina
        {
            get => _tamanhoPagina;
            set => _tamanhoPagina = value is < 1 or > TamanhoMaximo ? 20 : value;
        }

        public bool? Ativo { get; set; }
    }

    public class EnderecoDto
    {
        public int? Id { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Bairro { get; set; }
        public string? Cep { get; set; }
        public int? CidadeId { get; set; }
        public string? CidadeNome { get; set; }
        public string? EstadoSigla { get; set; }
    }

    public class LookupDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = null!;
    }
}
