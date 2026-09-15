namespace Sigook.Functions.Models;

public record CraTable(CraTableKind Kind, ImportCraTableFromBlobModel Import)
{
    public string Label => Kind == CraTableKind.Cpp ? "CPP" : "income tax";
}
