using Covenant.Common.Functionals;
using Covenant.Common.Models;

namespace Covenant.Common.Interfaces
{
    public interface IPdfGeneratorService
    {
        Task<Result<byte[]>> GeneratePdfFromHtml(PdfParams pdfParams);
    }
}
