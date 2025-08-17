using CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Pdf;
using CraqForge.DocuCraft.Extractions.Pdf;

namespace CraqForge.DocuCraft.Abstractions
{
    /// <summary>
    /// Interface especializada de <see cref="IPdfContentAnalyzer{T}"/> para uso com <see cref="PdfPageContent"/>.
    /// Inclui gerenciamento de recursos com <see cref="IDisposable"/>.
    /// </summary>
    public interface IPdfExtractor : IPdfContentAnalyzer<PdfPageContent>, IDisposable
    {
    }
}
