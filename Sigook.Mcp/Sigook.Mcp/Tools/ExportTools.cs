using System.ComponentModel;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using ModelContextProtocol.Server;
using Sigook.Mcp.Api;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static class ExportTools
{
    private static readonly string[] DocumentExtensions = [".pdf", ".doc", ".docx"];

    [McpServerTool(Name = "read_export"), Description("Reads a job-board export (a folder or a zip containing CSV files and resume documents). Returns the CSV rows as raw header/value maps, untouched, plus the local path of every document found so it can be passed to create_candidate. Zips are extracted once into a local cache. Use skip/take to page through large files.")]
    public static object ReadExport(
        ExportCache cache,
        [Description("Folder or .zip path of the export. A folder may contain several zips; all of them are read.")] string path,
        [Description("Rows to skip, per CSV file. Default 0.")] int skip = 0,
        [Description("Maximum rows to return, per CSV file. Default 100.")] int take = 100)
    {
        var root = cache.Materialize(path);
        var csvFiles = Directory.GetFiles(root, "*.csv", SearchOption.AllDirectories)
            .OrderBy(f => f)
            .Select(file => ReadCsv(file, root, skip, take))
            .ToList();
        var documents = Directory.GetFiles(root, "*", SearchOption.AllDirectories)
            .Where(f => DocumentExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .Select(f => new
            {
                fileName = Path.GetFileName(f),
                relativePath = Path.GetRelativePath(root, f).Replace('\\', '/'),
                path = f,
                sizeBytes = new FileInfo(f).Length
            })
            .ToList();
        return new { root, csvFiles, documents };
    }

    private static object ReadCsv(string file, string root, int skip, int take)
    {
        using var reader = new StreamReader(file);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            DetectDelimiter = true,
            BadDataFound = null,
            MissingFieldFound = null
        });
        csv.Read();
        csv.ReadHeader();
        var headers = csv.HeaderRecord ?? [];
        var rows = new List<Dictionary<string, string>>();
        var total = 0;
        while (csv.Read())
        {
            total++;
            if (total <= skip || rows.Count >= take) continue;
            var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var header in headers) row[header] = csv.GetField(header) ?? string.Empty;
            rows.Add(row);
        }
        return new
        {
            fileName = Path.GetRelativePath(root, file).Replace('\\', '/'),
            headers,
            totalRows = total,
            skip,
            returned = rows.Count,
            rows
        };
    }
}
