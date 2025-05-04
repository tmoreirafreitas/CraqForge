﻿using CraqForge.Core.Abstractions.FileManagement;
using CraqForge.Core.Abstractions.Identification;
using CraqForge.Core.Abstractions.Validations;
using CraqForge.Core.FileConversion;
using CraqForge.Core.FileManagement;
using CraqForge.Core.Identification;
using CraqForge.Core.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace CraqForge.Core.DependencyInjection
{
    public static class CraqForgeCoreServicesExtension
    {
        public static IServiceCollection AddCraqForgeCore(this IServiceCollection services)
        {
            services.AddSingleton<IDocumentFormatValidator, DocumentFormatValidator>();
            services.AddSingleton<IIdentifierFactory, IdentifierFactory>();
            services.AddSingleton<IFormatConversion, FormatConversion>();
            services.AddScoped<IFileManagementSystem, FileManagementSystem>();
            return services;
        }
    }
}
