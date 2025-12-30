using Covenant.Common.Configuration;
using Covenant.Common.Functionals;
using Covenant.Common.Models;
using ImageMagick;
using Microsoft.AspNetCore.StaticFiles;
using System.Text.RegularExpressions;

namespace Covenant.Api.Common
{
    public class FileSaver
    {
        private readonly FilesConfiguration filesConfiguration;

        public FileSaver(string filesUrl)
        {
            filesConfiguration = new FilesConfiguration 
            { 
                FilesUrl = filesUrl,
                FilesPath = filesUrl,
            };
        }

        public FileSaver(FilesConfiguration filesConfiguration) => this.filesConfiguration = filesConfiguration;

        public async Task<Result<List<FilesResult>>> SaveFiles(IEnumerable<IFormFile> files, string[] validExtensions, SigookFileOptions options, Action<string, string> upload)
        {
            var addTag = false;
            if (!string.IsNullOrEmpty(options.Tag))
            {
                options.Tag = Regex.Replace(options.Tag, @"\s+", "");
                addTag = true;
            }
            var filesLocation = new List<FilesResult>();
            if (!Directory.Exists(filesConfiguration.FilesUrl))
            {
                Directory.CreateDirectory(filesConfiguration.FilesUrl);
            }
            foreach (var file in files)
            {
                if (file is null) continue;
                string extension = Path.GetExtension(file.GetCleanName());
                if (!validExtensions.Any(c => extension.EndsWith(c, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Result.Fail<List<FilesResult>>($"Invalid format {extension}");
                }

                string fileName = $"{Guid.NewGuid():N}{extension}";
                if (addTag) fileName = string.Concat(options.Tag, fileName);
                string path = Path.Combine(filesConfiguration.FilesUrl, fileName);
                if (file.Length <= 0) continue;
                using (var stream = new FileStream(path, FileMode.Create)) await file.CopyToAsync(stream);
                filesLocation.Add(new FilesResult 
                { 
                    Path = fileName,
                    FullPath = Path.Combine(filesConfiguration.FilesPath, fileName)
                });

                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
                upload.Invoke(path, contentType);
            }

            return Result.Ok(filesLocation);
        }

        public async Task<Result<FilesResult>> SaveImageProfile(IEnumerable<IFormFile> files, string[] validExtensions, SigookFileOptions options, Action<string, string> upload)
        {
            if (!Directory.Exists(filesConfiguration.FilesUrl)) Directory.CreateDirectory(filesConfiguration.FilesUrl);
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file.GetCleanName());
                if (!validExtensions.Any(c => extension.EndsWith(c, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return Result.Fail<FilesResult>($"Invalid format {extension}");
                }

                string fileName = $"image_profile_{Guid.NewGuid():N}{extension}";
                string path = Path.Combine(filesConfiguration.FilesUrl, fileName);
                if (file.Length <= 0) continue;
                bool result = SaveResizeImage(file, path);
                result &= await RotateImage(file, path, options.Degrees);
                if (!result)
                    using (var stream = new FileStream(path, FileMode.Create)) await file.CopyToAsync(stream);
                new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
                upload.Invoke(path, contentType);
                return Result.Ok(new FilesResult 
                { 
                    Path = fileName,
                    FullPath = Path.Combine(filesConfiguration.FilesPath, fileName)
                });
            }

            return Result.Fail<FilesResult>();
        }

        public async Task<Result<FilesResult>> SaveImageProfile(string sourcePath, Action<string, string> upload)
        {
            if (!Directory.Exists(filesConfiguration.FilesUrl)) Directory.CreateDirectory(filesConfiguration.FilesUrl);
            string fileName = $"image_profile_{Path.GetFileName(sourcePath)}";
            string path = Path.Combine(filesConfiguration.FilesUrl, fileName);
            using (FileStream file = File.OpenRead(sourcePath))
            {
                using (var stream = new FileStream(path, FileMode.Create)) await file.CopyToAsync(stream);
            }
            new FileExtensionContentTypeProvider().TryGetContentType(fileName, out string contentType);
            upload.Invoke(path, contentType);
            return Result.Ok(new FilesResult { Path = fileName });
        }

        private static bool SaveResizeImage(IFormFile file, string pathWhereToSave)
        {
            try
            {
                using (Stream stream = file.OpenReadStream())
                {
                    using (var image = new MagickImage(stream))
                    {
                        var size = new MagickGeometry(400, 400) { IgnoreAspectRatio = true };
                        image.Resize(size);
                        image.Write(pathWhereToSave);
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static async Task<bool> RotateImage(IFormFile file, string pathWhereToSave, double? degrees)
        {
            if (degrees.HasValue)
            {
                using var content = file.OpenReadStream();
                try
                {
                    var image = new MagickImage(content);
                    image.Rotate(degrees.Value);
                    await image.WriteAsync(pathWhereToSave);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }

    }
}