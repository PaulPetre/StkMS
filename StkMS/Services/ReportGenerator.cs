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
using System;

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
            var font2 = new XFont("Arial", 9, XFontStyle.Regular);
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
            XFont font2 = new XFont("Arial", 9);
            XFont font3 = new XFont("Arial", 10);
            XFont font4 = new XFont("Arial", 8);

            AddText(page, font, 05, 10, 25, 5, $"Bon Nr. {saleDetails.Id} din {saleDetails.FormatDateTime}", XStringFormats.Center);

            AddText(page, font, 05, 27, 20, 3, "Cod Produs", XStringFormats.Center);
            AddText(page, font, 25, 27, 30, 3, "Nume Produs", XStringFormats.Center);
            AddText(page, font, 55, 27, 10, 3, "U.M", XStringFormats.Center);
            AddText(page, font, 65, 27, 10, 3, "Pret", XStringFormats.Center);
            AddText(page, font, 75, 27, 10, 3, "Cantitate", XStringFormats.Center);
            AddText(page, font, 85, 27, 10, 3, "Valoare", XStringFormats.Center);
            // rect mare
            AddText(page, font, 05, 27, 90, 50, "", XStringFormats.Center);
            
            // rect mare de jos stanga
            AddText(page, font, 05, 69, 55, 8, "", XStringFormats.Center);
            // rect pentru total
            AddText(page, font3, 60, 69, 35, 3, "    Total de plata", XStringFormats.CenterLeft);
            AddText(page, font3, 60, 69, 35, 3, $"{saleDetails.TotalValue}           ", XStringFormats.CenterRight);
            // rect semnatura primire
            AddText(page, font2 , 60, 69, 35, 8, "  Semnatura de primire", XStringFormats.CenterLeft);
            //rect semnatura stanga
            
            AddText(page, font4, 05, 68, 25, 8, "Valabil fara semnatura si stampila \n conform L227/2015, \n Art.319(29)", XStringFormats.Center);
            AddText(page, font4, 05, 69, 25, 8, "conform L227/2015,", XStringFormats.Center);


            var row = 30;
            foreach (var item in saleDetails.Items)
            {
                AddText(page, font, 05, row, 20, 2, item.Code, XStringFormats.Center);
                AddText(page, font, 25, row, 30, 2, item.Name, XStringFormats.Center);
                AddText(page, font, 55, row, 10, 2, item.Unit, XStringFormats.Center);
                AddText(page, font, 65, row, 10, 2, item.UnitPrice.ToString(), XStringFormats.Center);
                AddText(page, font, 75, row, 10, 2, item.Quantity.ToString("0.##"), XStringFormats.Center);
                AddText(page, font, 85, row, 10, 2, item.Value.ToString("N2"), XStringFormats.Center);

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