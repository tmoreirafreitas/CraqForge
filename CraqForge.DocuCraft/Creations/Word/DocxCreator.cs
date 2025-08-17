using CraqForge.Core.Abstractions.FileManagement;
using CraqForge.Core.Abstractions.FileManagement.Models;
using CraqForge.DocuCraft.Shared;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace CraqForge.DocuCraft.Creations.Word
{
    internal class DocxCreator(IFormatConversion formatConversion) : IDocxCreator
    {
        public Task<byte[]> CreateAsync(StructuredFileContent dataFile, CancellationToken cancellation = default)
        {
            if (dataFile.Content == null)
                throw new ArgumentException(DocumentValidationMessage.ContentIsEmpty);

            if (formatConversion.GetFormatType(dataFile.FileName) != FormatConversionType.Docx)
                throw new NotSupportedException(DocumentValidationMessage.SupportsOnlyDocx);

            using var ms = new MemoryStream(dataFile.Content);
            using var document = DocX.Load(ms);
            ReplaceVariablesInDocument(document, dataFile.VariablesDocuments, cancellation);
            ReplaceVariablesInTables(document, dataFile.Tables, cancellation);

            var outputStream = new MemoryStream();
            document.SaveAs(outputStream);
            outputStream.Seek(0, SeekOrigin.Begin);

            return Task.FromResult(outputStream.ToArray());
        }

        /// <summary>
        /// Replaces variables within the document content.
        /// </summary>
        /// <param name="document">The DOCX document to be processed.</param>
        /// <param name="variables">The dictionary of variables and their values.</param>
        /// <param name="cancellation">Token for canceling the operation.</param>
        private static void ReplaceVariablesInDocument(DocX document, IDictionary<string, string> variables, CancellationToken cancellation = default)
        {
            foreach (var paragraph in document.Paragraphs)
            {
                cancellation.ThrowIfCancellationRequested();
                foreach (var variable in variables)
                {
                    cancellation.ThrowIfCancellationRequested();
                    if (paragraph.Text.Contains(variable.Key))
                    {
                        paragraph.ReplaceText(new StringReplaceTextOptions
                        {
                            SearchValue = variable.Key,
                            NewValue = variable.Value
                        });
                    }
                }
            }

            foreach (var table in document.Tables)
            {
                ReplaceVariablesInTable(table, variables, cancellation);
            }
        }

        /// <summary>
        /// Replaces variables in tables within the document.
        /// </summary>
        /// <param name="document">The DOCX document to be processed.</param>
        /// <param name="tables">The tables that need to be processed.</param>
        /// <param name="cancellation">Token for canceling the operation.</param>
        private static void ReplaceVariablesInTables(DocX document, IList<DataTable> dataTables, CancellationToken cancellation = default)
        {
            foreach (var dataTable in dataTables)
            {
                cancellation.ThrowIfCancellationRequested();

                // Tenta encontrar uma tabela correspondente no documento pelo nome (caption)
                var docTable = document.Tables.FirstOrDefault(t => t.TableCaption?.Equals(dataTable.Name, StringComparison.OrdinalIgnoreCase) == true);

                // Se não encontrar por caption, tenta por correspondência de cabeçalhos
                docTable ??= FindMatchingTableByHeader(document.Tables, dataTable);

                if (docTable == null)
                    continue;

                if (docTable.RowCount == 0)
                    continue; // pula a tabela vazia

                if (dataTable.Rows.Count == 0)
                    continue;

                // Assume a primeira linha como template
                var templateRow = docTable.Rows.Last(); // pode ser header ou uma linha de exemplo

                foreach (var dataRow in dataTable.Rows)
                {
                    cancellation.ThrowIfCancellationRequested();

                    var newRow = docTable.InsertRow(templateRow);
                    for (int i = 0; i < dataRow.Columns.Count; i++)
                    {
                        var column = dataRow.Columns[i];
                        var cell = newRow.Cells.ElementAtOrDefault(i);
                        if (cell == null)
                            continue;

                        foreach (var paragraph in cell.Paragraphs)
                        {
                            paragraph.ReplaceText(new StringReplaceTextOptions
                            {
                                SearchValue = $"{{{{{column.Name}}}}}",
                                NewValue = column.Content
                            });
                        }
                    }
                }

                templateRow.Remove();
            }
        }

        private static Table? FindMatchingTableByHeader(IReadOnlyList<Table> docTables, DataTable dataTable)
        {
            var columnNames = dataTable.Rows.FirstOrDefault()?.Columns.Select(c => c.Name).ToList();
            if (columnNames == null || columnNames.Count == 0)
                return null;

            foreach (var table in docTables)
            {
                var headerRow = table.Rows.FirstOrDefault();
                if (headerRow == null)
                    continue;

                var headerTexts = headerRow.Cells.Select(c => c.Paragraphs.FirstOrDefault()?.Text ?? "").ToList();

                // Checa se pelo menos 50% dos nomes de coluna estão no header
                int matchCount = columnNames.Count(col => headerTexts.Any(header => header.Contains(col, StringComparison.OrdinalIgnoreCase)));
                if (matchCount >= columnNames.Count / 2)
                    return table;
            }

            return null;
        }

        /// <summary>
        /// Replaces variables within a table.
        /// </summary>
        /// <param name="table">The table in the document.</param>
        /// <param name="variables">The dictionary of variables and their values.</param>
        /// <param name="cancellation">Token for canceling the operation.</param>
        private static void ReplaceVariablesInTable(Table table, IDictionary<string, string> variables, CancellationToken cancellation = default)
        {
            foreach (var row in table.Rows)
            {
                cancellation.ThrowIfCancellationRequested();
                foreach (var cell in row.Cells)
                {
                    cancellation.ThrowIfCancellationRequested();
                    foreach (var variable in variables)
                    {
                        cancellation.ThrowIfCancellationRequested();
                        foreach (var paragraph in cell.Paragraphs)
                        {
                            cancellation.ThrowIfCancellationRequested();
                            if (paragraph.Text.Contains(variable.Key))
                            {
                                paragraph.ReplaceText(new StringReplaceTextOptions
                                {
                                    SearchValue = variable.Key,
                                    NewValue = variable.Value
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}
