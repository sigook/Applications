using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Services;

public class DocumentService(
    CovenantContext context,
    IFilesContainer filesContainer) : IDocumentService
{
    public async Task DeleteFile(Guid documentId)
    {
        var document = await context.CovenantFiles.FirstOrDefaultAsync(cf => cf.Id == documentId);
        await filesContainer.DeleteFileIfExists(document.FileName);
        context.CovenantFiles.Remove(document);
        await context.SaveChangesAsync();
    }
}
