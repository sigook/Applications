using Covenant.Common.Entities;

namespace Covenant.Common.Models;

public class CovenantFileModel : ICovenantFile, IEquatable<CovenantFileModel>
{
    public CovenantFileModel()
    {
    }

    public CovenantFileModel(string fileName)
    {
        FileName = fileName;
    }

    public CovenantFileModel(string fileName, string description)
    {
        FileName = fileName;
        Description = description;
    }

    public Guid Id { get; set; }
    public string PathFile { get; set; }
    public string FileName { get; set; }
    public string Description { get; set; }
    public bool CanDownload { get; set; }

    public bool Equals(CovenantFileModel other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return FileName == other.FileName;
    }
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CovenantFileModel)obj);
    }
    public override int GetHashCode() => FileName != null ? FileName.GetHashCode() : 0;
    public static bool operator ==(CovenantFileModel left, CovenantFileModel right) => Equals(left, right);
    public static bool operator !=(CovenantFileModel left, CovenantFileModel right) => !Equals(left, right);
}
