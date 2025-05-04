namespace CraqForge.Core.Abstractions.FileManagement
{
    /// <summary>
    /// Interface responsável por operações de manipulação de arquivos no sistema de arquivos.
    /// Fornece métodos para download, criação de pastas, geração de nomes temporários e armazenamento de arquivos.
    /// </summary>
    public interface IFileManagementSystem
    {
        /// <summary>
        /// Baixa um arquivo a partir do caminho fornecido.
        /// O conteúdo do arquivo é retornado como um array de bytes, permitindo que o consumidor defina a estrutura de retorno, se necessário.
        /// </summary>
        /// <param name="filePathName">Caminho completo do arquivo a ser baixado.</param>
        /// <param name="cancellationToken">Token de cancelamento opcional para cancelar a operação.</param>
        /// <returns>O conteúdo do arquivo como array de bytes.</returns>
        /// <exception cref="ArgumentException">Lançado quando o caminho do arquivo está vazio ou nulo.</exception>
        /// <exception cref="FileNotFoundException">Lançado quando o arquivo não é encontrado no caminho fornecido.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro ao tentar baixar o arquivo.</exception>
        Task<byte[]> DownloadAsync(string filePathName, CancellationToken cancellationToken = default);

        /// <summary>
        /// Cria uma pasta no sistema de arquivos, caso ela ainda não exista.
        /// </summary>
        /// <param name="folderName">Nome da pasta a ser criada.</param>
        /// <returns>O caminho completo da pasta criada.</returns>
        /// <exception cref="ArgumentException">Lançado quando o nome da pasta é inválido ou vazio.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro ao criar a pasta.</exception>
        string CreateFolder(string folderName);

        /// <summary>
        /// Gera um nome único para um arquivo temporário.
        /// O nome é baseado em um GUID e no diretório temporário especificado.
        /// </summary>
        /// <param name="tempPath">Caminho do diretório temporário onde o arquivo será criado.</param>
        /// <returns>O nome completo do arquivo temporário gerado.</returns>
        /// <exception cref="ArgumentException">Lançado quando o caminho do diretório é inválido ou vazio.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro ao gerar o nome do arquivo.</exception>
        string NewTempFileName(string tempPath);

        /// <summary>
        /// Salva um arquivo no sistema de arquivos a partir de um array de bytes.
        /// </summary>
        /// <param name="fileBytes">Conteúdo do arquivo como array de bytes.</param>
        /// <param name="filePath">Caminho completo onde o arquivo será salvo.</param>
        /// <exception cref="ArgumentException">Lançado quando o conteúdo do arquivo está vazio.</exception>
        /// <exception cref="Exception">Lançado quando ocorre um erro ao salvar o arquivo.</exception>
        Task SaveFileAsync(byte[] fileBytes, string filePath);
    }
}
