using CraqForge.Core.Abstractions.FileManagement.Models;

namespace CraqForge.DocuCraft.Creations.Rtf
{
    public interface IRtfDocumentCreator
    {
        Task CreateAsync(IReadOnlyList<DocumentMetadata> metadata, string fileName, CancellationToken cancellationToken = default);
    }
}
