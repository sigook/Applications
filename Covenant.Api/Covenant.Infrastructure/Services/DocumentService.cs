using Covenant.Common.Interfaces;
using Covenant.Common.Interfaces.Storage;
using Covenant.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Covenant.Infrastructure.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly CovenantContext context;
        private readonly IFilesContainer filesContainer;
        private readonly IPayStubsContainer payStubsContainer;
        private readonly IInvoicesContainer invoicesContainer;

        public DocumentService(
            CovenantContext context, 
            IFilesContainer filesContainer, 
            IPayStubsContainer payStubsContainer, 
            IInvoicesContainer invoicesContainer)
        {
            this.context = context;
            this.filesContainer = filesContainer;
            this.payStubsContainer = payStubsContainer;
            this.invoicesContainer = invoicesContainer;
        }

        public async Task DeleteFile(Guid documentId)
        {
            var document = await context.CovenantFile.FirstOrDefaultAsync(cf => cf.Id == documentId);
            await filesContainer.DeleteFileIfExists(document.FileName);
            context.CovenantFile.Remove(document);
            await context.SaveChangesAsync();
        }
    }
}
