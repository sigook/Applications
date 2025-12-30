namespace Covenant.Common.Models
{
    public class EmailAttachment
    {
        public EmailAttachment(string name, string mediaType, Stream contentStream)
        {
            Name = name;
            MediaType = mediaType;
            ContentStream = contentStream;
        }
        public EmailAttachment(string name, string mediaType, string filePath)
        {
            Name = name;
            MediaType = mediaType;
            FilePath = filePath;
        }

        public string Name { get; private set; }
        public string MediaType { get; private set; }
        public Stream ContentStream { get; private set; }
        public string FilePath { get; private set; }

        public bool IsPath => !string.IsNullOrEmpty(FilePath);
    }
}