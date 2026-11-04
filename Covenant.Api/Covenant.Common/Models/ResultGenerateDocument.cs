namespace Covenant.Common.Models;

public class ResultGenerateDocument<T>(T document, string documentName, string documentPath)
{
    public string DocumentName { get; } = documentName;
    public string DocumentPath { get; } = documentPath;
    public T Document { get; } = document;
}
