using Covenant.Api.Common;
using Covenant.Common.Configuration;
using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

namespace Covenant.Api.Utils
{
    public class DefaultLogoProvider : IDefaultLogoProvider
    {
        private readonly FilesConfiguration filesConfiguration;
        private readonly IFilesContainer filesContainer;

        public DefaultLogoProvider(IOptions<FilesConfiguration> options, IFilesContainer filesContainer)
        {
            filesConfiguration = options.Value;
            this.filesContainer = filesContainer;
        }

        public async Task<CovenantFileModel> GetLogo(string name)
        {
            var c = new CreateDefaultLogo();
            IFileInfo logo = c.Create(name);
            var fileSaver = new FileSaver(filesConfiguration.FilesUrl);
            var result = await fileSaver.SaveImageProfile(logo.PhysicalPath, async (path, contentType) => await filesContainer.Upload(path, contentType));
            return new CovenantFileModel(result.Value.Path, name);
        }
    }
}