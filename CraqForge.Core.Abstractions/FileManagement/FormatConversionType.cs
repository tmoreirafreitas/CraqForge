using System.Runtime.Serialization;

namespace CraqForge.Core.Abstractions.FileManagement
{
    /// <summary>
    /// Defines the supported format types for document conversion.
    /// </summary>
    public enum FormatConversionType
    {
        [EnumMember(Value = "Undefined")] Undefined,
        [EnumMember(Value = "dot")] Dot,
        [EnumMember(Value = "dotx")] Dotx,
        [EnumMember(Value = "doc")] Word97,
        [EnumMember(Value = "docx")] Docx,
        [EnumMember(Value = "xls:\"MS Excel 97\"")] Xls,
        [EnumMember(Value = "xlsx:\"Calc MS Excel 2007 XML\"")] Xlsx,
        [EnumMember(Value = "ppt:\"MS PowerPoint 97\"")] Ppt,
        [EnumMember(Value = "pptx:\"Impress MS PowerPoint 2007 XML\"")] Pptx,
        [EnumMember(Value = "Text - txt - csv (StarCalc)")] Csv,
        [EnumMember(Value = "html")] Html,
        [EnumMember(Value = "txt:Text (encoded):UTF8")] Txt,
        [EnumMember(Value = "odt")] Odt,
        [EnumMember(Value = "ods")] Ods,
        [EnumMember(Value = "rtf")] Rtf,
        [EnumMember(Value = "pdf")] Pdf,
        [EnumMember(Value = "odp")] Odp,
        [EnumMember(Value = "fodt")] Fodt,
        [EnumMember(Value = "odsx")] Odsx,
        [EnumMember(Value = "odtm")] Odtm,
        [EnumMember(Value = "latex")] LaTeX,
        [EnumMember(Value = "md")] Markdown,
        [EnumMember(Value = "sxw")] OpenOffice,
        [EnumMember(Value = "tif")] Tif,
        [EnumMember(Value = "tiff")] Tiff,
        [EnumMember(Value = "gif")] Gif,
        [EnumMember(Value = "bmp")] Bmp,
    }
}
