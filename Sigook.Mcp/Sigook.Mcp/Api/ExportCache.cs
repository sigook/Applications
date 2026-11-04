using ModelContextProtocol;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using Sigook.Mcp.Configuration;

namespace Sigook.Mcp.Api;

public sealed class ExportCache
{
    private readonly string root = Path.Combine(SigookEnvironment.DataDirectory, "exports");

    public string Materialize(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (Directory.Exists(fullPath))
        {
            var zips = Directory.GetFiles(fullPath, "*.zip", SearchOption.TopDirectoryOnly);
            if (zips.Length == 0) return fullPath;
            var target = TargetFor(fullPath, zips);
            foreach (var zip in zips) ExtractIfMissing(zip, target);
            foreach (var file in Directory.GetFiles(fullPath).Where(f => !f.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)))
            {
                var destination = Path.Combine(target, Path.GetFileName(file));
                if (!File.Exists(destination)) File.Copy(file, destination);
            }
            return target;
        }
        if (File.Exists(fullPath) && fullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            var target = TargetFor(fullPath, [fullPath]);
            ExtractIfMissing(fullPath, target);
            return target;
        }
        if (File.Exists(fullPath)) return Path.GetDirectoryName(fullPath)!;
        throw new McpException($"Path not found: {fullPath}");
    }

    private string TargetFor(string source, string[] zips)
    {
        var fingerprint = string.Join("|", zips.OrderBy(z => z).Select(z => $"{z}:{new FileInfo(z).Length}:{File.GetLastWriteTimeUtc(z).Ticks}"));
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source + fingerprint)))[..16];
        var target = Path.Combine(root, hash);
        Directory.CreateDirectory(target);
        return target;
    }

    private static void ExtractIfMissing(string zip, string target)
    {
        var marker = Path.Combine(target, $".{Path.GetFileNameWithoutExtension(zip)}.extracted");
        if (File.Exists(marker)) return;
        ZipFile.ExtractToDirectory(zip, target, overwriteFiles: true);
        File.WriteAllText(marker, DateTime.UtcNow.ToString("O"));
    }
}
