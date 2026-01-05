namespace Covenant.Common.Models
{
    public class ResultGenerateDocument<T>
    {
        public ResultGenerateDocument(T document, string documentName, string documentPath)
        {
            Document = document;
            DocumentName = documentName;
            DocumentPath = documentPath;
        }

        public string DocumentName { get; }
        public string DocumentPath { get; }
        public T Document { get; }
    }
}
