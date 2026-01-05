using Covenant.Common.Functionals;
using Covenant.Common.Interfaces;
using Covenant.Common.Models;
using Covenant.Common.Models.Pdf;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Covenant.Infrastructure.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public const string PdfGeneratorClient = "PdfGenerator";
        private readonly IHttpClientFactory httpClientFactory;

        public PdfGeneratorService(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<Result<MemoryStream>> GeneratePdfFromHtml(string name, string html)
        {
            string fileName = AddPdfExtension(name);
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string dataAsString = JsonConvert.SerializeObject(new { Name = "Payroll", Html = html });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = httpClientFactory.CreateClient(PdfGeneratorClient);
            var response = await client.PostAsync("/Pdf/Pdf", content);
            if (!response.IsSuccessStatusCode) return Result.Fail<MemoryStream>("PDF Service not available");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(filePath, bytes);
            var memoryStream = new MemoryStream();
            using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;
            return Result.Ok(memoryStream);
        }

        public async Task<Result<PdfResult>> GeneratePdfFromHtml(PdfParams pdfParams)
        {
            string fileName = AddPdfExtension(pdfParams.Name);
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            string dataAsString = JsonConvert.SerializeObject(new { Name = fileName, pdfParams.Html });
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var client = httpClientFactory.CreateClient(PdfGeneratorClient);
            var response = await client.PostAsync("/Pdf/Pdf", content);
            if (!response.IsSuccessStatusCode) return Result.Fail<PdfResult>("PDF Service not available");
            var bytes = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(filePath, bytes);
            return Result.Ok(new PdfResult(filePath));
        }

        private static string AddPdfExtension(string name) => name.ToLower().EndsWith(".pdf") ? name : $"{name}.pdf";
    }
}