using Covenant.Common.Functionals;
using Covenant.Common.Models.Pdf;

namespace Covenant.Common.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<Result<MemoryStream>> GeneratePdfFromHtml(string name, string html);
        Task<Result<PdfResult>> GeneratePdfFromHtml(PdfParams pdfParams);
    }
}
