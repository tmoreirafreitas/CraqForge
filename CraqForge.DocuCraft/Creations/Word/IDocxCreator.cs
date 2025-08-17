using CraqForge.Core.Abstractions.FileManagement.Models;
using CraqForge.DocuCraft.Abstractions.FileManagement;

namespace CraqForge.DocuCraft.Creations.Word
{
    /// <summary>
    /// Performs variable substitutions in the DOCX file content and tables.
    /// </summary>
    /// <param name="dataFile">The data file containing the variables and tables.</param>
    /// <param name="cancellation">Token for canceling the operation.</param>
    /// <returns>A stream containing the file content with the replaced variables.</returns>
    public interface IDocxCreator : IDocumentCreator<StructuredFileContent>
    {
    }
}
