namespace Covenant.Common.Interfaces
{
    public interface IDocumentService
    {
        Task DeleteFile(Guid documentId);
    }
}
