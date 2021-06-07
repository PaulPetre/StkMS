using System;
using System.Diagnostics.CodeAnalysis;
using PdfSharpCore.Fonts;
using StkMS.Properties;

namespace StkMS.Services
{
    public class FontResolver : IFontResolver
    {
        public string DefaultFontName => "Arial";

        public byte[] GetFont(string faceName) => Resources.ARIAL;

        [SuppressMessage("ReSharper", "ConvertIfStatementToSwitchStatement")]
        [SuppressMessage("ReSharper", "ConvertIfStatementToReturnStatement")]
        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (!familyName.Equals("Arial", StringComparison.CurrentCultureIgnoreCase))
                return null;

            if (isBold && isItalic)
                return new FontResolverInfo("Arial-BoldItalic.ttf");
            if (isBold)
                return new FontResolverInfo("Arial-Bold.ttf");
            if (isItalic)
                return new FontResolverInfo("Arial-Italic.ttf");
            return new FontResolverInfo("Arial-Regular.ttf");
        }
    }
}