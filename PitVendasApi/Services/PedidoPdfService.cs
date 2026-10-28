using PitVendasApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PitVendasApi.Services
{
    public interface IPedidoPdfService
    {
        byte[] Gerar(Pedido pedido);
    }

    public class PedidoPdfService : IPedidoPdfService
    {
        private static readonly string Verde = "#4ce0a5";
        private static readonly string Azul = "#0d86d9";
        private static readonly string Cinza = "#707a82";

        public byte[] Gerar(Pedido pedido)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Calibri));

                    page.Header().Element(c => Cabecalho(c, pedido));
                    page.Content().PaddingVertical(15).Element(c => Corpo(c, pedido));
                    page.Footer().Element(Rodape);
                });
            });

            return documento.GeneratePdf();
        }

        private void Cabecalho(IContainer container, Pedido pedido)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("PIT VENDAS").FontSize(20).Bold().FontColor(Azul);
                        c.Item().Text("Controle e Emissão de Vendas").FontSize(9).FontColor(Cinza);
                    });

                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text($"PEDIDO Nº {pedido.Numero:D4}").FontSize(16).Bold();
                        c.Item().Text($"Data: {pedido.Data:dd/MM/yyyy}").FontColor(Cinza);
                        c.Item().Text($"Situação: {pedido.Status}").FontColor(Cinza);
                    });
                });

                col.Item().PaddingTop(8).LineHorizontal(2).LineColor(Verde);
            });
        }

        private void Corpo(IContainer container, Pedido pedido)
        {
            container.Column(col =>
            {
                col.Spacing(12);

                // ---------------- partes envolvidas ----------------
                col.Item().Row(row =>
                {
                    row.RelativeItem().Element(c => Bloco(c, "CLIENTE", new[]
                    {
                        pedido.Cliente.Nome,
                        pedido.Cliente.RazaoSocial,
                        $"CNPJ: {FormatarCnpj(pedido.Cliente.Cnpj)}",
                        pedido.Cliente.Telefone ?? "",
                    }));

                    row.ConstantItem(15);

                    row.RelativeItem().Element(c => Bloco(c, "FÁBRICA", new[]
                    {
                        pedido.Fabrica.Nome,
                        pedido.Fabrica.RazaoSocial,
                        $"CNPJ: {FormatarCnpj(pedido.Fabrica.Cnpj)}",
                        pedido.Fabrica.Telefone ?? "",
                    }));
                });

                // ---------------- condicoes ----------------
                col.Item().Background("#f5f8fb").Padding(10).Row(row =>
                {
                    ItemCondicao(row, "Tabela", pedido.TabelaPreco ?? "—");
                    ItemCondicao(row, "Condição", pedido.CondicaoPagamento ?? "—");
                    ItemCondicao(row, "Parcelas", pedido.Parcelas.ToString());
                    ItemCondicao(row, "Prazo", pedido.Prazo ?? "—");
                    ItemCondicao(row, "Frete", pedido.Frete ?? "—");
                    ItemCondicao(row, "Faturamento",
                        pedido.DataFaturamento?.ToString("dd/MM/yyyy") ?? "—");
                });

                // ---------------- itens ----------------
                col.Item().Element(c => TabelaItens(c, pedido));

                // ---------------- totais ----------------
                col.Item().AlignRight().Element(c => Totais(c, pedido));

                if (!string.IsNullOrWhiteSpace(pedido.Observacao))
                {
                    col.Item().PaddingTop(5).Column(c =>
                    {
                        c.Item().Text("Observação").Bold().FontSize(9).FontColor(Cinza);
                        c.Item().Text(pedido.Observacao);
                    });
                }
            });
        }

        private void Bloco(IContainer container, string titulo, string[] linhas)
        {
            container.Border(1).BorderColor("#e5eaef").Padding(10).Column(col =>
            {
                col.Item().Text(titulo).Bold().FontSize(9).FontColor(Azul);
                col.Item().PaddingTop(3).LineHorizontal(0.5f).LineColor("#e5eaef");
                foreach (var linha in linhas.Where(l => !string.IsNullOrWhiteSpace(l)))
                    col.Item().PaddingTop(2).Text(linha);
            });
        }

        private void ItemCondicao(RowDescriptor row, string rotulo, string valor)
        {
            row.RelativeItem().Column(c =>
            {
                c.Item().Text(rotulo).FontSize(8).FontColor(Cinza);
                c.Item().Text(valor).Bold();
            });
        }

        private void TabelaItens(IContainer container, Pedido pedido)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(55);
                    cols.RelativeColumn(3);
                    cols.RelativeColumn(1.5f);
                    cols.RelativeColumn(2.5f);
                    cols.ConstantColumn(35);
                    cols.ConstantColumn(60);
                    cols.ConstantColumn(65);
                });

                table.Header(header =>
                {
                    void Th(string t, bool right = false)
                    {
                        var cell = header.Cell().Background(Azul).Padding(5);
                        (right ? cell.AlignRight() : cell.AlignLeft())
                            .Text(t).FontColor("#ffffff").Bold().FontSize(9);
                    }
                    Th("Código"); Th("Descrição"); Th("Cor"); Th("Grade");
                    Th("Qtd", true); Th("Unit.", true); Th("Subtotal", true);
                });

                var alterna = false;
                foreach (var item in pedido.Itens)
                {
                    var fundo = alterna ? "#f5f8fb" : "#ffffff";
                    alterna = !alterna;

                    var grade = string.Join("  ",
                        item.Grade.OrderBy(g => g.Tamanho.Ordem)
                                  .Select(g => $"{g.CodigoTamanho}:{g.Quantidade}"));

                    table.Cell().Background(fundo).Padding(5).Text(item.CodigoProduto);
                    table.Cell().Background(fundo).Padding(5).Text(item.DescricaoProduto);
                    table.Cell().Background(fundo).Padding(5).Text(item.NomeCor);
                    table.Cell().Background(fundo).Padding(5).Text(grade).FontSize(9);
                    table.Cell().Background(fundo).Padding(5).AlignRight().Text(item.Quantidade.ToString());
                    table.Cell().Background(fundo).Padding(5).AlignRight().Text(Moeda(item.ValorUnitario));
                    table.Cell().Background(fundo).Padding(5).AlignRight().Text(Moeda(item.Subtotal));
                }
            });
        }

        private void Totais(IContainer container, Pedido pedido)
        {
            container.Width(240).Column(col =>
            {
                LinhaTotal(col, "Total de peças", pedido.TotalPecas.ToString());
                LinhaTotal(col, "Valor bruto", Moeda(pedido.ValorBruto));
                LinhaTotal(col, $"Desconto ({pedido.PercentualDesconto:0.##}%)", "- " + Moeda(pedido.Desconto));
                col.Item().PaddingVertical(3).LineHorizontal(0.5f).LineColor("#e5eaef");
                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("VALOR LÍQUIDO").Bold();
                    row.RelativeItem().AlignRight().Text(Moeda(pedido.ValorTotal)).Bold().FontColor(Azul);
                });
            });
        }

        private void LinhaTotal(ColumnDescriptor col, string rotulo, string valor)
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Text(rotulo).FontColor(Cinza);
                row.RelativeItem().AlignRight().Text(valor);
            });
        }

        private void Rodape(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(0.5f).LineColor("#e5eaef");
                col.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text($"Emitido em {DateTime.Now:dd/MM/yyyy HH:mm}")
                        .FontSize(8).FontColor(Cinza);
                    row.RelativeItem().AlignRight().Text(t =>
                    {
                        t.Span("Página ").FontSize(8).FontColor(Cinza);
                        t.CurrentPageNumber().FontSize(8).FontColor(Cinza);
                        t.Span(" de ").FontSize(8).FontColor(Cinza);
                        t.TotalPages().FontSize(8).FontColor(Cinza);
                    });
                });
            });
        }

        private static string Moeda(decimal valor) =>
            valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));

        private static string FormatarCnpj(string cnpj)
        {
            var so = new string(cnpj.Where(char.IsDigit).ToArray());
            return so.Length == 14
                ? $"{so[..2]}.{so[2..5]}.{so[5..8]}/{so[8..12]}-{so[12..]}"
                : cnpj;
        }
    }
}
