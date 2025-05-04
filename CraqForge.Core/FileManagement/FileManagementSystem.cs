using CraqForge.Core.Abstractions.FileManagement;
using Microsoft.Extensions.Logging;

namespace CraqForge.Core.FileManagement
{
    internal abstract class FileManagementSystem(ILoggerFactory loggerFactory) : IFileManagementSystem
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<FileManagementSystem>();
        public virtual async Task<byte[]> DownloadAsync(string filePathName, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(filePathName))
            {
                _logger?.LogWarning("O caminho do arquivo está vazio ou nulo.");
                throw new ArgumentException("O caminho do arquivo não pode ser nulo ou vazio.", nameof(filePathName));
            }

            if (!File.Exists(filePathName))
            {
                _logger?.LogWarning("Tentativa de acesso a um arquivo inexistente: {FilePath}", filePathName);
                throw new FileNotFoundException($"Arquivo não encontrado: {filePathName}");
            }
                       
            _logger?.LogInformation("Iniciando download do arquivo: {FilePath}", filePathName);
            
            try
            {
                var ms = new MemoryStream();
                using var fs = File.OpenRead(filePathName);
                await fs.CopyToAsync(ms, cancellationToken);
                ms.Seek(0, SeekOrigin.Begin);

                var fileName = Path.GetFileName(filePathName);
                _logger?.LogInformation("Download concluído. Arquivo: {FileName}, Tamanho: {FileSize} bytes",
                    fileName, ms.Length);

                return ms.ToArray();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao fazer o download do arquivo: {FilePath}", filePathName);
                throw;
            }
        }

        public string CreateFolder(string folderName)
        {
            if (string.IsNullOrWhiteSpace(folderName))
            {
                _logger?.LogWarning("Nome da pasta inválido ou vazio.");
                throw new ArgumentException("O nome da pasta não pode ser nulo ou vazio.", nameof(folderName));
            }

            try
            {
                if (!Directory.Exists(folderName))
                {
                    Directory.CreateDirectory(folderName);
                    _logger?.LogInformation("Pasta criada com sucesso: {FolderName}", folderName);
                }
                else
                {
                    _logger?.LogInformation("A pasta já existe: {FolderName}", folderName);
                }

                return folderName;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao criar a pasta: {FolderName}", folderName);
                throw;
            }
        }

        public string NewTempFileName(string tempPath)
        {
            if (string.IsNullOrWhiteSpace(tempPath))
            {
                _logger?.LogWarning("O caminho do diretório temporário é inválido ou vazio.");
                throw new ArgumentException("O caminho do diretório temporário não pode ser nulo ou vazio.", nameof(tempPath));
            }

            try
            {
                string fileName = Path.Combine(tempPath, $"{Guid.NewGuid():N}").ToUpper();
                _logger?.LogInformation("Novo arquivo temporário gerado: {FileName}", fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao gerar o nome do arquivo temporário no diretório: {TempPath}", tempPath);
                throw;
            }
        }

        public async Task SaveFileAsync(byte[] fileBytes, string filePath)
        {
            if (fileBytes == null || fileBytes.Length == 0)
                throw new ArgumentException("O array de bytes está vazio.");

            string directory = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, FileOptions.Asynchronous);
            using BufferedStream bufferedStream = new(fileStream, bufferSize: 81920);

            await bufferedStream.WriteAsync(fileBytes);
        }
    }
}
