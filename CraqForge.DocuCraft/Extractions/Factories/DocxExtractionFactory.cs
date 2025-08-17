using CraqForge.DocuCraft.Abstractions.FileManagement.Extractions.Word;
using CraqForge.DocuCraft.Extractions.Word;

namespace CraqForge.DocuCraft.Extractions.Factories
{
    /// <summary>
    /// Factory helper para criar instâncias de <see cref="IDocxExtractor"/>.
    /// </summary>
    public static class DocxExtractionFactory
    {
        /// <summary>
        /// Cria um <see cref="DocxExtractor"/> a partir do conteúdo do DOCX em memória.
        /// </summary>
        /// <param name="docxContent">Conteúdo do DOCX como array de bytes.</param>
        /// <returns>Instância de <see cref="IDocxExtractor"/> pronta para uso.</returns>
        /// <exception cref="ArgumentException">Se o conteúdo for nulo ou vazio.</exception>
        public static IDocxExtractor Create(byte[] docxContent)
        {
            ValidateDocxContent(docxContent);
            return new DocxExtractor(docxContent);
        }

        /// <summary>
        /// Valida se o conteúdo do DOCX é válido (não nulo e não vazio).
        /// </summary>
        /// <param name="docxContent">Conteúdo do DOCX.</param>
        /// <exception cref="ArgumentException">Se inválido.</exception>
        private static void ValidateDocxContent(byte[] docxContent)
        {
            if (docxContent == null || docxContent.Length == 0)
                throw new ArgumentException("O DOCX não pode ser nulo ou vazio.", nameof(docxContent));
        }
    }
}
