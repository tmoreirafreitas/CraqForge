namespace CraqForge.DocuCraft.Abstractions.FileManagement
{
    public interface IDocumentCreator<T>
    {
        /// <summary>
        /// Performs variable substitutions in the DOCX file content and tables.
        /// </summary>
        /// <param name="dataFile">The data file containing the variables and tables.</param>
        /// <param name="cancellation">Token for canceling the operation.</param>
        /// <returns>A stream containing the file content with the replaced variables.</returns>
        Task<byte[]> CreateAsync(T dataFile, CancellationToken cancellation = default);
    }
}
