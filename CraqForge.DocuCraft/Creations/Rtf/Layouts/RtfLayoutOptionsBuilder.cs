using CraqForge.DocuCraft.Layouts;
using MigraDoc.DocumentObjectModel;

namespace CraqForge.DocuCraft.Creations.Rtf.Layouts
{
    public class RtfLayoutOptionsBuilder
    {
        private readonly RtfLayoutOptions _layout = new();

        public RtfLayoutOptionsBuilder WithOrientation(Orientation orientation = Orientation.Portrait)
        {
            _layout.Orientation = orientation;
            return this;
        }

        public RtfLayoutOptionsBuilder WithPageFormat(PageFormat pageFormat = PageFormat.A4)
        {
            _layout.PageFormat = pageFormat;
            return this;
        }

        public RtfLayoutOptionsBuilder WithPageSize(double widthCm, double heightCm)
        {
            _layout.PageWidthCm = widthCm;
            _layout.PageHeightCm = heightCm;
            return this;
        }

        public RtfLayoutOptionsBuilder WithMargins(double top, double left, double right, double bottom)
        {
            _layout.MarginTopCm = top;
            _layout.MarginLeftCm = left;
            _layout.MarginRightCm = right;
            _layout.MarginBottomCm = bottom;
            return this;
        }

        public RtfLayoutOptions Build() => _layout;
    }
}
