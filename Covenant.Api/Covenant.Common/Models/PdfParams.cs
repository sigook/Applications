using System.Text.Json.Serialization;
namespace Covenant.Common.Models;

public readonly record struct PdfParams(string Name, string Html)
{
    public override string ToString()
    {
        return $"FileName: {Name}, Html: {Html}";
    }
}
