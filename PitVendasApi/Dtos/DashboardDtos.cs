namespace PitVendasApi.Dtos
{
    public class DashboardDto
    {
        public int TotalPedidos { get; set; }
        public int PedidosNoMes { get; set; }
        public decimal ComissaoNoMes { get; set; }
        public decimal ComissaoNoAno { get; set; }
        public int TotalClientes { get; set; }
        public int TotalFabricas { get; set; }

        public List<RankingFabricaDto> RankingFabricas { get; set; } = new();
        public List<PedidosPorMesDto> PedidosPorMes { get; set; } = new();
    }

    public class RankingFabricaDto
    {
        public int FabricaId { get; set; }
        public string Nome { get; set; } = null!;
        public int QuantidadePedidos { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal Comissao { get; set; }
    }

    public class PedidosPorMesDto
    {
        public int Mes { get; set; }
        public int Quantidade { get; set; }
        public decimal Valor { get; set; }
    }
}
