using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models.Pdf;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Covenant.Infrastructure.Services;

public class PdfGeneratorService(IHttpClientFactory httpClientFactory) : IPdfGeneratorService
{
    public const string PdfGeneratorClient = "PdfGenerator";

    public async Task<Result<byte[]>> GeneratePdfFromHtml(PdfParams pdfParams)
    {
        string fileName = pdfParams.Name.ToLower().EndsWith(".pdf") ? pdfParams.Name : $"{pdfParams.Name}.pdf";
        string dataAsString = JsonSerializer.Serialize(new { Name = fileName, pdfParams.Html });
        var content = new StringContent(dataAsString);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        var client = httpClientFactory.CreateClient(PdfGeneratorClient);
        var response = await client.PostAsync("/Pdf/Pdf", content);
        if (!response.IsSuccessStatusCode) return Result.Fail<byte[]>("PDF Service not available");
        var bytes = await response.Content.ReadAsByteArrayAsync();
        return Result.Ok(bytes);
    }
}
