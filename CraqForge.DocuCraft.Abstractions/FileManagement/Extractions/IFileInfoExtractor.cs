namespace CraqForge.DocuCraft.Abstractions.FileManagement.Extractions
{
    public interface IFileInfoExtractor<T> where T : class
    {
        T Extract(byte[] fileBytes);
    }
}
