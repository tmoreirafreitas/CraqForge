using CraqForge.DocuCraft.Layouts;
using MigraDoc.DocumentObjectModel;

namespace CraqForge.DocuCraft.Creations.Rtf.Layouts
{
    public class RtfDocumentStyleBuilder
    {
        private readonly RtfDocumentStyleOptions _style = new();

        public RtfDocumentStyleBuilder WithFont(string fontName)
        {
            _style.FontName = fontName;
            return this;
        }

        public RtfDocumentStyleBuilder WithFontSize(double pointSize)
        {
            _style.FontSizePt = pointSize;
            return this;
        }

        public RtfDocumentStyleBuilder WithLineSpacing(double pointSpacing)
        {
            _style.LineSpacingPt = pointSpacing;
            return this;
        }

        public RtfDocumentStyleBuilder WithLineSpacingRule(LineSpacingRule lineSpacingRule = LineSpacingRule.AtLeast)
        {
            _style.LineSpacingRule = lineSpacingRule;
            return this;
        }

        public RtfDocumentStyleBuilder WithOutlineLevel(OutlineLevel outlineLevel = OutlineLevel.BodyText)
        {
            _style.OutlineLevel = outlineLevel;
            return this;
        }

        public RtfDocumentStyleBuilder WithParagraphSpacing(double beforeCm, double afterCm)
        {
            _style.SpaceBeforeCm = beforeCm;
            _style.SpaceAfterCm = afterCm;
            return this;
        }

        public RtfDocumentStyleBuilder WithIndentation(double leftPt, double rightPt)
        {
            _style.LeftIndentPt = leftPt;
            _style.RightIndentPt = rightPt;
            return this;
        }

        public RtfDocumentStyleOptions Build() => _style;
    }
}
