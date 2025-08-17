namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Word
{
    public interface IDocxExtractor : IDisposable
    {
        string ExtractTextFromFile(byte[] docxBytes);
        DocxInfo ExtractInfo();
    }
}
