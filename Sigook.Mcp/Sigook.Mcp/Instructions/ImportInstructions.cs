using System.Reflection;
using ModelContextProtocol;

namespace Sigook.Mcp.Instructions;

public sealed record SourceMapping(string Name, string Title, string Description, string[] DetectHeaders, string Markdown);

public sealed class ImportInstructions
{
    private const string Prefix = "Sigook.Mcp.Instructions.";
    private const string WorkflowResource = Prefix + "workflow.md";

    public string Workflow { get; } = Read(WorkflowResource);

    public IReadOnlyList<SourceMapping> Mappings { get; } = Assembly.GetExecutingAssembly()
        .GetManifestResourceNames()
        .Where(n => n.StartsWith(Prefix) && n.EndsWith(".md") && n != WorkflowResource)
        .Select(n => Parse(Read(n)))
        .OrderBy(m => m.Name)
        .ToList();

    public SourceMapping GetMapping(string source) =>
        Mappings.FirstOrDefault(m => m.Name.Equals(source.Trim(), StringComparison.OrdinalIgnoreCase))
        ?? throw new McpException($"Unknown source '{source}'. Known sources: {string.Join(", ", Mappings.Select(m => m.Name))}.");

    public SourceMapping? Detect(IEnumerable<string> headers)
    {
        var set = headers.ToHashSet(StringComparer.OrdinalIgnoreCase);
        return Mappings.FirstOrDefault(m => m.DetectHeaders.Length > 0 && m.DetectHeaders.All(set.Contains));
    }

    private static string Read(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded resource {resourceName} is missing");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static SourceMapping Parse(string content)
    {
        var lines = content.Split('\n').Select(l => l.TrimEnd('\r')).ToList();
        if (lines.Count == 0 || lines[0] != "---") throw new InvalidOperationException("Mapping file must start with a front matter block");
        var end = lines.IndexOf("---", 1);
        var meta = lines[1..end]
            .Select(l => l.Split(':', 2))
            .Where(p => p.Length == 2)
            .ToDictionary(p => p[0].Trim(), p => p[1].Trim(), StringComparer.OrdinalIgnoreCase);
        var markdown = string.Join('\n', lines[(end + 1)..]).Trim();
        var detect = meta.GetValueOrDefault("detect", string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new SourceMapping(
            meta["name"],
            meta.GetValueOrDefault("title", meta["name"]),
            meta.GetValueOrDefault("description", string.Empty),
            detect,
            markdown);
    }
}
