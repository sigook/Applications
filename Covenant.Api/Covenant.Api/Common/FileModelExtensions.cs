using Covenant.Common.Entities;
using Covenant.Common.Models;
using System.Net.Http.Headers;

namespace Covenant.Api.Common
{
    public static class FileModelExtensions
    {
        public static CovenantFile ToCovenantFile(this CovenantFileModel model, CovenantFile covenantFile)
        {
            if (model is null) return null;
            if (covenantFile is null) return new CovenantFile(model.FileName, model.Description);
            covenantFile.FileName = model.FileName;
            covenantFile.Description = model.Description;
            return covenantFile;
        }

        public static string GetCleanName(this IFormFile file)
        {
            if (file is null) return null;
            string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName;
            return fileName.Contains("\\")
                ? fileName.Trim('"').Substring(fileName.LastIndexOf("\\", StringComparison.Ordinal) + 1)
                : fileName.Trim('"');
        }
    }
}