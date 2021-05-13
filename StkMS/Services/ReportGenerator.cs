using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;
using PdfSharpCore.Pdf.IO;
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

            //var doc = new Document();
            //var section = doc.AddSection();

            //// Create footer
            //Paragraph paragraph = section.Footers.Primary.AddParagraph();
            //paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
            //paragraph.Format.Font.Size = 9;
            //paragraph.Format.Alignment = ParagraphAlignment.Center;

            AddText(page, font, 05, 10, 25, 5, "Code", XStringFormats.Center);
            AddText(page, font, 30, 10, 35, 5, "Name", XStringFormats.Center);
            AddText(page, font, 65, 10, 10, 5, "Unit", XStringFormats.Center);
            AddText(page, font, 75, 10, 10, 5, "Price", XStringFormats.Center);
            AddText(page, font, 85, 10, 10, 5, "Qty", XStringFormats.Center);


            var row = 15;
            foreach (var stock in batch)
            {
                AddText(page, font, 05, row, 25, 2, stock.Product.Code, XStringFormats.CenterLeft);
                AddText(page, font, 30, row, 35, 2, stock.Product.Name, XStringFormats.Center);
                AddText(page, font, 65, row, 10, 2, stock.Product.Unit, XStringFormats.Center);
                AddText(page, font, 75, row, 10, 2, stock.Product.UnitPrice.ToString("C"), XStringFormats.CenterRight);
                AddText(page, font, 85, row, 10, 2, stock.Quantity.ToString("N"), XStringFormats.CenterRight);

                row += 3;
            }
            //PaginaMigraDoc(document, font);
            //var docRenderer = new DocumentRenderer(doc);
            //docRenderer.PrepareDocument();
            return page;
        }

        #region PaginaMigraDoc
        //static void PaginaMigraDoc(PdfDocument document, XFont font)
        //{
        //    var page = document.AddPage();
        //    var gfxs = XGraphics.FromPdfPage(page);
        //    // HACK²
        //    gfxs.MUH = PdfFontEncoding.Unicode;

        //    gfxs.DrawString("The following paragraph was rendered using MigraDoc:", font, XBrushes.Black,
        //        new XRect(100, 100, page.Width - 200, 300), XStringFormats.Center);
        //    // You always need a MigraDoc document for rendering.
        //    var doc = new Document();
        //    var section = doc.AddSection();
        //    // Add a single paragraph with some text and format information.

        //    // Put a logo in the header
        //    Image image = section.Headers.Primary.AddImage("../../PowerBooks.png");
        //    image.Height = "2.5cm";
        //    image.LockAspectRatio = true;
        //    image.RelativeVertical = RelativeVertical.Line;
        //    image.RelativeHorizontal = RelativeHorizontal.Margin;
        //    image.Top = ShapePosition.Top;
        //    image.Left = ShapePosition.Right;
        //    image.WrapFormat.Style = WrapStyle.Through;

        //    // Create footer
        //    Paragraph paragraph = section.Footers.Primary.AddParagraph();
        //    paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
        //    paragraph.Format.Font.Size = 9;
        //    paragraph.Format.Alignment = ParagraphAlignment.Center;

        //    // Create the text frame for the address
        //    var addressFrame = section.AddTextFrame();
        //    addressFrame.Height = "3.0cm";
        //    addressFrame.Width = "7.0cm";
        //    addressFrame.Left = ShapePosition.Left;
        //    addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
        //    addressFrame.Top = "5.0cm";
        //    addressFrame.RelativeVertical = RelativeVertical.Page;

        //    // Put sender in address frame
        //    paragraph = addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
        //    paragraph.Format.Font.Name = "Times New Roman";
        //    paragraph.Format.Font.Size = 7;
        //    paragraph.Format.SpaceAfter = 3;

        //    // Add the print date field
        //    paragraph = section.AddParagraph();
        //    paragraph.Format.SpaceBefore = "8cm";
        //    paragraph.Style = "Reference";
        //    paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
        //    paragraph.AddTab();
        //    paragraph.AddText("Cologne, ");
        //    paragraph.AddDateField("dd.MM.yyyy");

        //    // Create the item table
        //    var table = section.AddTable();
        //    table.Style = "Table";
        //    table.Borders.Width = 0.25;
        //    table.Borders.Left.Width = 0.5;
        //    table.Borders.Right.Width = 0.5;
        //    table.Rows.LeftIndent = 0;

        //    // Before you can add a row, you must define the columns
        //    var column = table.AddColumn("1cm");
        //    column.Format.Alignment = ParagraphAlignment.Center;

        //    column = table.AddColumn("2.5cm");
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    column = table.AddColumn("3cm");
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    column = table.AddColumn("3.5cm");
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    column = table.AddColumn("2cm");
        //    column.Format.Alignment = ParagraphAlignment.Center;

        //    column = table.AddColumn("4cm");
        //    column.Format.Alignment = ParagraphAlignment.Right;

        //    // Create the header of the table
        //    var row = table.AddRow();
        //    row.HeadingFormat = true;
        //    row.Format.Alignment = ParagraphAlignment.Center;
        //    row.Format.Font.Bold = true;
        //    row.Cells[0].AddParagraph("Item");
        //    row.Cells[0].Format.Font.Bold = false;
        //    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[0].MergeDown = 1;
        //    row.Cells[1].AddParagraph("Title and Author");
        //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[1].MergeRight = 3;
        //    row.Cells[5].AddParagraph("Extended Price");
        //    row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
        //    row.Cells[5].MergeDown = 1;

        //    row = table.AddRow();
        //    row.HeadingFormat = true;
        //    row.Format.Alignment = ParagraphAlignment.Center;
        //    row.Format.Font.Bold = true;
        //    row.Cells[1].AddParagraph("Quantity");
        //    row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[2].AddParagraph("Unit Price");
        //    row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[3].AddParagraph("Discount (%)");
        //    row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
        //    row.Cells[4].AddParagraph("Taxable");
        //    row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

        //    table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, MigraDoc.DocumentObjectModel.Color.Empty);
        //}

        #endregion
        private static void AddText(PdfPage page, XFont font, int x0, int y0, int w, int h, string text, XStringFormat format)
        {
            using var gfx = XGraphics.FromPdfPage(page);

            var xScale = page.Width / 100.0;
            var yScale = page.Height / 100.0;

            gfx.DrawString(text, font, XBrushes.Black, new XRect(x0 * xScale, y0 * yScale, w * xScale, h * yScale), format);
            XPen pen = new XPen(XColors.Black);

            gfx.DrawRectangle(pen, x0 * xScale, y0 * yScale, w * xScale, h * yScale);
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