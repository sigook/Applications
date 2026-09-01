using Covenant.Common.Interfaces.Storage;
using Covenant.Common.Models;

namespace Covenant.Api.Utils;

public class DefaultLogoProvider(IFilesContainer filesContainer) : IDefaultLogoProvider
{
    public async Task<CovenantFileModel> GetLogo(string name)
    {
        using var content = new CreateDefaultLogo().Create(name);
        if (content is null) return null;
        string fileName = $"default{Guid.NewGuid()}.png";
        await filesContainer.UploadAsync(content, fileName);
        return new CovenantFileModel(fileName, name);
    }
}
