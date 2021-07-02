using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using StkMS.Contracts;
using StkMS.Library.Models;
using StkMS.Library.Services;
using StkMS.ViewModels;

namespace StkMS.Services
{
    public class ReportGenerator : IReportGenerator
    {
        public byte[] GenerateInventory(IEnumerable<ProductStock> inventory)
        {
            var font = new XFont("Arial", 14, XFontStyle.Regular);

            var document = new PdfDocument { PageLayout = PdfPageLayout.OneColumn };

            _ = inventory
                .GetBatches(20)
                .Select(batch => CreateInventoryPage(document, font, batch))
                .ToArray();

            return GetDocumentBytes(document);
        }

        public byte[] GenerateSaleReport(SaleDetailsViewModel saleDetails)
        {
            var font = new XFont("Arial", 10, XFontStyle.Regular);

            var document = new PdfDocument { PageLayout = PdfPageLayout.OneColumn };
            CreateSalePage(document, font, saleDetails);

            return GetDocumentBytes(document);
        }

        //

        private static PdfPage CreateInventoryPage(PdfDocument document, XFont font, IEnumerable<ProductStock> batch)
        {
            var font = new XFont("Arial", 10, XFontStyle.Regular);

            AddText(page, font, 05, 10, 25, 5, "Code", XStringFormats.Center);
            AddText(page, font, 30, 10, 35, 5, "Name", XStringFormats.Center);
            AddText(page, font, 65, 10, 10, 5, "Unit", XStringFormats.Center);
            AddText(page, font, 75, 10, 10, 5, "Price", XStringFormats.Center);
            AddText(page, font, 85, 10, 10, 5, "Qty", XStringFormats.Center);

            var row = 15;
            foreach (var stock in batch)
            {
                AddText(page, font, 05, row, 25, 2, stock.Product.Code, XStringFormats.Center);
                AddText(page, font, 30, row, 35, 2, stock.Product.Name, XStringFormats.Center);
                AddText(page, font, 65, row, 10, 2, stock.Product.Unit, XStringFormats.Center);
                AddText(page, font, 75, row, 10, 2, stock.Product.UnitPrice.ToString("C", new CultureInfo("ro-RO")), XStringFormats.Center);
                AddText(page, font, 85, row, 10, 2, stock.Quantity.ToString("N1"), XStringFormats.CenterRight);

                row += 2;
            }

            AddFooter(page, font);

            return page;
        }

        private static void CreateSalePage(PdfDocument document, XFont font, SaleDetailsViewModel saleDetails)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;

            AddText(page, font, 05, 10, 25, 5, $"Bon Nr. {saleDetails.Id} din {saleDetails.DateTime:U}", XStringFormats.Center);

            var row = 20;
            foreach (var item in saleDetails.Items)
            {
                AddText(page, font, 05, row, 25, 2, item.Code, XStringFormats.Center);
                AddText(page, font, 30, row, 35, 2, item.Name, XStringFormats.Center);
                AddText(page, font, 65, row, 10, 2, item.Unit, XStringFormats.Center);
                AddText(page, font, 75, row, 10, 2, item.UnitPrice.ToString("C", new CultureInfo("ro-RO")), XStringFormats.Center);
                AddText(page, font, 85, row, 10, 2, item.Quantity.ToString("N1"), XStringFormats.CenterRight);

                row += 2;
            }
        }

        private static void AddText(PdfPage page, XFont font, int x0, int y0, int w, int h, string text, XStringFormat format)
        {
            using var gfx = XGraphics.FromPdfPage(page);

            var xScale = page.Width / 100.0;
            var yScale = page.Height / 100.0;

            gfx.DrawString(text, font, XBrushes.Black, new XRect(x0 * xScale, y0 * yScale, w * xScale, h * yScale), format);
            XPen pen = new XPen(XColors.Black);

            gfx.DrawRectangle(pen, x0 * xScale, y0 * yScale, w * xScale, h * yScale);
        }

        private static void AddFooter(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);

            gfx.DrawString(
                "Produse inventariate",
                font,
                XBrushes.Black,
                new XRect(10, 10, page.Width, page.Height),
                XStringFormats.Center);
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