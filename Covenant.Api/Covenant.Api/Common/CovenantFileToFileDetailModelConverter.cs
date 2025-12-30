using AutoMapper;
using Covenant.Common.Configuration;
using Covenant.Common.Entities;
using Covenant.Common.Models;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Common
{
    public class CovenantFileToFileDetailModelConverter : ITypeConverter<CovenantFile, CovenantFileModel>
    {
        private readonly FilesConfiguration filesConfiguration;
        public CovenantFileToFileDetailModelConverter(IOptions<FilesConfiguration> options) =>
            filesConfiguration = options.Value;

        public CovenantFileModel Convert(CovenantFile source, CovenantFileModel destination, ResolutionContext context)
        {
            if (source is null) return null;
            return new CovenantFileModel
            {
                Id = source.Id,
                FileName = source.FileName,
                Description = source.Description,
                PathFile = string.Concat(filesConfiguration.FilesPath, source.FileName)
            };
        }
    }
}