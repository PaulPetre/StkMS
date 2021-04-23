using System.IO;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using StkMS.Contracts;

namespace StkMS.Services
{
    public class ReportGenerator : IReportGenerator
    {
        public byte[] Generate()
        {
            var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Arial", 20, XFontStyle.Regular);

            gfx.DrawString("Hello World!", font, XBrushes.Black, new XRect(20, 20, page.Width, page.Height), XStringFormats.Center);

            return GetDocumentBytes(document);
        }

        //

        private static byte[] GetDocumentBytes(PdfDocument document)
        {
            using var ms = new MemoryStream();
            document.Save(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms.ToArray();
        }
    }
}