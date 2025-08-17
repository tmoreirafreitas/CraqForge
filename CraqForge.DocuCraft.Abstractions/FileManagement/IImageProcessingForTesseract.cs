namespace CraqForge.DocuCraft.Abstractions.FileManagement
{
    public interface IImageProcessingForTesseract
    {
        Task<byte[]> PreprocessImage(byte[] imageBytes);
    }
}
