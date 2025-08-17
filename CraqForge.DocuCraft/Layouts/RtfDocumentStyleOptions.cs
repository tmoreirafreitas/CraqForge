using MigraDoc.DocumentObjectModel;

namespace CraqForge.DocuCraft.Layouts
{
    public record RtfDocumentStyleOptions
    {
        public string FontName { get; set; } = "Arial";
        public double FontSizePt { get; set; } = 7;
        public double LineSpacingPt { get; set; } = 8;
        public double SpaceBeforeCm { get; set; } = 0;
        public double SpaceAfterCm { get; set; } = 0;
        public double LeftIndentPt { get; set; } = 0;
        public double RightIndentPt { get; set; } = 0;
        public LineSpacingRule LineSpacingRule { get; set; } = LineSpacingRule.AtLeast;
        public OutlineLevel OutlineLevel { get; set; } = OutlineLevel.BodyText;
    }
}
