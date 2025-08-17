using MigraDoc.DocumentObjectModel;

namespace CraqForge.DocuCraft.Layouts
{
    public class RtfLayoutOptions
    {
        public double PageWidthCm { get; set; } = 9.01;
        public double PageHeightCm { get; set; } = 29.7;
        public double MarginTopCm { get; set; } = 1.01;
        public double MarginLeftCm { get; set; } = 1.01;
        public double MarginRightCm { get; set; } = 0;
        public double MarginBottomCm { get; set; } = 0;
        public Orientation Orientation { get; set; } = Orientation.Portrait;
        public PageFormat PageFormat { get; set; } = PageFormat.A4;
    }
}
