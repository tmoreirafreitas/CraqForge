using CraqForge.Core.Abstractions.FileManagement.Models;
using CraqForge.DocuCraft.Creations.Rtf.Layouts;
using Microsoft.Extensions.Logging;
using System.Text;
using MDoc = MigraDoc.DocumentObjectModel;

namespace CraqForge.DocuCraft.Creations.Rtf
{
    internal sealed class RtfDocumentCreator(RtfDocumentStyleBuilder rtfDocumentStyleBuilder, RtfLayoutOptionsBuilder rtfLayoutOptionsBuilder, ILogger<RtfDocumentCreator> logger) : IRtfDocumentCreator
    {
        public async Task CreateAsync(IReadOnlyList<DocumentMetadata> metadata, string fileName, CancellationToken cancellationToken = default)
        {
            try
            {
                await Task.Run(() =>
                {
                    var style = rtfDocumentStyleBuilder
                    .WithFont("Arial")
                    .WithFontSize(7)
                    .WithIndentation(0, 0)
                    .WithParagraphSpacing(0, 0)
                    .WithLineSpacing(8)
                    .WithLineSpacingRule()
                    .WithOutlineLevel()
                    .Build();

                    var layout = rtfLayoutOptionsBuilder
                    .WithOrientation()
                    .WithPageFormat()
                    .WithPageSize(9.01, 29.7)
                    .WithMargins(1.01, 1.01, 0, 0)
                    .Build();

                    var document = RtfDocumentFactory.Create(style, layout);
                    var section = document.LastSection;

                    MapMetadataToRtfDocument(section, metadata, cancellationToken: cancellationToken);

                    var rtfRenderer = new MigraDoc.RtfRendering.RtfDocumentRenderer();
                    rtfRenderer.Render(document, fileName, null!);
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                logger?.LogWarning("RTF document rendering was canceled.");
                throw;
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Error rendering RTF document.");
                throw;
            }
        }

        private void MapMetadataToRtfDocument(MDoc.Section section, IReadOnlyList<DocumentMetadata> fields, int index = 0, bool signatureStarted = false, CancellationToken cancellationToken = default)
        {
            foreach (var field in fields)
            {
                cancellationToken.ThrowIfCancellationRequested();

                logger?.LogInformation("Processing metadata of type {Tipo}: {Titulo}", field.Type, field.Content);
                var paragraph = section.AddParagraph();
                paragraph.Style = MDoc.StyleNames.Normal;

                switch (field.Type)
                {
                    case MetadataType.Header:
                        AddHeader(paragraph, field);
                        break;

                    case MetadataType.Information:
                    case MetadataType.Paragraph:
                        AddInformation(paragraph, field, ref index, cancellationToken);
                        break;

                    case MetadataType.Signature:
                        AddSignature(section, ref signatureStarted, paragraph, field);
                        break;
                }

                if (field.Childs.Any())
                {
                    logger?.LogInformation("Field has {QtdFilhos} children. Mapping recursively...", field.Childs.Count);
                    MapMetadataToRtfDocument(section, field.Childs, index, signatureStarted, cancellationToken);
                }
            }
        }

        private void AddHeader(MDoc.Paragraph paragraph, DocumentMetadata campo)
        {
            if (!string.IsNullOrEmpty(campo.Content))
            {
                paragraph.Format.Alignment = MDoc.ParagraphAlignment.Center;
                paragraph.AddFormattedText(campo.Content, MDoc.TextFormat.Bold);
                if (paragraph.Section != null)
                    _ = paragraph.Section.AddParagraph();

                logger?.LogInformation("Header added: {Header}", campo.Content);
            }
        }

        private void AddInformation(MDoc.Paragraph paragrath, DocumentMetadata field, ref int indice, CancellationToken cancellation = default)
        {
            ArgumentNullException.ThrowIfNull(paragrath);
            ArgumentNullException.ThrowIfNull(field);

            paragrath.Format.Alignment = MDoc.ParagraphAlignment.Justify;
            string info = FormatContent(field, ref indice);

            logger?.LogDebug("Informação formatada: {Info}", info);

            var segments = ExtractFormattedSegments(info);
            logger?.LogDebug("Segmentos formatados encontrados: {Qtd}", segments.Count);

            foreach (var segment in segments)
            {
                cancellation.ThrowIfCancellationRequested();

                if (segment.IsBold || segment.IsItalic)
                {
                    var format = MDoc.TextFormat.NoUnderline;
                    if (segment.IsBold) format |= MDoc.TextFormat.Bold;
                    if (segment.IsItalic) format |= MDoc.TextFormat.Italic;

                    paragrath.AddFormattedText(segment.Text, format);
                }
                else
                {
                    paragrath.AddText(segment.Text);
                }
            }

            if (field.Parent != null)
                paragrath.Format.LeftIndent = MDoc.Unit.FromCentimeter(0.67);

            if (paragrath.Section != null)
                _ = paragrath.Section.AddParagraph();

            if (field.Childs.Any())
            {
                logger?.LogDebug("Campo possui {QtdFilhos} filhos. Iniciando mapeamento recursivo...", field.Childs.Count);
                MapMetadataToRtfDocument(paragrath.Section, field.Childs, cancellationToken: cancellation);
            }
        }

        private void AddSignature(MDoc.Section section, ref bool signatureStarted, MDoc.Paragraph paragraph, DocumentMetadata campo)
        {
            if (!signatureStarted)
            {
                AdjustSpacingBeforeSignature(section);
                signatureStarted = true;
            }

            paragraph.Format.Alignment = MDoc.ParagraphAlignment.Center;
            paragraph.AddFormattedText(campo.Content, MDoc.TextFormat.Bold);

            logger?.LogInformation("Signature added: {Signature}", campo.Content);

            _ = section.AddParagraph();
            _ = section.AddParagraph();
        }

        private void AdjustSpacingBeforeSignature(MDoc.Section section)
        {
            var paragraphs = section.Elements.OfType<MDoc.Paragraph>().ToList();
            if (paragraphs.Count >= 2)
            {
                var penultimate = paragraphs[^2];
                penultimate.Format.SpaceAfter = MDoc.Unit.FromPoint(24);
                logger?.LogInformation("Spacing adjusted before signature (24pt).");
            }
        }

        private static string FormatContent(DocumentMetadata field, ref int index, bool addIndex = false)
        {
            field.Index = ++index;

            if (string.IsNullOrEmpty(field.Content))
                return field.Content ?? string.Empty;

            if (addIndex && field.Parent != null && field.Parent.Index.HasValue)
                return $"{field.Parent?.Index}.{field.Index.Value} - {field.Content}";

            if (addIndex && field.Index.HasValue)
                return $"{field.Index.Value} - {field.Content}";

            return field.Content ?? string.Empty;
        }

        public static List<TextFormatted> ExtractFormattedSegments(string input)
        {
            var result = new List<TextFormatted>();
            var sb = new StringBuilder();
            bool bold = false, italic = false;

            for (int i = 0; i < input.Length;)
            {
                if (input[i] == '\\')
                {
                    if (input.Substring(i).StartsWith(@"\b0"))
                    {
                        AddSegment(sb, bold, italic, result);
                        bold = false;
                        i += 3;
                        continue;
                    }

                    if (input.Substring(i).StartsWith(@"\b"))
                    {
                        AddSegment(sb, bold, italic, result);
                        bold = true;
                        i += 2;
                        continue;
                    }

                    if (input.Substring(i).StartsWith(@"\i0"))
                    {
                        AddSegment(sb, bold, italic, result);
                        italic = false;
                        i += 3;
                        continue;
                    }

                    if (input.Substring(i).StartsWith(@"\i"))
                    {
                        AddSegment(sb, bold, italic, result);
                        italic = true;
                        i += 2;
                        continue;
                    }
                }

                sb.Append(input[i]);
                i++;
            }

            AddSegment(sb, bold, italic, result);

            return result;
        }

        private static void AddSegment(StringBuilder sb, bool bold, bool italic, List<TextFormatted> result)
        {
            if (sb.Length == 0) return;

            result.Add(new TextFormatted
            {
                Text = sb.ToString(),
                IsBold = bold,
                IsItalic = italic
            });

            sb.Clear();
        }
    }
}