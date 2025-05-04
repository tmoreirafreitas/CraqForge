namespace CraqForge.Core.Abstractions.Validations
{
    public interface IDocumentFormatValidator
    {
        bool IsPdf(byte[] pdfBytes);
        bool IsDocx(byte[] docxBytes);
    }
}
