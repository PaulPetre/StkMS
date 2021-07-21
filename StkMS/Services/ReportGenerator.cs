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
                .Select(batch => CreateInventoryPage(document, font, batch, inventory))
                .ToArray();

            return GetDocumentBytes(document);
        }
       
        public byte[] GenerateSaleReport(SaleDetailsViewModel saleDetails, Customer customer)
        {
            var font = new XFont("Arial", 10, XFontStyle.Regular);
            var document = new PdfDocument { PageLayout = PdfPageLayout.OneColumn };
            CreateSalePage(document, font, saleDetails, customer);

            return GetDocumentBytes(document);
        }
        private static PdfPage CreateInventoryPage(PdfDocument document, XFont font, IEnumerable<InventoryDetails> batch, Inventory inventory)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;
            var font2 = new XFont("Arial", 10, XFontStyle.Regular);

            AddReportHeader(page, font);
            AddReportHeader2(page, font2);
            AddReportHeader3(page, font2, inventory);

            AddText(page, font, 03, 20, 18, 5, "Cod Produs", XStringFormats.Center);
            AddText(page, font, 21, 20, 29, 5, "Nume Produs", XStringFormats.Center);
            AddText(page, font, 50, 20, 8, 5, "U.M", XStringFormats.Center);
            AddText(page, font, 58, 20, 10, 5, "StocS", XStringFormats.Center);
            AddText(page, font, 68, 20, 10, 5, "StocF", XStringFormats.Center);
            AddText(page, font, 78, 20, 10, 5, "PretV", XStringFormats.Center);
            AddText(page, font, 88, 20, 10, 5, "PretN", XStringFormats.Center);

            var row = 25;
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

            return page;
        }

        private static void CreateSalePage(PdfDocument document, XFont font, SaleDetailsViewModel saleDetails, Customer customer)
        {
            var page = document.AddPage();
            page.Size = PageSize.A4;
            XFont font2 = new XFont("Arial", 9);
            XFont font3 = new XFont("Arial", 10);
            XFont font4 = new XFont("Arial", 8);
            XFont font6 = new XFont("Arial", 9);

            AddInvoiceHeader(page, font);
            AddInvoiceHeader2(page, font2, saleDetails);
            AddInvoiceHeader3(page, font2);
            AddInvoiceHeader4(page, font6);
            AddInvoiceHeader5(page, font2);
            AddInvoiceHeader6(page, font2, customer);

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
           
            AddInvoiceFooter(page, font4);

            var row = 30;
            foreach (var item in saleDetails.Items)
            {
                AddText(page, font, 05, row, 20, 2, item.Code, XStringFormats.Center);
                AddText(page, font, 25, row, 30, 2, item.Name, XStringFormats.Center);
                AddText(page, font, 55, row, 10, 2, item.Unit, XStringFormats.Center);
                AddText(page, font, 65, row, 10, 2, item.UnitPrice.ToString("N2"), XStringFormats.Center);
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

        private static void AddReportHeader(PdfPage page, XFont font)
        {
            XFont font5 = new XFont("Arial", 20, XFontStyle.Bold);
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString("Lista de inventariere", font5, XBrushes.Black, new XRect(05, 40, page.Width, page.Height), XStringFormats.TopCenter);

        }
        private static void AddReportHeader2(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString("Inventar inceput la:", font, XBrushes.Black, new XRect(29, 100, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Inventar finalizat la:", font, XBrushes.Black, new XRect(29, 114, page.Width, page.Height), XStringFormats.TopLeft);
        }
        private static void AddReportHeader3(PdfPage page, XFont font, Inventory inventory)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString($"{inventory.StartDate}", font, XBrushes.Black, new XRect(120, 100, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{inventory.EndDate}", font, XBrushes.Black, new XRect(120, 114, page.Width, page.Height), XStringFormats.TopLeft);
        }
        private static void AddInvoiceHeader(PdfPage page, XFont font)
        {
            XFont font5 = new XFont("Arial", 30, XFontStyle.Bold);
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString("FACTURA", font5, XBrushes.Black, new XRect(05,40, page.Width, page.Height), XStringFormats.TopCenter);

        }
        private static void AddInvoiceHeader2(PdfPage page, XFont font, SaleDetailsViewModel saleDetails)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString($"Nr. facturii {saleDetails.Id}", font, XBrushes.Black, new XRect(01, 80, page.Width, page.Height), XStringFormats.TopCenter);
            gfx.DrawString($"Data: {saleDetails.FormatDateTime}", font, XBrushes.Black, new XRect(01, 88, page.Width, page.Height), XStringFormats.TopCenter);

        }
        private static void AddInvoiceHeader3(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString("Furnizor:", font, XBrushes.Black, new XRect(29, 80, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Capital social:", font, XBrushes.Black, new XRect(29, 91, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("CUI:", font, XBrushes.Black, new XRect(29, 102, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Adresa:", font, XBrushes.Black, new XRect(29, 113, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Cont:", font, XBrushes.Black, new XRect(29, 124, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Telefon:", font, XBrushes.Black, new XRect(29, 135, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Email:", font, XBrushes.Black, new XRect(29, 146, page.Width, page.Height), XStringFormats.TopLeft);

        }
        private static void AddInvoiceHeader4(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            XFont font6 = new XFont("Arial", 9);
            //header stanga
            gfx.DrawString("S.C. GeSTOC S.R.L", font6, XBrushes.Black, new XRect(90, 80, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("435034", font6, XBrushes.Black, new XRect(90, 91, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("234123", font6, XBrushes.Black, new XRect(90, 102, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Merilor, nr.2, Targoviste", font6, XBrushes.Black, new XRect(90, 113, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("RO73RDZG0000000563455", font6, XBrushes.Black, new XRect(90, 124, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("0723233233", font6, XBrushes.Black, new XRect(90, 135, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("www.gestoc@gmail.com", font6, XBrushes.Black, new XRect(90, 146, page.Width, page.Height), XStringFormats.TopLeft);

        }
        private static void AddInvoiceHeader5(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString("Client:", font, XBrushes.Black, new XRect(400, 80, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Nr. inregistrare:", font, XBrushes.Black, new XRect(400, 91, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("CUI:", font, XBrushes.Black, new XRect(400, 102, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Adresa:", font, XBrushes.Black, new XRect(400, 113, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Oras:", font, XBrushes.Black, new XRect(400, 124, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("Cont:", font, XBrushes.Black, new XRect(400, 135, page.Width, page.Height), XStringFormats.TopLeft);
        }
        private static void AddInvoiceHeader6(PdfPage page, XFont font, Customer customer)
        {
            using var gfx = XGraphics.FromPdfPage(page);
            //header mijloc
            gfx.DrawString($"{customer.Name}", font, XBrushes.Black, new XRect(450, 80, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("", font, XBrushes.Black, new XRect(450, 91, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{customer.CUI}", font, XBrushes.Black, new XRect(450, 102, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{customer.Address}", font, XBrushes.Black, new XRect(450, 113, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString($"{customer.City}", font, XBrushes.Black, new XRect(450, 124, page.Width, page.Height), XStringFormats.TopLeft);
            gfx.DrawString("", font, XBrushes.Black, new XRect(450, 135, page.Width, page.Height), XStringFormats.TopLeft);
        }
        private static void AddInvoiceFooter(PdfPage page, XFont font)
        {
            using var gfx = XGraphics.FromPdfPage(page);

            //footer partea stanga
            gfx.DrawString("Valabil fara", font, XBrushes.Black, new XRect(45, 181, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("semnatura si stampila", font, XBrushes.Black, new XRect(40, 189, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("conform L227/2015,", font, XBrushes.Black, new XRect(42, 198, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("Art.319(29)", font, XBrushes.Black, new XRect(45, 206 , page.Width, page.Height), XStringFormats.CenterLeft);

            //footer mijloc
            gfx.DrawString("Date privind expeditia:", font, XBrushes.Black, new XRect(130, 173, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("Numele delegatului:", font, XBrushes.Black, new XRect(130, 181, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("BI/CI : seria: nr. eliberat(a)", font, XBrushes.Black, new XRect(130, 189, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("Mijlocul de transport:", font, XBrushes.Black, new XRect(130, 198, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("Expedierea s-a facut in prezenta noastra", font, XBrushes.Black, new XRect(130, 207, page.Width, page.Height), XStringFormats.CenterLeft);
            gfx.DrawString("la data de", font, XBrushes.Black, new XRect(130, 215, page.Width, page.Height), XStringFormats.CenterLeft);

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