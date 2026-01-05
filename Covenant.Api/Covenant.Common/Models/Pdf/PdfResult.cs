namespace Covenant.Common.Models.Pdf
{
    public readonly struct PdfResult
    {
        public PdfResult(string path) => Path = path;

        public string Path { get; }
    }
}
