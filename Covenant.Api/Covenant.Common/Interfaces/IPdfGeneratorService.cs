using Covenant.Common.Functionals;
using Covenant.Common.Models.Pdf;

namespace Covenant.Common.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<Result<byte[]>> GeneratePdfFromHtml(PdfParams pdfParams);
    }
}
