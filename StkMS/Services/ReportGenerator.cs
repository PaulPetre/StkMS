using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using StkMS.Contracts;
using StkMS.Library.Models;

namespace StkMS.Services
{
    public class ReportGenerator : IReportGenerator
    {
        public byte[] Generate(IEnumerable<ProductStock> inventory)
        {
            var font = new XFont("Arial", 14, XFontStyle.Regular);

            var document = new PdfDocument { PageLayout = PdfPageLayout.OneColumn };

            _ = inventory
                .GetBatches(20)
                .Select(batch => CreatePage(document, font, batch))
                .ToArray();

            return GetDocumentBytes(document);
        }

        //

        private static PdfPage CreatePage(PdfDocument document, XFont font, IEnumerable<ProductStock> batch)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;

            AddText(page, font, 05, 10, 20, 5, "Code", XStringFormats.Center);
            AddText(page, font, 30, 10, 30, 5, "Name", XStringFormats.Center);
            AddText(page, font, 65, 10, 05, 5, "Unit", XStringFormats.Center);
            AddText(page, font, 80, 10, 05, 5, "Price", XStringFormats.Center);
            AddText(page, font, 90, 10, 05, 5, "Qty", XStringFormats.Center);

            var row = 15;
            foreach (var stock in batch)
            {
                AddText(page, font, 05, row, 20, 2, stock.Product.Code, XStringFormats.CenterLeft);
                AddText(page, font, 30, row, 30, 2, stock.Product.Name, XStringFormats.CenterLeft);
                AddText(page, font, 65, row, 05, 2, stock.Product.Unit, XStringFormats.Center);
                AddText(page, font, 80, row, 05, 2, stock.Product.UnitPrice.ToString("C"), XStringFormats.CenterRight);
                AddText(page, font, 90, row, 05, 2, stock.Quantity.ToString("N"), XStringFormats.CenterRight);

                row += 3;
            }

            return page;
        }

        private static void AddText(PdfPage page, XFont font, int x0, int y0, int w, int h, string text, XStringFormat format)
        {
            using var gfx = XGraphics.FromPdfPage(page);

            var xScale = page.Width / 100.0;
            var yScale = page.Height / 100.0;

            gfx.DrawString(text, font, XBrushes.Black, new XRect(x0 * xScale, y0 * yScale, w * xScale, h * yScale), format);
        }

        private static byte[] GetDocumentBytes(PdfDocument document)
        {
            using var ms = new MemoryStream();
            document.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }
    }
}