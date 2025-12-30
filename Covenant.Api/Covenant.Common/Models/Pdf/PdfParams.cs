namespace Covenant.Common.Models.Pdf
{
    public readonly record struct PdfParams(string Name, string Html)
    {
        public override string ToString()
        {
            return $"FileName: {Name}, Html: {Html}";
        }
    }
}
