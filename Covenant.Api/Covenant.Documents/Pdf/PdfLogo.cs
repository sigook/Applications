using Covenant.Common.Interfaces.Storage;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using static Covenant.Documents.Pdf.PdfLayout;

namespace Covenant.Documents.Pdf;

public sealed record PdfLogo(Image Image, float AspectRatio)
{
    private const float MaxHeight = 85 * Px;

    public static async Task<PdfLogo> Load(IFilesContainer filesContainer, string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return null;
        byte[] content = await filesContainer.Download(fileName);
        if (content is not { Length: > 0 }) return null;
        try
        {
            return new PdfLogo(Image.FromBinaryData(content), ReadAspectRatio(content));
        }
        catch
        {
            return null;
        }
    }

    public void Compose(IContainer container, float width) =>
        container.AlignLeft().Width(width).Height(Math.Min(MaxHeight, width * AspectRatio))
            .Image(Image).UseOriginalImage().FitUnproportionally();

    private static float ReadAspectRatio(byte[] content)
    {
        if (content.Length > 24 && content[0] == 0x89 && content[1] == 'P' && content[2] == 'N' && content[3] == 'G')
            return Ratio(ReadInt32(content, 16), ReadInt32(content, 20));

        if (content.Length > 4 && content[0] == 0xFF && content[1] == 0xD8)
        {
            int i = 2;
            while (i + 9 < content.Length && content[i] == 0xFF)
            {
                byte marker = content[i + 1];
                bool isFrameHeader = marker is >= 0xC0 and <= 0xCF and not 0xC4 and not 0xC8 and not 0xCC;
                if (isFrameHeader) return Ratio(ReadInt16(content, i + 7), ReadInt16(content, i + 5));
                i += 2 + ReadInt16(content, i + 2);
            }
        }

        return 1;
    }

    private static float Ratio(int width, int height) => width > 0 && height > 0 ? (float)height / width : 1;

    private static int ReadInt32(byte[] content, int offset) =>
        content[offset] << 24 | content[offset + 1] << 16 | content[offset + 2] << 8 | content[offset + 3];

    private static int ReadInt16(byte[] content, int offset) => content[offset] << 8 | content[offset + 1];
}
