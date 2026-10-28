using Microsoft.EntityFrameworkCore;
using PitVendasApi.Models;

namespace PitVendasApi.DataContexts
{
    public static class SeedData
    {
        public static void Apply(ModelBuilder builder)
        {
            SeedEstados(builder);
            SeedCidadesRondonia(builder);
            SeedCores(builder);
            SeedTamanhos(builder);
        }

        private static void SeedEstados(ModelBuilder builder)
        {
            var estados = new (int Id, string Nome, string Sigla)[]
            {
                (1,  "Acre", "AC"),
                (2,  "Alagoas", "AL"),
                (3,  "Amazonas", "AM"),
                (4,  "Amapá", "AP"),
                (5,  "Bahia", "BA"),
                (6,  "Ceará", "CE"),
                (7,  "Distrito Federal", "DF"),
                (8,  "Espírito Santo", "ES"),
                (9,  "Goiás", "GO"),
                (10, "Maranhão", "MA"),
                (11, "Minas Gerais", "MG"),
                (12, "Mato Grosso do Sul", "MS"),
                (13, "Mato Grosso", "MT"),
                (14, "Pará", "PA"),
                (15, "Paraíba", "PB"),
                (16, "Pernambuco", "PE"),
                (17, "Piauí", "PI"),
                (18, "Paraná", "PR"),
                (19, "Rio de Janeiro", "RJ"),
                (20, "Rio Grande do Norte", "RN"),
                (21, "Rondônia", "RO"),
                (22, "Roraima", "RR"),
                (23, "Rio Grande do Sul", "RS"),
                (24, "Santa Catarina", "SC"),
                (25, "Sergipe", "SE"),
                (26, "São Paulo", "SP"),
                (27, "Tocantins", "TO"),
            };

            builder.Entity<Estado>().HasData(
                estados.Select(x => new Estado { Id = x.Id, Nome = x.Nome, Sigla = x.Sigla }));
        }

        private static void SeedCidadesRondonia(ModelBuilder builder)
        {
            const int rondonia = 21;

            var cidades = new[]
            {
                "Alta Floresta d'Oeste", "Alto Alegre dos Parecis", "Alto Paraíso",
                "Alvorada d'Oeste", "Ariquemes", "Buritis", "Cabixi", "Cacaulândia",
                "Cacoal", "Campo Novo de Rondônia", "Candeias do Jamari", "Castanheiras",
                "Cerejeiras", "Chupinguaia", "Colorado do Oeste", "Corumbiara",
                "Costa Marques", "Cujubim", "Espigão d'Oeste", "Governador Jorge Teixeira",
                "Guajará-Mirim", "Itapuã do Oeste", "Jaru", "Ji-Paraná",
                "Machadinho d'Oeste", "Ministro Andreazza", "Mirante da Serra",
                "Monte Negro", "Nova Brasilândia d'Oeste", "Nova Mamoré", "Nova União",
                "Novo Horizonte do Oeste", "Ouro Preto do Oeste", "Parecis",
                "Pimenta Bueno", "Pimenteiras do Oeste", "Porto Velho",
                "Presidente Médici", "Primavera de Rondônia", "Rio Crespo",
                "Rolim de Moura", "Santa Luzia d'Oeste", "São Felipe d'Oeste",
                "São Francisco do Guaporé", "São Miguel do Guaporé", "Seringueiras",
                "Teixeirópolis", "Theobroma", "Urupá", "Vale do Anari",
                "Vale do Paraíso", "Vilhena",
            };

            builder.Entity<Cidade>().HasData(
                cidades.Select((nome, i) => new Cidade
                {
                    Id = i + 1,
                    Nome = nome,
                    EstadoId = rondonia
                }));
        }

        private static void SeedCores(ModelBuilder builder)
        {
            var cores = new[]
            {
                "Verde", "Azul", "Preto", "Cinza",
                "Branco", "Vermelho", "Rosa", "Sortido"
            };

            builder.Entity<Cor>().HasData(
                cores.Select((nome, i) => new Cor { Id = i + 1, Nome = nome }));
        }

        private static void SeedTamanhos(ModelBuilder builder)
        {
            var tamanhos = new (int Id, string Codigo, int Ordem)[]
            {
                (1,  "PP",  1),
                (2,  "P",   2),
                (3,  "M",   3),
                (4,  "G",   4),
                (5,  "GG",  5),
                (16, "EG",  6),
                (6,  "XG",  7),
                (7,  "XXG", 8),
                (17, "3XG", 9),
                (8,  "2",  10),
                (9,  "4",  11),
                (10, "6",  12),
                (11, "8",  13),
                (12, "10", 14),
                (13, "12", 15),
                (14, "14", 16),
                (15, "16", 17),
            };

            builder.Entity<Tamanho>().HasData(
                tamanhos.Select(t => new Tamanho
                {
                    Id = t.Id,
                    Codigo = t.Codigo,
                    Ordem = t.Ordem
                }));
        }
    }
}
