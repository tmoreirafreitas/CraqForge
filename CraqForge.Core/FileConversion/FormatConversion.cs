using CraqForge.Core.Abstractions.FileManagement;

namespace CraqForge.Core.FileConversion
{
    internal class FormatConversion : IFormatConversion
    {
        private static readonly Dictionary<FormatConversionType, string> ContentTypes = new()
        {
            { FormatConversionType.Undefined, "application/octet-stream" },
            { FormatConversionType.Dot, "application/msword" },
            { FormatConversionType.Dotx, "application/vnd.openxmlformats-officedocument.wordprocessingml.template" },
            { FormatConversionType.Word97, "application/msword" },
            { FormatConversionType.Docx, "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { FormatConversionType.Xls, "application/vnd.ms-excel" },
            { FormatConversionType.Xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { FormatConversionType.Ppt, "application/vnd.ms-powerpoint" },
            { FormatConversionType.Pptx, "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
            { FormatConversionType.Csv, "text/csv" },
            { FormatConversionType.Html, "text/html" },
            { FormatConversionType.Txt, "text/plain" },
            { FormatConversionType.Odt, "application/vnd.oasis.opendocument.text" },
            { FormatConversionType.Ods, "application/vnd.oasis.opendocument.spreadsheet" },
            { FormatConversionType.Rtf, "application/rtf" },
            { FormatConversionType.Pdf, "application/pdf" },
            { FormatConversionType.Odp, "application/vnd.oasis.opendocument.presentation" },
            { FormatConversionType.Fodt, "application/vnd.oasis.opendocument.text-flat-xml" },
            { FormatConversionType.Odsx, "application/vnd.ms-excel.sheet.macroEnabled.12" },
            { FormatConversionType.Odtm, "application/vnd.oasis.opendocument.text-master" },
            { FormatConversionType.LaTeX, "application/x-latex" },
            { FormatConversionType.Markdown, "text/markdown" },
            { FormatConversionType.OpenOffice, "application/vnd.sun.xml.writer" },
            { FormatConversionType.Tif, "image/tiff" },
            { FormatConversionType.Tiff, "image/tiff" },
            { FormatConversionType.Gif, "image/gif" },
            { FormatConversionType.Bmp, "image/bmp" }
        };

        private static readonly Dictionary<string, FormatConversionType> ExtensionToFormatMap = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".dot", FormatConversionType.Dot },
            { ".dotx", FormatConversionType.Dotx },
            { ".doc", FormatConversionType.Word97 },
            { ".docx", FormatConversionType.Docx },
            { ".xls", FormatConversionType.Xls },
            { ".xlsx", FormatConversionType.Xlsx },
            { ".ppt", FormatConversionType.Ppt },
            { ".pptx", FormatConversionType.Pptx },
            { ".csv", FormatConversionType.Csv },
            { ".html", FormatConversionType.Html },
            { ".htm", FormatConversionType.Html },
            { ".txt", FormatConversionType.Txt },
            { ".odt", FormatConversionType.Odt },
            { ".ods", FormatConversionType.Ods },
            { ".rtf", FormatConversionType.Rtf },
            { ".pdf", FormatConversionType.Pdf },
            { ".odp", FormatConversionType.Odp },
            { ".fodt", FormatConversionType.Fodt },
            { ".odsx", FormatConversionType.Odsx },
            { ".odtm", FormatConversionType.Odtm },
            { ".tex", FormatConversionType.LaTeX },
            { ".md", FormatConversionType.Markdown },
            { ".sxw", FormatConversionType.OpenOffice },
            { ".tif", FormatConversionType.Tif },
            { ".tiff", FormatConversionType.Tiff },
            { ".gif", FormatConversionType.Gif },
            { ".bmp", FormatConversionType.Bmp },
        };

        public string GetContentType(FormatConversionType format)
        {
            return ContentTypes.TryGetValue(format, out var contentType) ? contentType : "application/octet-stream";
        }

        public FormatConversionType GetFormatType(string fileName)
        {
            var extension = Path.GetExtension(fileName)?.ToLower();
            return extension != null && ExtensionToFormatMap.TryGetValue(extension, out var format)
                ? format
                : FormatConversionType.Undefined;
        }

        public string GetExtensionType(FormatConversionType format)
        {
            var extension = ExtensionToFormatMap.FirstOrDefault(x => x.Value == format);
            return extension.Key;
        }
    }
}
