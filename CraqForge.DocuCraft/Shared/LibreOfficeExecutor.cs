using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace CraqForge.DocuCraft.Shared
{
    internal sealed class LibreOfficeExecutor(ILogger logger)
    {
        public async Task<int> ExecuteAsync(string sofficePath, string arguments, bool useDocker, string dockerCommand, CancellationToken cancellationToken)
        {
            string fileName = useDocker ? "docker" : sofficePath;
            string finalArgs = useDocker ? dockerCommand : arguments;

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = finalArgs,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            logger?.LogDebug("Executando: {FileName} {Arguments}", fileName, finalArgs);

            try
            {
                process.Start();
                string stdOut = await process.StandardOutput.ReadToEndAsync(cancellationToken);
                string stdErr = await process.StandardError.ReadToEndAsync(cancellationToken);
                await process.WaitForExitAsync(cancellationToken);

                logger?.LogDebug("Saída padrão: {StdOut}", stdOut);
                if (!string.IsNullOrWhiteSpace(stdErr))
                    logger?.LogWarning("Erro padrão: {StdErr}", stdErr);

                if (process.ExitCode != 0)
                    logger?.LogError("Processo terminou com erro: {ExitCode}", process.ExitCode);

                return process.ExitCode;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Erro ao executar processo LibreOffice.");
                throw;
            }
        }
    }
}
