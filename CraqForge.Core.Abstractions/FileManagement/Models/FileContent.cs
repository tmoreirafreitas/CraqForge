namespace CraqForge.Core.Abstractions.FileManagement.Models
{
    /// <summary>
    /// Representa o conteúdo básico de um arquivo, contendo metadados essenciais e os dados binários.
    /// Pode ser usado tanto como modelo de entrada (para upload ou geração) quanto como resultado (para download ou leitura).
    /// </summary>
    public record FileContent
    {
        /// <summary>
        /// Nome do arquivo, incluindo a extensão (por exemplo: "documento.pdf").
        /// </summary>
        public string FileName { get; init; } = string.Empty;

        /// <summary>
        /// Tipo de conteúdo (MIME type), como "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", etc.
        /// </summary>
        public string ContentType { get; init; } = string.Empty;

        /// <summary>
        /// Extensão do arquivo (por exemplo: ".pdf", ".docx").
        /// </summary>
        public string Extension { get; init; } = string.Empty;

        /// <summary>
        /// Conteúdo do arquivo em formato de array de bytes.
        /// </summary>
        public byte[] Content { get; init; } = [];
    }
}
