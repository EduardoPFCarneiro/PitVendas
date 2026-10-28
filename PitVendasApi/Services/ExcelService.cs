using ClosedXML.Excel;

namespace PitVendasApi.Services
{
    public interface IExcelService
    {
        byte[] Gerar(string titulo, string[] colunas, IEnumerable<object?[]> linhas);
    }

    public class ExcelService : IExcelService
    {
        public byte[] Gerar(string titulo, string[] colunas, IEnumerable<object?[]> linhas)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(LimitarNome(titulo));

            var celulaTitulo = ws.Cell(1, 1);
            celulaTitulo.Value = titulo;
            celulaTitulo.Style.Font.Bold = true;
            celulaTitulo.Style.Font.FontSize = 14;
            ws.Range(1, 1, 1, colunas.Length).Merge();

            for (var c = 0; c < colunas.Length; c++)
            {
                var cell = ws.Cell(3, c + 1);
                cell.Value = colunas[c];
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#0d86d9");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            var linha = 4;
            foreach (var registro in linhas)
            {
                for (var c = 0; c < registro.Length; c++)
                {
                    var cell = ws.Cell(linha, c + 1);
                    switch (registro[c])
                    {
                        case null: cell.Value = string.Empty; break;
                        case decimal d: cell.Value = d; cell.Style.NumberFormat.Format = "R$ #,##0.00"; break;
                        case int i: cell.Value = i; break;
                        case DateTime dt: cell.Value = dt; cell.Style.DateFormat.Format = "dd/MM/yyyy"; break;
                        case DateOnly do_: cell.Value = do_.ToDateTime(TimeOnly.MinValue); cell.Style.DateFormat.Format = "dd/MM/yyyy"; break;
                        case bool b: cell.Value = b ? "Sim" : "Não"; break;
                        default: cell.Value = registro[c]!.ToString(); break;
                    }
                }
                linha++;
            }

            ws.Columns().AdjustToContents();
            ws.SheetView.FreezeRows(3);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string LimitarNome(string nome)
        {
            var limpo = new string(nome.Where(c => !"\\/?*[]:".Contains(c)).ToArray());
            return limpo.Length > 31 ? limpo[..31] : limpo;
        }
    }
}
