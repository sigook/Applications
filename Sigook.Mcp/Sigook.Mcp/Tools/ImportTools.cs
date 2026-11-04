using System.ComponentModel;
using ModelContextProtocol.Server;
using Sigook.Mcp.Instructions;

namespace Sigook.Mcp.Tools;

[McpServerToolType]
public static class ImportTools
{
    [McpServerTool(Name = "get_import_instructions"), Description("Returns the step-by-step workflow to import candidates from a job-board export into Sigook, plus the column mapping of a source (ZipRecruiter, ...). Call it with no source to list the known sources and the CSV headers that identify each one; call it with the headers of the CSV to auto-detect the source. Always read this before importing.")]
    public static object GetImportInstructions(
        ImportInstructions instructions,
        [Description("Source name, e.g. ziprecruiter. Optional.")] string? source = null,
        [Description("CSV headers as returned by read_export, used to auto-detect the source. Optional.")] string[]? headers = null)
    {
        var mapping = source is not null ? instructions.GetMapping(source)
            : headers is { Length: > 0 } ? instructions.Detect(headers)
            : null;
        return new
        {
            workflow = instructions.Workflow,
            detectedSource = mapping?.Name,
            mapping = mapping?.Markdown,
            knownSources = instructions.Mappings.Select(m => new { m.Name, m.Title, m.Description, detectHeaders = m.DetectHeaders })
        };
    }
}

[McpServerPromptType]
public static class ImportPrompts
{
    [McpServerPrompt(Name = "import-candidates"), Description("Import candidates from a job-board export (folder or zip with CSV + resumes) into Sigook.")]
    public static string ImportCandidates(
        ImportInstructions instructions,
        [Description("Folder or zip path of the export.")] string path,
        [Description("Source name (ziprecruiter, ...). Leave empty to auto-detect from the CSV headers.")] string? source = null)
    {
        var mapping = string.IsNullOrWhiteSpace(source) ? null : instructions.GetMapping(source);
        var mappingSection = mapping is null
            ? $"Detect the source: call `get_import_instructions` with the CSV headers from `read_export`. Known sources: {string.Join(", ", instructions.Mappings.Select(m => m.Name))}."
            : $"# Source mapping: {mapping.Title}\n\n{mapping.Markdown}";
        return $"Import the candidates found at `{path}` into Sigook.\n\n{instructions.Workflow}\n\n{mappingSection}";
    }
}

[McpServerResourceType]
public static class ImportResources
{
    [McpServerResource(UriTemplate = "sigook://instructions/import-candidates", Name = "import-candidates", MimeType = "text/markdown"), Description("Generic workflow to import candidates into Sigook.")]
    public static string Workflow(ImportInstructions instructions) => instructions.Workflow;

    [McpServerResource(UriTemplate = "sigook://mappings/{source}", Name = "source-mapping", MimeType = "text/markdown"), Description("Column mapping of a job-board export (ziprecruiter, ...) to Sigook fields.")]
    public static string Mapping(ImportInstructions instructions, string source) => instructions.GetMapping(source).Markdown;
}
