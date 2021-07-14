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

        public byte[] GenerateInventory(Inventory inventory)
        {
            var font = new XFont("Arial", 14, XFontStyle.Regular);

            var document = new PdfDocument { PageLayout = PdfPageLayout.OneColumn };
            
            _ = inventory
                .Items
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



        private static PdfPage CreateInventoryPage(PdfDocument document, XFont font, IEnumerable<InventoryDetails> batch)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;

            AddHeader(page, font);

            AddText(page, font, 03, 15, 18, 5, "Cod Produs", XStringFormats.Center);
            AddText(page, font, 21, 15, 29, 5, "Nume Produs", XStringFormats.Center);
            AddText(page, font, 50, 15, 8, 5, "U.M", XStringFormats.Center);
            AddText(page, font, 58, 15, 10, 5, "StocS", XStringFormats.Center);
            AddText(page, font, 68, 15, 10, 5, "StocF", XStringFormats.Center);
            AddText(page, font, 78, 15, 10, 5, "PretV", XStringFormats.Center);
            AddText(page, font, 88, 15, 10, 5, "PretN", XStringFormats.Center);

            var row = 20;
            foreach (var stock in batch)
            {
                AddText(page, font, 03, row, 18, 2, stock.Code, XStringFormats.Center);
                AddText(page, font, 21, row, 29, 2, stock.Name, XStringFormats.Center);
                AddText(page, font, 50, row, 8, 2, stock.Unit, XStringFormats.Center);
                AddText(page, font, 58, row, 10, 2, stock.OldQuantity.ToString("N2"), XStringFormats.Center);
                AddText(page, font, 68, row, 10, 2, stock.NewQuantity.ToString("N2"), XStringFormats.Center);
                AddText(page, font, 78, row, 10, 2, stock.OldPrice.ToString("N2"), XStringFormats.Center);
                AddText(page, font, 88, row, 10, 2, stock.NewPrice.ToString("N2"), XStringFormats.Center);

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
        private static void AddHeader(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //de pus deasupra tabelului
            gfx.DrawString(
                "Produse inventariate",
                font,
                XBrushes.Black,
                new XRect(5, 5, page.Width, page.Height),
                XStringFormats.Center);
        }
        private static void AddFooter(PdfPage page, XFont font)
        {
            //using var gfx = XGraphics.FromPdfPage(page);

            //gfx.DrawString(
            //    "Produse inventariate",
            //    font,
            //    XBrushes.Black,
            //    new XRect(10, 10, page.Width, page.Height),
            //    XStringFormats.Center);
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