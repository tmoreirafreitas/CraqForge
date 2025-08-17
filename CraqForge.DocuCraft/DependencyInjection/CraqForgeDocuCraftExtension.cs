using CraqForge.Core.DependencyInjection;
using CraqForge.DocuCraft.Abstractions;
using CraqForge.DocuCraft.Abstractions.FileManagement;
using CraqForge.DocuCraft.Abstractions.FileManagement.Conversions;
using CraqForge.DocuCraft.Conversion;
using CraqForge.DocuCraft.Creations.Rtf;
using CraqForge.DocuCraft.Extractions.Ocr.Images;
using CraqForge.DocuCraft.Extractions.Ocr.Tesseract;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CraqForge.DocuCraft.DependencyInjection
{
    public static class CraqForgeDocuCraftExtension
    {
        public static IServiceCollection AddDocuCraf(this IServiceCollection services)
        {
            services.AddCraqForgeCore();
            services.TryAddSingleton<OcrDataManager>();
            services.TryAddSingleton<ILibreOfficePathResolver, LibreOfficePathResolver>();
            services.TryAddSingleton<ILibreOfficeConverter, LibreOfficeConverter>();
            services.TryAddSingleton<IHtmlToPdfConverter, HtmlToPdfConverter>();
            services.TryAddSingleton<IRtfDocumentCreator, RtfDocumentCreator>();
            services.TryAddSingleton<IImageProcessingForTesseract, ImageProcessingForTesseract>();            
            services.TryAddSingleton<IOcrServiceFactory, OcrServiceFactory>();            

            return services;
        }
    }
}
