using Covenant.Common.Functionals;
using Covenant.Common.Resources;

namespace Covenant.Common.Entities;

public class CovenantFile : ICovenantFile
{
    public CovenantFile() 
    { 
    }

    public CovenantFile(string fileName, string description = null)
    {
        FileName = fileName;
        Description = description;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string Description { get; set; }

    public CovenantFile UpdateFileName(string fileName)
    {
        FileName = fileName;
        return this;
    }

    public CovenantFile UpdateDescription(string description)
    {
        Description = description;
        return this;
    }

    public void Update(ICovenantFile newFile)
    {
        if (newFile is null) return;
        FileName = newFile.FileName;
        Description = newFile.Description;
    }

    public static Result<CovenantFile> Create(ICovenantFile file) =>
        string.IsNullOrEmpty(file?.FileName)
            ? Result.Fail<CovenantFile>(ValidationMessages.RequiredMsg("Filename"))
            : Result.Ok(new CovenantFile(file.FileName, file.Description));

    public static Result<CovenantFile> Create(string fileName) =>
        string.IsNullOrEmpty(fileName)
            ? Result.Fail<CovenantFile>(ValidationMessages.RequiredMsg("Filename"))
            : Result.Ok(new CovenantFile(fileName));
}
