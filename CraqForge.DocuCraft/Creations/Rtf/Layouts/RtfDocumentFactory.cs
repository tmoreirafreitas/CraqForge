using CraqForge.DocuCraft.Layouts;
using MDoc = MigraDoc.DocumentObjectModel;

namespace CraqForge.DocuCraft.Creations.Rtf.Layouts
{
    public static class RtfDocumentFactory
    {
        public static MDoc.Document Create(RtfDocumentStyleOptions? styleOptions = null, RtfLayoutOptions? layoutOptions = null)
        {
            var document = new MDoc.Document();
            ConfigureStyles(document, styleOptions ?? new RtfDocumentStyleOptions());

            var section = document.AddSection();

            ConfigureLayout(section, layoutOptions ?? new RtfLayoutOptions());
            return document;
        }

        private static void ConfigureStyles(MDoc.Document document, RtfDocumentStyleOptions style)
        {
            var normal = document.Styles[MDoc.StyleNames.Normal];

            if (normal != null)
            {
                normal.Font.Name = style.FontName;
                normal.Font.Size = MDoc.Unit.FromPoint(style.FontSizePt);

                normal.ParagraphFormat.LeftIndent = MDoc.Unit.FromPoint(style.LeftIndentPt);
                normal.ParagraphFormat.RightIndent = MDoc.Unit.FromPoint(style.RightIndentPt);
                normal.ParagraphFormat.SpaceBefore = MDoc.Unit.FromCentimeter(style.SpaceBeforeCm);
                normal.ParagraphFormat.SpaceAfter = MDoc.Unit.FromCentimeter(style.SpaceAfterCm);
                normal.ParagraphFormat.LineSpacing = MDoc.Unit.FromPoint(style.LineSpacingPt);
                normal.ParagraphFormat.LineSpacingRule = MDoc.LineSpacingRule.AtLeast;
                normal.ParagraphFormat.OutlineLevel = MDoc.OutlineLevel.BodyText;
            }
        }

        private static void ConfigureLayout(MDoc.Section section, RtfLayoutOptions layout)
        {
            section.PageSetup.Orientation = layout.Orientation;
            section.PageSetup.PageFormat = layout.PageFormat;
            section.PageSetup.PageWidth = MDoc.Unit.FromCentimeter(layout.PageWidthCm);
            section.PageSetup.PageHeight = MDoc.Unit.FromCentimeter(layout.PageHeightCm);

            section.PageSetup.TopMargin = MDoc.Unit.FromCentimeter(layout.MarginTopCm);
            section.PageSetup.LeftMargin = MDoc.Unit.FromCentimeter(layout.MarginLeftCm);
            section.PageSetup.RightMargin = MDoc.Unit.FromCentimeter(layout.MarginRightCm);
            section.PageSetup.BottomMargin = MDoc.Unit.FromCentimeter(layout.MarginBottomCm);
        }
    }
}