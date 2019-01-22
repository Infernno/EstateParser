using System.IO;
using System.Reflection;
using EstateParser.Contracts.Data;
using EstateParser.Contracts.Exporting;
using OfficeOpenXml;

namespace EstateParser.Core.Exporting
{
    public class XlsxExporter : IExporter<EstateWorksheet>
    {
        public string Extension { get; } = ".xlsx";

        public void Export(EstateWorksheet item, Stream stream)
        {
            if (!stream.CanWrite)
            {
                throw new IOException("Can't write to stream");
            }

            using (var package = new ExcelPackage())
            {
                var workSheet = PrepareWorksheet(package, item.Headers);
                var fields = typeof(IEstateFullDataItem).GetProperties(BindingFlags.Instance | BindingFlags.Public);

                for (int i = 0; i < item.Items.Length; i++)
                {
                    for (int j = 0; j < fields.Length; j++)
                    {
                        workSheet.Cells[i + 2, j + 1].Value = fields[i].GetValue(item.Items[i]);
                    }
                }

                package.SaveAs(stream);
            }
        }

        private static ExcelWorksheet PrepareWorksheet(ExcelPackage package, string[] headers)
        {
            var worksheet = package.Workbook.Worksheets.Add("Лист 1");

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
            }

            return worksheet;
        }
    }
}
