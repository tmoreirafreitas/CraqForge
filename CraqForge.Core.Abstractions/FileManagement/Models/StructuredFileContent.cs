namespace CraqForge.Core.Abstractions.FileManagement.Models
{
    /// <summary>
    /// Representa um arquivo estruturado com dados adicionais, como variáveis substituíveis e tabelas.
    /// É útil para documentos gerados dinamicamente, como modelos de texto com preenchimento automático.
    /// </summary>
    public record StructuredFileContent : FileContent
    {
        /// <summary>
        /// Dicionário contendo variáveis e seus respectivos valores para substituição em modelos de documentos.
        /// A chave representa o nome da variável e o valor o texto a ser inserido.
        /// </summary>
        public IDictionary<string, string> VariablesDocuments { get; } = new Dictionary<string, string>();

        /// <summary>
        /// Lista de tabelas que podem ser inseridas no documento.
        /// Usado principalmente em documentos estruturados como relatórios ou contratos com dados tabulares.
        /// </summary>
        public IList<DataTable> Tables { get; } = [];
    }
}
