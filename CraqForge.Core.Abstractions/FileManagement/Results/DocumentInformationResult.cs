namespace CraqForge.Core.Abstractions.FileManagement.Results
{
    /// <summary>
    /// Representa as informações básicas extraídas de um documento genérico.
    /// </summary>
    public class DocumentInformationResult
    {
        /// <summary>
        /// Nome do arquivo.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de conteúdo (MIME type) do arquivo.
        /// </summary>
        public string ContentType { get; init; } = string.Empty;

        /// <summary>
        /// Extensão do arquivo (ex: .pdf, .docx).
        /// </summary>
        public string Extension { get; init; } = string.Empty;

        /// <summary>
        /// Título do documento, se disponível nos metadados.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Autor do documento, conforme metadados.
        /// </summary>
        public string Author { get; init; } = string.Empty;

        /// <summary>
        /// Último modificador registrado nos metadados do documento.
        /// </summary>
        public string Modifier { get; init; } = string.Empty;

        /// <summary>
        /// Assunto associado ao documento, conforme metadados.
        /// </summary>
        public string Subject { get; init; } = string.Empty;

        /// <summary>
        /// Palavras-chave definidas nos metadados do documento.
        /// </summary>
        public string Keywords { get; init; } = string.Empty;

        /// <summary>
        /// Data de criação do documento, se disponível.
        /// </summary>
        public DateTime? CreationDate { get; init; }

        /// <summary>
        /// Data da última modificação registrada nos metadados.
        /// </summary>
        public DateTime? ModificationDate { get; init; }
    }
}
