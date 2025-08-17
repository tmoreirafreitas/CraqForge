using CraqForge.DocuCraft.Abstractions.FileManagement;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace CraqForge.DocuCraft.Conversion
{
    internal sealed class LibreOfficePathResolver(ILogger<LibreOfficePathResolver> logger) : ILibreOfficePathResolver
    {
        private const string LinuxDefaultPath = "/usr/bin/soffice";
        private const string MacDefaultPath = "/Applications/LibreOffice.app/Contents/MacOS/soffice";
        private const string WindowsDefaultPath = "C:\\Program Files\\LibreOffice\\program\\soffice.exe";
        private const string WindowsX86Path = "C:\\Program Files (x86)\\LibreOffice\\program\\soffice.exe";
        private const string DockerImage = "libreoffice-docker:latest";

        public bool ShouldUseDocker => !TryResolveSofficePath(out _);

        public string GetSofficePath()
        {
            logger?.LogInformation("Resolvendo o caminho do executável do LibreOffice (soffice)...");

            if (TryResolveSofficePath(out string? path))
            {
                logger?.LogInformation("LibreOffice resolvido em: {Path}", path);
                return path!;
            }

            logger?.LogError("LibreOffice não encontrado no sistema.");
            throw new FileNotFoundException("LibreOffice não encontrado.");
        }

        private static bool TryResolveSofficePath(out string? resolvedPath)
        {
            string? envPath = Environment.GetEnvironmentVariable("SOFFICE_PATH");
            if (!string.IsNullOrWhiteSpace(envPath) && File.Exists(envPath))
            {
                resolvedPath = envPath;
                return true;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (File.Exists(WindowsDefaultPath))
                {
                    resolvedPath = WindowsDefaultPath;
                    return true;
                }

                if (File.Exists(WindowsX86Path))
                {
                    resolvedPath = WindowsX86Path;
                    return true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (File.Exists(LinuxDefaultPath))
                {
                    resolvedPath = LinuxDefaultPath;
                    return true;
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (File.Exists(MacDefaultPath))
                {
                    resolvedPath = MacDefaultPath;
                    return true;
                }
            }

            resolvedPath = null;
            return false;
        }

        public bool IsLibreOfficeInstalled()
        {
            return TryResolveSofficePath(out _);
        }

        public string GetDockerCommand(string inputFile, string outputDir, string formatDescription)
        {
            string inputDir = Path.GetDirectoryName(inputFile)!;
            string inputFileName = Path.GetFileName(inputFile);

            return $"run --rm -v \"{inputDir}:/in\" -v \"{outputDir}:/out\" {DockerImage} " +
                   $"libreoffice --headless --convert-to {formatDescription} \"/in/{inputFileName}\" --outdir \"/out\"";
        }
    }
}
