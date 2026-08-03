using System.Text.Json.Serialization;

namespace Sigook.Functions.Models;

public class ImportCraTableFromBlobModel
{
    public string BlobName { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public PayPeriod PayPeriod { get; set; }

    public int Year { get; set; }
}
